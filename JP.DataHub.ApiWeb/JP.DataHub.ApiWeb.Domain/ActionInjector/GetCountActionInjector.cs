using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Log;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal class GetCountActionInjector : ActionInjector
    {
        private JPDataHubLogger _log = new JPDataHubLogger(typeof(GetCountActionInjector));

        public override void Execute(Action action)
        {
            // 基底の処理を呼び出す
            action();

            // 成功していれば、２行になっている（かも）jsonのデータを１行にする
            HttpResponseMessage response = ReturnValue as HttpResponseMessage;
            if (response != null)
            {
                if (response.IsSuccessStatusCode == true)
                {
                    string str = response.Content.ReadAsStringAsync().Result;
                    _log.Debug(str);
                    var tempobj = Newtonsoft.Json.JsonConvert.DeserializeObject(str, new Newtonsoft.Json.JsonSerializerSettings { FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Decimal });
                    JToken json = JToken.FromObject(tempobj);
                    long sum = json.Sum(x => x.ToObject<long>());
                    JToken getcount = JToken.FromObject($"{{ \"Count\" : {sum} }}");
                    string MEDIATYPE_JSON = "application/json";
                    ReturnValue = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(getcount.ToString(), Encoding.UTF8, MEDIATYPE_JSON) };
                }
            }
        }
    }
}
