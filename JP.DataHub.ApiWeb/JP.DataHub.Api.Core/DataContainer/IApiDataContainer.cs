using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DataContainer;

namespace JP.DataHub.Api.Core.DataContainer
{
    public interface IApiDataContainer : IDataContainer
    {
        DateTime Time { get; set; }
        string ControllerId { get; set; }
        string ActionId { get; set; }
        ConcurrentDictionary<string, string> LoggingIdUrlList { get; set; }
        Func<object,bool> VendorCheckFunc { get; set; }
        string RequestUriAuthority { get; set; }
        string RequestUriScheme { get; set; }
        bool IsStaticApi { get; set; }

        bool VendorCheck(object obj);
    }
}
