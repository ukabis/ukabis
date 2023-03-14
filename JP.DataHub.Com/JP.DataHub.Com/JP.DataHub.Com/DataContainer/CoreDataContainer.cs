using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.TimeZone;

namespace JP.DataHub.Com.DataContainer
{
    public class CoreDataContainer : IDataContainer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public string OpenId { get; set; }

        public bool VendorSystemAuthenticated { get; set; }

        public Dictionary<string, string> Claims { get; set; }
        public string AuthorizationError { get; set; }
        public bool IsDeveloper { get; set; }
        public string ClientIpAddress { get; set; }
        public bool IsInternalCall { get; set; }

        public string OriginalAccessToken { get; set; }
        /// <summary>
        /// 表示言語
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        public DateTimeUtil GetDateTimeUtil() => dateTimeUtil.Value;

        private Lazy<DateTimeUtil> dateTimeUtil = new Lazy<DateTimeUtil>(() => new DateTimeUtil("yyyy/MM/dd",
            new string[] { "yyyy/MM/dd hh:mm:ss tt", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd h:mm:ss" }, "yyyy/M/d"));

        public bool ProfilerDisabled { get; set; }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public object Argument { get; set; }
    }
}
