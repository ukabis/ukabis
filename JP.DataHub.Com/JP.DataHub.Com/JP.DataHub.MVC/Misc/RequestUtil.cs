using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using JP.DataHub.MVC.Misc;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Settings;
using JP.DataHub.MVC.Http;

namespace JP.DataHub.MVC.Misc
{
    public static class RequestUtil
    {
        private static Lazy<IHttpContextAccessor> _httpContextAccessor = new Lazy<IHttpContextAccessor>(() => UnityCore.Resolve<IHttpContextAccessor>());
        private static IHttpContextAccessor httpContextAccessor => _httpContextAccessor.Value;

        private static Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        private static IConfiguration configuration => _configuration.Value;

        /// <summary>
        /// ClientIpを取得する。HTTP_X_FORWARDED_FORが設定されている場合はそれを採用し、なければUserHostAddressを採用する。
        /// </summary>
        /// <returns>ClientのIP</returns>
        public static string GetRequestIP() => GetRequestIP(CanUseXFFAddresses());

        /// <summary>
        /// ClientIpを取得する。HTTP_X_FORWARDED_FORが設定されている場合はそれを採用し、なければUserHostAddressを採用する。
        /// </summary>
        /// <returns>ClientのIP</returns>
        public static string GetRequestIP(bool tryUseXForwardHeader = true)
        {
            string ip = null;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
                ip = RequestHeaderUtil.GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault();

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (ip.IsNullOrWhiteSpace() && httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
                ip = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhiteSpace())
                ip = RequestHeaderUtil.GetHeaderValueAs<string>("REMOTE_ADDR");

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local Host.

            //if (ip.IsNullOrWhiteSpace())
            //    throw new Exception("Unable to determine caller's IP.");

            return ip;
        }

        private static bool CanUseXFFAddresses()
        {
            try
            {
                return (bool)UnityCore.Resolve<IpAddressSettings>()?.XFFIPAddressesCouldBeRegardedAsRepresentiveClientIP;
            }
            catch
            {
                return false;
            }
        }
    }
}
