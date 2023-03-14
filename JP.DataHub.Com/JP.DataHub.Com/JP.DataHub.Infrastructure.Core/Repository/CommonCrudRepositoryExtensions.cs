using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using JP.DataHub.MVC.Authentication;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Misc;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Consts;
using JP.DataHub.Infrastructure.Core.Repository.Attributes;
using JP.DataHub.Service.Core.Repository;
using Unity.Resolution;

namespace JP.DataHub.Infrastructure.Core.Repository
{
    public static class CommonCrudRepositoryExtensions
    {
        public const string ERROR_FAILDAO = "I can't find any of class, model, action.";

        public static T To<T>(this object result, bool isMap)
            => isMap ? result.Map<T>() : result.Convert<T>();

        public static List<T> ToMapList<T>(this object response) => ToList<T>(response, true);
        public static List<T> ToConvertList<T>(this object response) => ToList<T>(response, false);

        public static List<T> ToList<T>(this object response, bool isMap)
        {
            var result = new List<T>();
            if (response is IEnumerable<object> enumlist)
            {
                foreach (var item in enumlist)
                {
                    result.Add(isMap ? item.Map<T>() : item.Convert<T>());
                }
            }
            else if (response is List<T> objlist)
            {
                return objlist;
            }
            return result;
        }

        public static List<T> ToList<T>(this object response)
        {
            var result = new List<T>();
            if (response is IEnumerable<object> enumlist)
            {
                foreach (var item in enumlist)
                {
                    result.Add(item.To<T>());
                }
            }
            else if (response is List<T> objlist)
            {
                return objlist;
            }
            else if (response is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    result.Add(item.To<T>());
                }
            }
            return result;
        }

        public static object Map(this object obj, RepositoryDaoInfo dao)
        {
            return null;
        }

        private static object lockobj = new object();
        private static List<Type> alltypes = null;
        private static IServerEnvironment ServerEnvironment = UnityCore.Resolve<IServerEnvironment>();

        /// <summary>
        /// WebAPIを呼び出す情報から、リソースを定義したクラス（IResourceから派生した実態）を返す
        /// </summary>
        /// <param name="dao"></param>
        /// <returns></returns>
        public static object Init(this RepositoryDaoInfo dao)
        {
            object result = null;
            lock (lockobj)
            {
                if (alltypes == null)
                {
                    alltypes = new List<Type>();
                    AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(x => alltypes.AddRange(x.GetTypes()));
                }
            }
            if (dao.ClassType == null)
            {
                dao.ClassType = alltypes.Where(x => x.Name == $"{dao.ClassName}").FirstOrDefault();
            }
            if (dao.ModelType == null)
            {
                dao.ModelType = dao.ResultType != null ? dao.ResultType : alltypes.Where(x => x.Name == $"{dao.ModelName}").FirstOrDefault();
            }
            dao.ActionMethod = dao.ClassType?.GetMethod(dao.ActionName);
            if (dao.ClassType != null)
            {
                // IResourceのクラスを取得する
                
                result = UnityCore.Resolve(dao.ClassType, null, new ParameterOverride("serverEnvironment", ServerEnvironment));
                if (result != null && dao.ModelType == null && result is IResource resource)
                {
                    dao.ModelType = resource.ModelType;
                }
            }
            if (dao.ActionMethod == null)
            {
                dao.ActionMethod = result.GetType().GetMethod(dao.ActionName);
            }
            return dao.ClassType == null || dao.ModelType == null || dao.ActionMethod == null ? null : result;
        }

        public static WebApiResponseResult Call(this RepositoryDaoInfo dao, params object[] paramter)
        {
            var target = Init(dao);
            if (target == null)
            {
                return new WebApiResponseResult(new Exception(ERROR_FAILDAO));
            }

            // parameterがRepository.Methodの引数パラメータと合致し、かつ、変換属性があるなら変換する
            var stackframe = CallStack.FindStackFrame<ICommonCrudRepository>();
            var pargs = stackframe.GetMethod().GetParameters();
            for (int i = 0; i < pargs.Length; i++)
            {
                var convertattr = pargs[i].GetCustomAttribute<ConvertRequestModelAttribute>();
                if (convertattr != null && paramter[i - 1] != null)
                {
                    // i-1は最初の引数daoを除くため
                    var p = paramter[i - 1];
                    if (p is IEnumerable enumparam)
                    {
                        var convertedType = typeof(List<>).MakeGenericType(dao.ModelType);
                        var converted = Activator.CreateInstance(convertedType);
                        foreach (var x in enumparam)
                        {
                            converted.ExecuteMethod("Add", x.Map(dao.ModelType));
                        }
                        paramter[i - 1] = converted;
                    }
                    else
                    {
                        paramter[i - 1] = paramter[i - 1].Map(dao.ModelType);
                    }
                }
            }

            return CallAction(target, dao, paramter);
        }

