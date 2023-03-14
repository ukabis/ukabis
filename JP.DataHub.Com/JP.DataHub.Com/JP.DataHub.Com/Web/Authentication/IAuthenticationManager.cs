using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Reactive.Bindings;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public interface IAuthenticationManager : IEnumerable<IAuthenticationInfo>
    {
        string FileName { get; set; }

        //ReactiveProperty<(ServerEnvironment Enviroment, IAuthenticationInfo AuthenticationInfo, IAuthenticationResult AuthenticationResult)> ChangeStatus { get; }

        void Load(ServerList list);
        void Save();
        void Add(IAuthenticationInfo authenticationInfo);
        IAuthenticationInfo Find(string authenticationId);
        void Remove(string authenticationId);
        void ChangeResult(IAuthenticationInfo authenticationInfo, IAuthenticationResult authenticationResult);
        IEnumerable<string> AccountList();
    }
}
