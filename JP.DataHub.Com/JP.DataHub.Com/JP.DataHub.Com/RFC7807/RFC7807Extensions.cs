using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.Resources;

namespace JP.DataHub.Com.RFC7807
{
    public static class RFC7807Extensions
    {
        /// <summary>
        /// Deserializeに使用している設定 これをセットしないと json文字列ならなんでもdeserializeOKになる
        /// </summary>
        public static readonly JsonSerializerSettings JsonSerializerSetting = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error };

        public static RFC7807ProblemDetail ToRFC7807ProblemDetail(this string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<RFC7807ProblemDetail>(str, JsonSerializerSetting);
            }
            catch
            {
                return null;
            }
        }
    }
}
