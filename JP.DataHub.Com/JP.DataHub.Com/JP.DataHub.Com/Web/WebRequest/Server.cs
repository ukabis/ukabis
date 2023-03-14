using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Instance;

namespace JP.DataHub.Com.Web.WebRequest
{
    public interface IServer
    {
        string ServerName { get; set; }
        ServerType Type { get; set; }
        bool Enable { get; set; }
        AuthenticationType AuthenticationType { get; set; }
        List<ServerEnvironment> EnvironmentList { get; set; }

        IServerEnvironment Find(string environmentName);

        void InstanceInitializer();
    }

    public class Server : IServer, IInstanceInitializer
    {
        public string ServerName { get; set; }
        public ServerType Type { get; set; }
        public bool Enable { get; set; } = true;
        public AuthenticationType AuthenticationType { get; set; }
        public List<ServerEnvironment> EnvironmentList { get; set; }

        public IServerEnvironment Find(string environmentName) => EnvironmentList?.FirstOrDefault(x => x.EnvironmentName == environmentName);

        public void InstanceInitializer()
        {
            EnvironmentList.ForEach(x => x.Parent = this);
        }
    }
}
