using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class VendorSystemFilter : AbstractApiFilter
    {
        // アプリケーションのキャッシュキーの接頭辞
        private static readonly string ApplicationCacheKeyPrefix = "VendorSystemFilter_ApplicationCache";

        // アプリケーション取得APIのURL
        private static readonly string ApplicationUrl = "/API/ApplicationAuthorization/Public/Application/GetApplication";

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // 指定されたアプリケーションのベンダーIDとシステムIDに差し替える
            // リクエストユーザが、管理画面のAdminロールと対象アプリケーションの何らかのロールを持っている必要がある
            if (param.QueryStringDic.IsNullOrEmpty("applicationId"))
            {
                // アプリケーションID未指定なら何もしない
                return null;
            }

            // アプリケーションIDと管理者OpenIDを指定してアプリケーションを取得
            var applicationId = param.QueryStringDic.GetOrDefault("applicationId");
            var app = CacheHelper.GetOrAdd<ApplicationModel>($"{ApplicationCacheKeyPrefix}.{applicationId}.{param.OpenId}", () =>
                            param.ApiHelper.ExecuteGetApi($"{ApplicationUrl}?ApplicationId={applicationId}&OpenId={param.OpenId}")
                                .ToWebApiResponseResult<ApplicationModel>()
                                .ThrowRfc7807()
                                .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m =>
                                {
                                    // 該当するアプリケーションがなければエラー
                                    Logger.Error($"Application[{applicationId}] does not exists, or User[{param.OpenId}] is not Manager of the application.");
                                    return param.MakeRfc7807Response(ErrorCodeMessage.Code.E101406);
                                })
                                .Result);

            // ベンダーIDとシステムIDを差し替え
            param.ImpersonateRequestVendorId = app.VendorId;
            param.ImpersonateRequestSystemId = app.SystemId;

            return null;
        }
                
    }
}
