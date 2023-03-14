using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Instance;

namespace JP.DataHub.Com.Web.WebRequest
{
    public class ServerManager : ServerList, IServerManager,  IInstanceInitializer
    {
        public static string ServerJsonFileName { get; set; } = "server.json";

        public ServerManager(string fileName = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                List = fileName.FileToJson<List<Server>>();
                base.Correct();
            }
        }

        public new void InstanceInitializer()
        {
            Correct();
        }
    }
}
