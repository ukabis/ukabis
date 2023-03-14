using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DataContainer;

namespace JP.DataHub.Api.Core.DataContainer
{
    public class ApiDataContainer : CoreDataContainer, IApiDataContainer
    {
        public DateTime Time { get; set; }
        public string ControllerId { get; set; }
        public string ActionId { get; set; }
        public ConcurrentDictionary<string, string> LoggingIdUrlList { get; set; } = new ConcurrentDictionary<string, string>();
        public Func<object, bool> VendorCheckFunc { get; set; }
        public string RequestUriAuthority { get; set; }
        public string RequestUriScheme { get; set; }
        public bool IsStaticApi { get; set; } = false;

        public bool VendorCheck(object obj)
        {
            if (VendorCheckFunc != null)
            {
                return VendorCheckFunc(obj);
            }
            return true;
        }
    }
}
