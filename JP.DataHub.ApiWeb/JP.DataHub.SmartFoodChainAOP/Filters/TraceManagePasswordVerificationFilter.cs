using JP.DataHub.Aop;
using JP.DataHub.Com.Cryptography;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Models;
using Newtonsoft.Json;
using System.Net;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class TraceManagePasswordVerificationFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            var productCode = param.QueryStringDic.GetValueOrDefault("ProductCode");
            var password = param.QueryStringDic.GetValueOrDefault("Password");

            //ProductCodeは必須(passwordはモデル上必須属性でなく、nullや空値も考慮するため、パスワード照合まで処理を流す）
            if (string.IsNullOrEmpty(productCode))
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104400);

            var ret = param.ApiHelper.ExecuteGetApi($"{param.ResourceUrl}/ODataOtherAccessible?$filter=ProductCode eq '{productCode}'");
            if (!ret.IsSuccessStatusCode && ret.StatusCode != HttpStatusCode.NotFound)
                return ret;

            if (ret.StatusCode == HttpStatusCode.NotFound)
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104409);
            else
            {
                //ProductCodeが存在したら、リクエストのパスワードのハッシュ値と、保存データのハッシュ値を照合する
                var pwdHashed = PasswordHashArgorithm.GetHashedPassword(password, null);
                var traceMng = JsonConvert.DeserializeObject<List<TraceManageModel>>(ret.Content.ReadAsStringAsync().Result).FirstOrDefault();

                if (pwdHashed == traceMng.Password)
                    return new HttpResponseMessage(HttpStatusCode.OK);
                else
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104404);
            }
        }
    }
}
