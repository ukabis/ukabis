using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Instance;

namespace JP.DataHub.Com.Web.WebRequest
{
    public class ServerList : IServerList, IInstanceInitializer
    {
        public List<Server> List { get; set; }

        public IServer Find(string serverName) => List?.FirstOrDefault(x => x.ServerName == serverName);
        public IServerEnvironment Find(string serverName, string environmentName) => List?.FirstOrDefault(x => x.ServerName == serverName)?.Find(environmentName);
        public IServerEnvironment FindOnce() => List.FindOnce<Server>().EnvironmentList.FindOnce<ServerEnvironment>();
        public bool IsOnce() => List.Count() == 1 && List.FirstOrDefault()?.EnvironmentList?.Count() == 1;

        public void InstanceInitializer()
        {
            Correct();
        }

        internal ServerList Correct()
        {
            List?.ForEach(src => src?.EnvironmentList?.ForEach(e => e.Parent = src));
            return this;
        }

        internal ServerList Correct(ServerList other)
        {
            var regReference = new Regex("^\\$ReferenceServer\\((?<name>.*?)\\)$");

            Correct();
            var swap = new Dictionary<Server, Server>();
            foreach (var server in List)
            {
                var match = regReference.Match(server.ServerName);
                if (match.Success == true)
                {
                    var name = match.Groups["name"].Value;
                    var hit = other.List.Where(x => x.ServerName == name).FirstOrDefault();
                    if (hit != null)
                    {
                        swap.Add(server, hit);
                    }
                }
            }
            foreach (var item in swap)
            {
                int pos = List.IndexOf(item.Key);
                if (pos != -1)
                {
                    List[pos] = item.Value;
                }
            }
            return this;
        }
    }
}
