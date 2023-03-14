using JP.DataHub.Aop;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.Com.Cryptography;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using JP.DataHub.Com.Extensions;
using System.Collections.Generic;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class TraceManageRegisterFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            if (param.ContentsStream == null)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
            }

            // RequestBody取出し(RegisterListを考慮し配列で取得)
            List<TraceManageModel> model;
            try
            {
                using (var reader = new StreamReader(param.ContentsStream, Encoding.UTF8, true, 1024, true))
                {
                    var content = reader.ReadToEnd();
                    var contentJson = content.ToJson();
                    model = contentJson.Type == Newtonsoft.Json.Linq.JTokenType.Array ? JsonConvert.DeserializeObject<List<TraceManageModel>>(content) : JsonConvert.DeserializeObject<List<TraceManageModel>>(contentJson.ToJsonArray().ToString());
                    param.ContentsStream.Position = 0;
                }
                if (model == null)
                {
                    return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
                }
            }
            catch (JsonException)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
            }

            foreach(var m in model)
            {
                //リソース全体を検索して、指定のProductCodeにパスワードが既に設定されているかチェックする
                var ret = param.ApiHelper.ExecuteGetApi($"{param.ResourceUrl}/ODataOtherAccessible?$filter=ProductCode eq '{m.ProductCode}'");
                if (!ret.IsSuccessStatusCode && ret.StatusCode != HttpStatusCode.NotFound)
                    return ret;

                if (ret.StatusCode != HttpStatusCode.NotFound)
                {
                    //既に設定されてるなら、Registerさせない
                    return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104405);
                }
                //パスワードのハッシュ値に変えてRegisterInternalする
                m.Password = PasswordHashArgorithm.GetHashedPassword(m.Password, null);
            }

            return param.ApiHelper.ExecutePostApi($"{param.ResourceUrl}/RegisterListInternal", JsonConvert.SerializeObject(model));
        }
    }
}
