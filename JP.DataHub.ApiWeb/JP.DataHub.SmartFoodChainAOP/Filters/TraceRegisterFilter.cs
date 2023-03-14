using JP.DataHub.Aop;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using JP.DataHub.Com.Extensions;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Web.Authentication;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class TraceRegisterFilter : AbstractApiFilter
    {
        private readonly string TraceManageUrl = "/API/Traceability/V3/Private/TraceManage";

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            if (param.ContentsStream == null)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
            }

            //RegisterListも考慮してListで取得
            List<TraceModel> requestModels;
            try
            {
                using (var reader = new StreamReader(param.ContentsStream, Encoding.UTF8, true, 1024, true))
                {
                    var content = reader.ReadToEnd();
                    var contentJson = content.ToJson();
                    requestModels = contentJson.Type == Newtonsoft.Json.Linq.JTokenType.Array ? JsonConvert.DeserializeObject<List<TraceModel>>(content) : JsonConvert.DeserializeObject<List<TraceModel>>(contentJson.ToJsonArray().ToString());
                    param.ContentsStream.Position = 0;
                }
                if (requestModels == null)
                {
                    return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
                }
            }
            catch (JsonException)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E100405);
            }

            //QueryStringからpassword取得
            var password = param.QueryStringDic?.GetOrDefault("password");

            foreach(var model in requestModels)
            {
                //該当のProductCodeにパスワードが設定されているか確認
                var ret = param.ApiHelper.ExecuteGetApi($"{TraceManageUrl}/PasswordVerification?ProductCode={model.ProductCode}&Password={password}");
                if (!ret.IsSuccessStatusCode && ret.StatusCode != HttpStatusCode.NotFound)
                    return ret;
                else
                {
                    //パスワード設定があり、且つリクエストと合致
                    if (ret.StatusCode == HttpStatusCode.OK)
                    {
                        //Nothing to do.
                    }
                    else
                    {
                        var responce = ret.Content.ReadAsStringAsync().Result.ToJson();
                        //指定ProductCodeにパスワード設定が無い
                        if (ret.StatusCode == HttpStatusCode.NotFound && responce["error_code"].ToString() == "E104409")
                            if (!string.IsNullOrEmpty(password))
                                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104404);
                            else
                            {
                                //Nothing to do.
                            }
                        //パスワードアンマッチ
                        else if (ret.StatusCode == HttpStatusCode.BadRequest && responce["error_code"].ToString() == "E104404")
                        {
                            return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104404);
                        }
                        //上記以外（システムエラーなど）
                        else
                            return ret;
                    }
                }
            }
            return null;
        }
    }
}
