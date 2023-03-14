using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    internal static class HttpContentEx
    {
        public static T DeserializeObject<T>(this HttpContent content) where T : class
        {
            if (content is StringContent)
            {
                return JsonConvert.DeserializeObject<T>(content.ReadAsStringAsync().Result);
            }
            else if (content is StreamContent)
            {
                var stream = content.ReadAsStreamAsync().Result;
                if (stream.Length == 0) return null;

                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }

            return null;
        }

        public static object DeserializeObject(this HttpContent content)
        {
            if (content is StringContent)
            {
                return JObject.Parse(content.ReadAsStringAsync().Result);

            }
            else if (content is StreamContent)
            {
                var stream = content.ReadAsStreamAsync().Result;
                if (stream.Length == 0) return null;

                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize(jsonTextReader);
                }
            }

            return null;
        }
    }
}
