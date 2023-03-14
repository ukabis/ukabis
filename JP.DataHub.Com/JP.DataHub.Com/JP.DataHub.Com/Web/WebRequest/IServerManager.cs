using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.WebRequest
{
    public interface IServerManager : IServerList
    {
        List<Server> List { get; set; }
        IServer Find(string serverName);
        IServerEnvironment Find(string serverName, string environmentName);
        IServerEnvironment FindOnce();
    }
}
