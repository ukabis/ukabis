using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JP.DataHub.Com.Web.Authentication
{
    [JsonObject]
    public class TokenInfo
    {
        [JsonProperty]
        public string access_token { get; set; }
        [JsonProperty]
        public string token_type { get; set; }
        [JsonProperty]
        public string expires_in { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string refresh_token { get; set; }
        private DateTime? AcquisitionDateTime { get; } = DateTime.Now;

        public bool IsExpire()
        {
            if (AcquisitionDateTime == null)
            {
                return true;
            }

            var expiresSeconds = int.Parse(expires_in);
            var expiresTime = AcquisitionDateTime + TimeSpan.FromSeconds(expiresSeconds) - new TimeSpan(0, 1, 0);//1分のゆとりを持たせる
            return expiresTime < DateTime.Now;
        }
    }
}
