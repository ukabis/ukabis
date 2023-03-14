using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class CertificationPortalImpersonateAsApplicantFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // URLから申請ID取得
            param.QueryStringDic
                .ThrowMessage(x => !x.ContainsKey("CertificationApplyId"), x => param.MakeRfc7807Response(ErrorCodeMessage.Code.E107401));
            var certificationApplyId = param.QueryStringDic.GetOrDefault("CertificationApplyId");

            // 申請を取得(呼び出し元の認証機関の認証以外の申請が指定された場合はNotFoundとなる)
            var url = $"/API/CertificationPortal/Private/CertificationApply/GetApplicantOpenId/{certificationApplyId}";
            var result = param.ApiHelper.ExecuteGetApi(url);
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E107402);
            }
            else if (!result.IsSuccessStatusCode)
            {
                Logger.Error($"Failed at url. Response: {result.Content.ReadAsStringAsync().Result}");
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E107501);
            }

            // 申請者のOpenIdに差し替え
            var apply = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            var applicantOpenId = apply["OwnerId"].Value<string>();
            param.OpenId = applicantOpenId;

            return null;
        }
    }
}