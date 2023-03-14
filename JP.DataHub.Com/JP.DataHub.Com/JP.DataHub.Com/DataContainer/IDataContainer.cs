using JP.DataHub.Com.TimeZone;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.DataContainer
{
    public interface IDataContainer
    {
        string Id { get; set; }
        string VendorId { get; set; }
        string SystemId { get; set; }
        string OpenId { get; set; }
        bool VendorSystemAuthenticated { get; set;  }
        Dictionary<string, string> Claims { get; set; }

        string AuthorizationError { get; set; }
        bool IsDeveloper { get; set; }
        string ClientIpAddress { get; set; }
        string OriginalAccessToken { get; set; }

        /// <summary>
        /// 表示言語
        /// </summary>
        CultureInfo CultureInfo { get; set; }
        
        DateTimeUtil GetDateTimeUtil();

        bool ProfilerDisabled { get; set; }

        string ControllerName { get; set; }
        string ActionName { get; set; }
        object Argument { get; set; }
    }
}
