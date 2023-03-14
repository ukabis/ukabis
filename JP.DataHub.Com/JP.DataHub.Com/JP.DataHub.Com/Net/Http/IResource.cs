using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Net.Http
{
    public interface IResource
    {
        Type ModelType { get; set; }

        IServerEnvironment ServerEnvironment { get; }
        String ServerUrl { get; }
        string ResourceUrl { get; set; }

        IDictionary<string,string[]> AddHeaders { get; set; }
    }
}
