using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class ApplicationManagerFilter : AbstractApiFilter
    {
        private static readonly string CacheKeyPrefix = "ApplicationManagerFilter.ManagedApplicationsCache";
        private static readonly string SourceUrl = "/API/ApplicationAuthorization/Public/Application/GetManagedApplications/";


        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            if (string.IsNullOrEmpty(param.OpenId))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E101407, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            var applications = CacheHelper.GetOrAdd<List<ApplicationIdModel>>($"{CacheKeyPrefix}.{param.OpenId}", () =>
                param.ApiHelper.ExecuteGetApi($"{SourceUrl}{param.OpenId}")
                    .ToWebApiResponseResult<List<ApplicationIdModel>>()
                    .ThrowRfc7807()
                    .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E101408))
                    .ThrowMessage(x => x.IsSuccessStatusCode == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E101501))
                    .Result);

            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(applications), Encoding.UTF8, "application/json") };
        }
    }
}
