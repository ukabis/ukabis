using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JP.DataHub.Com.Authentication
{
    public class Jwt
    {
        private Encoding enc = Encoding.GetEncoding("UTF-8");

        public string HeaderString { get; internal set; }
        public string PayloadString { get; internal set; }
        public string SignatureString { get; internal set; }

        public string HeaderJson { get; internal set; }
        public string PayloadJson { get; internal set; }

        public string oid { get; internal set; }

        public long exp { get; internal set; }

        public Jwt(string str)
        {
            Parse(str);
        }

        public void Parse(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str?.Replace("Bearer ", "");
                string[] split = str.Split('.');
                if (split.Length == 3)
                {
                    HeaderString = split[0];
                    PayloadString = split[1];
                    SignatureString = split[2];
                    PayloadJson = Base64UrlDecode(PayloadString);
                    HeaderJson = Base64UrlDecode(HeaderString);
                    var openIdPayload = JsonConvert.DeserializeObject<OpenIdPayload>(PayloadJson);
                    oid = openIdPayload.oid;
                    exp = openIdPayload.exp;
                }
            }
        }

        private string Base64UrlDecode(string str)
        {
            str = str.Replace("-", "+");
            str = str.Replace("_", "/");
            int padding = (str.Length % 4 == 0) ? 0 : 4 - str.Length % 4;
            Enumerable.Range(0, padding).ToList().ForEach(x => str += "=");
            return Base64Decode(str);
        }

        private string Base64Decode(string str)
        {
            var bytes = Convert.FromBase64String(str);
            return enc.GetString(bytes);
        }
    }

    public static class JwtExtensions
    {
        public static Jwt ParseJwt(this string str)
            => new Jwt(str);

        public static bool IsExpired(this Jwt jwt)
           => jwt.exp == 0 || jwt.exp < DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
    }
}
