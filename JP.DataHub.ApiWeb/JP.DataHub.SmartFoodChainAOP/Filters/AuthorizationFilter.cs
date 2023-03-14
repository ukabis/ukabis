using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class AuthorizationFilter : AbstractApiFilter
    {
        // グループのキャッシュキーの接頭辞
        private static readonly string ApplicationCacheKeyPrefix = "AuthorizationFilter.ApplicationCache";
        private static readonly string AuthorizeUserCacheKeyPrefix = "AuthorizationFilter.AuthorizeUserCache";
        private static readonly string RoleCacheKeyPrefix = "AuthorizationFilter.RoleCache";

        // アプリケーションが存在しているか？
        private static readonly string GetApplicationUrl = "/API/ApplicationAuthorization/Public/Application/Get/{app}";
        private static readonly string AuthorizeUserUrl = "/API/ApplicationAuthorization/Private/AuthorizeUser";
        private static readonly string RoleUrl = "/API/ApplicationAuthorization/Private/Role";

        private static readonly string QUERYSTRING_APP = "app";

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            var urlparam = param.QueryStringDic.Validate<Guid>(QUERYSTRING_APP, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E101400/*ApplicationIdが指定されていません*/));
            var applicationId = param.QueryStringDic.GetOrDefault(QUERYSTRING_APP);

            Func<ApplicationModel> getApplication = () =>
                CacheHelper.GetOrAdd<ApplicationModel>($"{ApplicationCacheKeyPrefix}.{applicationId}", () =>
                        param.ApiHelper.ExecuteGetApi(urlparam.CollectUrl(GetApplicationUrl))
                        .ToWebApiResponseResult<ApplicationModel>()
                        .ThrowRfc7807()
                        .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E101401/*アプリケーションが存在しません*/))
                        .ThrowMessage(x => x.IsSuccessStatusCode == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E101402/*Applicationリソースの取得に失敗しました*/))
                        .Result);
            Func<List<ApplicationUserModel>> getApplicationUser = () =>
                CacheHelper.GetOrAdd<List<ApplicationUserModel>>($"{AuthorizeUserCacheKeyPrefix}.{param.OpenId}.{applicationId}", () =>
                        param.ApiHelper.ExecuteGetApi($"{AuthorizeUserUrl}/ODataOverPartition?$filter=OpenId eq '{param.OpenId}' and ApplicationId eq '{applicationId}'")
                        .ToWebApiResponseResult<List<ApplicationUserModel>>()
                        .ThrowRfc7807()
                        .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E101403/*認可されていません"*/))
                        .ThrowMessage(x => x.IsSuccessStatusCode == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E101404/*ApplicationUserリソースの取得に失敗しました*/))
                        .Result);
            Func<List<RoleModel>> getRole = () =>
                CacheHelper.GetOrAdd<List<RoleModel>>($"{RoleCacheKeyPrefix}.{applicationId}", () =>
                    param.ApiHelper.ExecuteGetApi($"{RoleUrl}/ODataOverPartition?$filter=ApplicationId eq '{applicationId}'")
                        .ToWebApiResponseResult<List<RoleModel>>()
                        .ThrowRfc7807()
                        .ThrowMessage(x => x.IsSuccessStatusCode == false && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E101405/*Roleリソースの取得に失敗しました*/))
                        .Result);

#if (true)     // SYNC
            // ApplicationIdが存在し有効か？
            var application = getApplication();
            // ApplicationUserを取得
            var appuserlist = getApplicationUser();
            // Applicationに定義されているRoleを取得
            var rolelist = getRole();
#else           // ASYNC
            // ApplicationIdが存在し有効か？
            var task1 = Task.Run<ApplicationModel>(() => getApplication());
            // ApplicationUserを取得
            var task2 = Task.Run<ApplicationUserModel>(() => getApplicationUser());
            // Applicationに定義されているRoleを取得
            var task3 = Task.Run<List<RoleModel>>(() => getRole());
            Task.WaitAll(task1, task2, task3);
            var application = task1.Result;
            var appuserlist = task2.Result;
            var rolelist = task3.Result;
#endif
            var appuser = appuserlist?.Any() == true ? appuserlist[0] : null;

            // 自分に直接許可されている機能、自分が持っているロールに対する機能をマージする
            var functionlist = appuser?.Functions ?? new List<string>();
            rolelist?.Where(x => x.Functions?.Count > 0 && appuser?.PrivateRoleId?.Contains(x.PrivateRoleId) == true).ToList().ForEach(x => functionlist.AddRange(x.Functions));
            var resultmodel = new IsAuthorizationResult() { OpenId = param.OpenId, ApplicationId = applicationId, ApplicationName = application.ApplicationName, Result = application.IsEnable, FunctionList = application.IsEnable == true ? functionlist.Distinct().ToList() : null };
            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(resultmodel), Encoding.UTF8, "application/json") };
        }
    }
}
