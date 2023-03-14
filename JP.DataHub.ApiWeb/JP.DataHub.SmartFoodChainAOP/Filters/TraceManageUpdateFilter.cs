using JP.DataHub.Aop;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.Com.Cryptography;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class TraceManageUpdateFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            if (param.ContentsStream == null)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
            }

            // RequestBody取出し
            TraceManagePasswordModel model;
            try
            {
                using (var reader = new StreamReader(param.ContentsStream, Encoding.UTF8, true, 1024, true))
                {
                    model = JsonConvert.DeserializeObject<TraceManagePasswordModel>(reader.ReadToEnd());
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

            var productCode = param.QueryStringDic?.GetOrDefault("ProductCode");
            if (string.IsNullOrEmpty(productCode))
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104406);

            //全体を検索して、指定のProductCodeにパスワードが既に設定されているかチェックする
            var ret = param.ApiHelper.ExecuteGetApi($"{param.ResourceUrl}/ODataOtherAccessible?$filter=ProductCode eq '{productCode}'");
            if (!ret.IsSuccessStatusCode && ret.StatusCode != HttpStatusCode.NotFound)
                return ret;

            if (ret.StatusCode != HttpStatusCode.NotFound)
            {
                var tracemng = JsonConvert.DeserializeObject<List<TraceManageAdditionalOwenerIdModel>>(ret.Content.ReadAsStringAsync().Result).FirstOrDefault();

                //他人のなら、更新不可
                if(tracemng._Owner_Id != param.OpenId)
                    return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104408);

                //データがあるならX-Password指定は必須
                if (!param.Headers.ContainsKey("X-Password"))
                    return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104407);

                var pwd = param.Headers.GetValueOrDefault("X-Password")?.FirstOrDefault();
                //ハッシュ値取得
                var pwdHashed = PasswordHashArgorithm.GetHashedPassword(pwd, null);

                //チェック
                if (pwdHashed != tracemng.Password)
                    return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104404);

                //チェックが通ったら、指定されているパスワードをハッシュ値に変えて、UpdateInternalする
                model.Password = PasswordHashArgorithm.GetHashedPassword(model.Password, null);

                return param.ApiHelper.ExecutePatchApi($"{param.ResourceUrl}/UpdateInternal/{productCode}", JsonConvert.SerializeObject(model));
            }
            else
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104409);
        }
    }
}