        /// <summary>
        /// DynamicApi定義したクラスのActionを呼び出す
        /// これによってrestful APIの呼び出しを実行する
        /// </summary>
        /// <param name="target">リソースクラスのインスタンス</param>
        /// <param name="dao">DynamicAPI定義のクラスやアクションなどの情報</param>
        /// <param name="noConvert"></param>
        /// <param name="paramter"></param>
        /// <returns></returns>
        public static WebApiResponseResult CallAction(object target, RepositoryDaoInfo dao, params object[] paramter)
        {
            if (dao.ActionMethod == null)
            {
                return new WebApiResponseResult(new Exception("ActionName is null"));
            }

            // ICommonCrudRepositoryを継承（実装）しているクラスのメソッド属性の指定を反映する
            var stackframe = CallStack.FindStackFrame<ICommonCrudRepository>();

            // リターンの型がList<xxx>ならIsArrayをセットする
            var method = stackframe.GetMethod();
            var webApiResponseResultType = method.GetPropertyValue<object>("ReturnParameter").GetPropertyValue<Type>("ParameterType");
            var inGenericType = webApiResponseResultType.IsGenericParameter ? webApiResponseResultType.GenericTypeArguments[0] : null;
            var interfaces = inGenericType?.GetTypeInfo().ImplementedInterfaces;
            if (interfaces?.Where(x => x == typeof(IList)).Count() == 1)
            {
                dao.IsArray = true;
            }

            // 属性によってDao情報の変換を行う
            if (method.GetCustomAttribute<CrudRepositoryModelAttribute>() is CrudRepositoryModelAttribute modelattr && modelattr.Type != null)
            {
                dao.ModelType = modelattr.Type;
            }
            if (method.GetCustomAttribute<CrudRepositoryArrayAttribute>() is CrudRepositoryArrayAttribute arrayattr)
            {
                dao.IsArray = arrayattr.IsArray;
            }
            bool noConvert = method.GetCustomAttribute<CrudRepositoryResultNoConvertAttribute>() != null;

            // Resourceクラス（DynamicAPIを定義したリソースクラス）とActionメソッド（API）から、WebAPIを呼び出すためのRequestModelを作成する
            var requestModel = dao.ActionMethod.Invoke(target, paramter) as WebApiRequestModel;
            if (requestModel == null)
            {
                return new WebApiResponseResult(new Exception("I executed action but the return type is different from the expected value."));
            }

            // ヘッダーを追加
            if (dao.Headers?.Any() == true)
            {
                requestModel.Header = requestModel.Header.Merge(dao.Headers);
            }
            else
            {
                requestModel.Header = new Dictionary<string, string[]>();
            }

            // ARRAY時の変換
            Type resultModel = dao.ModelType;
            if (resultModel != null && dao.IsArray == true)
            {
                resultModel = typeof(List<>).MakeGenericType(new Type[] { dao.ModelType });
            }

            // restfull APIを呼び出す
            bool isExistsOpenIdToken = false;
            IDynamicApiClient client = null;
            if (dao.DynamicApiClientSelector != null)
            {
                client = UnityCore.Resolve<IDynamicApiClient>((UnityCore.Resolve(dao.DynamicApiClientSelector) as IDynamicApiClientSelector)?.Name);
                if (dao.DynamicApiClientSelector == typeof(ILoginUser))
                {
                    requestModel.Header.Add("Authorization", new string[] { UnityCore.Resolve<IOAuthContext>().GetOpenIdToken() });
                    isExistsOpenIdToken = true;
                }
            }
            if (isExistsOpenIdToken == false && dao?.Param != null)
            {
                if (dao?.Param?.ContainsKey(Const.APIPARAM_OPENID_TOKEN) == true)
                {
                    var oid = (string)dao.Param[Const.APIPARAM_OPENID_TOKEN];
                    requestModel.Header.Add("Authorization", new string[] { oid });
                }
            }

            // DynamicApiClientが見つからない場合は
            if (client == null)
            {
                client = new DynamicApiClient(UnityCore.Resolve<IServerEnvironment>(), UnityCore.Resolve<IAuthenticationResult>());
            }
            var response = client.Request(requestModel);

            if (response.IsSuccessStatusCode == false && response.StatusCode != System.Net.HttpStatusCode.NotFound || noConvert == true)
            {
                return response.ToWebApiResponseResult();
            }
            return response.ToWebApiResponseResult(resultModel);
        }
    }
}
