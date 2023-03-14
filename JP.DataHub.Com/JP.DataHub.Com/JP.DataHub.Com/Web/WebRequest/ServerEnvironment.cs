using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.Authentication;

namespace JP.DataHub.Com.Web.WebRequest
{
    public interface IServerEnvironment
    {
        string EnvironmentId { get; set; }
        string EnvironmentName { get; set; }

        string Url { get; set; }
        string Url2 { get; set; }
        string DynamicApiUrl { get; set; }

        Dictionary<AuthenticationServerType, IAuthenticationServerInfo> ServerAuthenticationList { get; set; }

        Server Parent { get; set; }

        IAuthenticationInfo GetAuthenticationInfo();
        void AddHeader(HttpRequestHeaders headers);
    }

    public class ServerEnvironment : IServerEnvironment
    {
        public string EnvironmentId { get; set; }
        public string EnvironmentName { get; set; }

        public string Url { get; set; }
        public string Url2 { get; set; }

        public string DynamicApiUrl { get; set; }

        public Dictionary<AuthenticationServerType, IAuthenticationServerInfo> ServerAuthenticationList { get; set; }

        public Server Parent { get; set; }

        public IAuthenticationInfo GetAuthenticationInfo()
            => AuthenticationInfoFactory.Create(this);

        public void AddHeader(HttpRequestHeaders headers)
            => ServerAuthenticationList.Values.ToList().ForEach(x => x.AddHeader(headers));
    }
}
