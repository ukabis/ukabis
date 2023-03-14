using System.Net;
using JP.DataHub.Aop;
using JP.DataHub.Com.Log;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class CompanyCertifiedRegisterCertifiedApplicationFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // RequestBody取出し
            CompanyCertifiedModel model;
            try
            {
                var stream = new MemoryStream();
                param.ContentsStream.Position = 0;
                param.ContentsStream.CopyTo(stream);
                param.ContentsStream.Position = 0;
                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    model = JsonConvert.DeserializeObject<CompanyCertifiedModel>(reader.ReadToEnd());
                }
            }
            catch (JsonException)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E103400);
            }

            // Companyを取得
            var url = $"/API/CompanyMaster/V3/Private/Company/ODataOtherAccessible?$filter=CompanyId eq '{model.CompanyId}'";
            var getResult = param.ApiHelper.ExecuteGetApi(url);
            if (getResult.StatusCode == HttpStatusCode.NotFound)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E107403);
            }
            else if (!getResult.IsSuccessStatusCode)
            {
                Logger.Error($"Failed at url. Response: {getResult.Content.ReadAsStringAsync().Result}");
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E107502);
            }

            // CompanyのOpenIdに差し替え
            var company = JToken.Parse(getResult.Content.ReadAsStringAsync().Result);
            var companyOpenId = company.Single()["_Owner_Id"].Value<string>();
            param.OpenId = companyOpenId;

            return null;
        }
    }
}