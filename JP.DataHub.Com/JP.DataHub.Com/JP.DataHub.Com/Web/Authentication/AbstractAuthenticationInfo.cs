using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public abstract class AbstractAuthenticationInfo : IAuthenticationInfo
    {
        public string Type { get; set; }

        public string AuthenticationInfoId { get; set; }

        [JsonIgnore]
        public bool HasAuthenticationResult { get => AuthenticationResult != null; }

        private IAuthenticationResult _auth = null;

        [JsonIgnore]
        public ServerEnvironment Environment { get; set; }

        [JsonIgnore]
        public IAuthenticationResult AuthenticationResult
        {
            get => _auth;
            set { _auth = value; /*通知しないため：UnityCore.Resolve<IAuthenticationManager>().ChangeAuthenticationResult(this, _auth);*/ }
        }

        public void ClearAuthenticationResult()
        {
            AuthenticationResult = null;
        }

        public override string ToString()
        {
            return null;
        }

        /// <summary>
        /// ２つの認証情報をマージする
        /// ※本当はCombinationAuthenticationInfoを定義して、２つの認証情報を内包する方がよい。が出来ていない。この実装は暫定
        /// </summary>
        /// <param name="anotherAauthenticationInfo"></param>
        /// <returns></returns>
        public IAuthenticationInfo Merge(params IAuthenticationInfo[] anotherAauthenticationInfo)
        {
            var list = new List<IAuthenticationInfo>(){ this};
            foreach (var a in anotherAauthenticationInfo)
            {
                list.Add(a);
            }
            return AbstractAuthenticationInfo.MergeAuthenticationInfo(list.ToArray());
        }

        internal static IAuthenticationInfo MergeAuthenticationInfo(params IAuthenticationInfo[] auth)
        {
            var list = new List<IAuthenticationInfo>(auth);
            list = list.Where(x => x != null).ToList();
            if (list.Count() == 1)
            {
                return list.First();
            }

            return new CombinationAuthenticationInfo(list);
        }

        public abstract void Setup(IServerEnvironment serverEnvironment);

        public AuthenticationType ToAuthenticationType()
             => GetType().AuthenticationInfoTypeToAuthenticationType();

        public IAuthenticationService GetAuthenticationService()
            => GetType().CreateAuthenticationServiceByAuthenticationInfo();
    }
}
