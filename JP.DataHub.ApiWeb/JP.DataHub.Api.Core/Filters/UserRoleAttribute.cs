using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Api.Core.DataContainer;

namespace JP.DataHub.Api.Core.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple =true)]
    public class UserRoleAttribute : ActionFilterAttribute
    {
        private const string ACCESS_DENIED = "操作へのアクセスが許可されていません";

        private IApiDataContainer _dataContainer { get => DataContainerUtil.ResolveDataContainer() as IApiDataContainer; }
        private Lazy<IUserRoleCheckService> _lazyUserRoleCheckService => new Lazy<IUserRoleCheckService>(() => UnityCore.Resolve<IUserRoleCheckService>());
        private IUserRoleCheckService _userRoleCheckService { get => _lazyUserRoleCheckService.Value; }

        private static Lazy<IList<string>> s_lazyOperatingVendorVendorId = new Lazy<IList<string>>(() => UnityCore.Resolve<IList<string>>("OperatingVendorVendorId"));
        private static IList<string> s_operatingVendorVendorId { get => s_lazyOperatingVendorVendorId.Value; }

        private string _functionName;
        private UserRoleAccessType _access;
        private bool _isOnlyVendorAccess;
        private Type _typeValidator;
        private bool _isPass = false;
        private IUserRoleValudator _validator;

        /// <summary>
        /// ユーザーのロールをチェックする
        /// この場合は画面IDが指定されていないので、その場合はクラスに指定されているUserRoleCheckController属性から拾う
        /// </summary>
        /// <param name="access">アクセスモード</param>
        /// <param name="isVendorAccess">ベンダーに関する操作か？</param>
        public UserRoleAttribute(UserRoleAccessType access, bool isOnlyVendorAccess)
        {
            _access = access;
            _isOnlyVendorAccess = isOnlyVendorAccess;
        }

        /// <summary>
        /// ユーザーのロールをチェックする
        /// この場合は画面IDが指定されていないので、その場合はクラスに指定されているUserRoleCheckController属性から拾う
        /// </summary>
        /// <param name="access">アクセスモード</param>
        /// <param name="isVendorAccess">ベンダーに関する操作か？</param>
        public UserRoleAttribute(UserRoleAccessType access, Type typeValidator = null, bool isOnlyVendorAccess = true)
        {
            _access = access;
            _isOnlyVendorAccess = isOnlyVendorAccess;
            _typeValidator = typeValidator;
            if (_typeValidator != null)
            {
                var obj = Activator.CreateInstance(_typeValidator);
                if ((obj is IUserRoleValudator) == false)
                {
                    throw new Exception("typeValidatorはIUserRoleValudatorから継承しなければなりません");
                }
                _validator = obj as IUserRoleValudator;
            }
        }

        /// <summary>
        /// ユーザーのロールをチェックする
        /// </summary>
        /// <param name="functionName">画面ID</param>
        /// <param name="access">アクセスモード</param>
        /// <param name="isVendorAccess">ベンダーに関する操作か？</param>
        public UserRoleAttribute(string functionName, UserRoleAccessType access, bool isOnlyVendorAccess)
        {
            _functionName = functionName;
            _access = access;
            _isOnlyVendorAccess = isOnlyVendorAccess;
        }

        /// <summary>
        /// ユーザーのロールをチェックする
        /// </summary>
        /// <param name="functionName">画面ID</param>
        /// <param name="access">アクセスモード</param>
        /// <param name="isVendorAccess">ベンダーに関する操作か？</param>
        public UserRoleAttribute(string functionName, UserRoleAccessType access, Type typeValidator = null, bool isOnlyVendorAccess = true)
        {
            _functionName = functionName;
            _access = access;
            _isOnlyVendorAccess = isOnlyVendorAccess;
            _typeValidator = typeValidator;
            if (_typeValidator != null)
            {
                var obj = Activator.CreateInstance(_typeValidator);
                if ((obj is IUserRoleValudator) == false)
                {
                    throw new Exception("typeValidatorはIUserRoleValudatorから継承しなければなりません");
                }
                _validator = obj as IUserRoleValudator;
            }
        }

        /// <summary>
        /// アクセスしているユーザーのRoleを見て、Actionメソッドを通すか判定する
        /// 判断する内容は２つ
        /// 0. HttpMethodType=POST,PUT,PATCH
        /// 1. Actionメソッドへのアクセスが許可されているかのチェック（管理画面からのアクセスできるものかの判定）
        /// 2. ユーザーが基盤管理者でない場合に、ベンダーに関する設定の場合、自ベンダーのみしか操作できないようにする
        /// ※Getの場合はExecutedにて確認
        /// 　Deleteの場合はビジネスロジック層で確認（するしかない）
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 運用管理ベンダーの人がアクセスしてきた場合は
            // ベンダーに依存するチェックは行わない
            var isOnlyVendorAccess = _isOnlyVendorAccess;
            if (s_operatingVendorVendorId.Contains(_dataContainer.VendorId?.ToLower()) == true)
            {
                return;
            }

            // HttpMethod = Deleteの場合
            // Deleteの場合は、自ベンダーへのアクセスかわからない
            // よってService層でチェックのためのロジックを書いてもらう
            // チェックする仕組みはPerRequestDataContainer.VendorCheckFuncを利用してもらうことによって、ややこしいことは気にする必要がなくなる
            if (context.HttpContext.Request.Method == "DELETE")
            {
                var pass = _userRoleCheckService.Check(_dataContainer.OpenId, _dataContainer.VendorId, _dataContainer.SystemId, _functionName, _access, isOnlyVendorAccess, context.ActionArguments);
                if (pass == false &&  isOnlyVendorAccess == true)
                {
                    _dataContainer.VendorCheckFunc = new Func<object, bool>(model => _userRoleCheckService.CheckModel(_dataContainer.VendorId, _dataContainer.SystemId, isOnlyVendorAccess, model));
                }
            }
            // HttpMethod = POST,PUT,PATCHの場合（RequestBodyにjsonがある場合）
            // その場合にはRequestBodyにvendorid,systemidが存在し、その値が自ベンダー・システムと同一かチェックする
            // もし違った場合には自ベンダー以外へのアクセスとみなすことが出来る
            else if (context.HttpContext.Request.Method == "POST" || context.HttpContext.Request.Method == "PUT" || context.HttpContext.Request.Method == "PATCH")
            {
                bool isPass = _userRoleCheckService.Check(_dataContainer.OpenId, _dataContainer.VendorId, _dataContainer.SystemId, _functionName, _access, isOnlyVendorAccess, context.ActionArguments);
                if (isPass == false && _validator != null)
                {
                    isPass = _validator.CheckModel(_dataContainer.OpenId, _dataContainer.VendorId, _dataContainer.SystemId, _functionName, _access, context.ActionArguments);
                }
                if (isPass == false)
                { 
                    context.Result = new ObjectResult(new { Message = ACCESS_DENIED }) { StatusCode = (int)HttpStatusCode.Forbidden };
                }
            }
        }

        /// <summary>
        /// アクセスしているユーザーのRoleを見て、Actionメソッドを通すか判定する
        /// 判断する内容は１つ
        /// 0. HttpMethodType=GET
        /// 1. 戻りのモデルにVendorId,SystemIdが含まれておりそれが合致するか？
        /// 2. ユーザーが基盤管理者でない場合に、ベンダーに関する設定の場合、自ベンダーのみしか操作できないようにする
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NullReferenceException"></exception>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.Method != "GET")
            {
                return;
            }

            // 運用管理ベンダーの人がアクセスしてきた場合は
            // ベンダーに依存するチェックは行わない
            var isOnlyVendorAccess = _isOnlyVendorAccess;
            if (s_operatingVendorVendorId.Contains(_dataContainer.VendorId?.ToLower()) == true)
            {
                return;
            }

            // GETの場合のみチェック
            // Getによって返されるモデルが自ベンダーかチェックする
            // Jsonのモデルにvendorid,systemidが存在し、かつその値が自ベンダーかのチェックを行う
            // もし自ベンダー以外なら、そのデータは閲覧できないのでエラーとする
            if (context.Result is OkObjectResult okor && okor.Value != null && okor.Value is ObjectResult or)
            {
                var dic = new Dictionary<string, object>();
                if (or.Value is IEnumerable array)
                {
                    foreach (var item in array)
                    {
                        if (_userRoleCheckService.CheckModel(_dataContainer.VendorId, _dataContainer.SystemId, isOnlyVendorAccess, item) == false)
                        {
                            context.Result = new ObjectResult(new { Message = ACCESS_DENIED }) { StatusCode = (int)HttpStatusCode.Forbidden };
                        }
                    }
                }
                else
                {
                    if (_userRoleCheckService.CheckModel(_dataContainer.VendorId, _dataContainer.SystemId, isOnlyVendorAccess, or.Value) == false)
                    {
                        context.Result = new ObjectResult(new { Message = ACCESS_DENIED }) { StatusCode = (int)HttpStatusCode.Forbidden };
                    }
                }
            }
        }

        private string GetCharsetFromContentType(string contentType)
        {
            if (contentType == null)
            {
                return null;
            }
            return contentType.Split(';').ToList().Select(x => x.Trim()).ToList().Where(x => x.StartsWith("charset=")).FirstOrDefault()?.Replace("charset=", "");
        }
    }
}
