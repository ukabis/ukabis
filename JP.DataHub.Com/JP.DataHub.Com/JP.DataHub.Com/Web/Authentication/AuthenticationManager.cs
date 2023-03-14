using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
//using Reactive.Bindings;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public class AuthenticationManager : IAuthenticationManager
    {
        public static string AccountJsonFileName { get; set; } = "account.json";

        private Dictionary<string, IAuthenticationInfo> dic = new Dictionary<string, IAuthenticationInfo>();

        //public ReactiveProperty<(ServerEnvironment, IAuthenticationInfo, IAuthenticationResult)> ChangeStatus { get; } = new ReactiveProperty<(ServerEnvironment,IAuthenticationInfo, IAuthenticationResult)>();

        public string FileName { get; set; }

        public AuthenticationManager(string fileName = null)
        {
            if (fileName.IsNullOrEmpty() == false)
            {
                Load(fileName);
            }
        }

        public void Load(string fileName)
        {
            dic = fileName.ReadFileContents().ToJson<Dictionary<string, IAuthenticationInfo>>();
        }

        public void Load(ServerList list)
        {
            dic = FileName.ReadFileContents().ToJson<Dictionary<string, IAuthenticationInfo>>();
            foreach (var envid in dic.Keys)
            {
                foreach (var server in list.List)
                {
                    if (server.EnvironmentList != null)
                    {
                        foreach (var env in server.EnvironmentList)
                        {
                            if (env.EnvironmentId == envid)
                            {
                                dic[envid].Environment = env;
                            }
                        }
                    }
                }
            }
        }

        public void Save()
        {
            dic.ToJson().ToString().ToFile(FileName);
        }

        public void Add(IAuthenticationInfo authenticationInfo)
        {
            if (authenticationInfo == null)
            {
                throw new ArgumentNullException(nameof(authenticationInfo));
            }
            if (authenticationInfo.AuthenticationInfoId == null)
            {
                throw new ArgumentNullException(nameof(authenticationInfo.AuthenticationInfoId));
            }
            if (authenticationInfo.Environment == null)
            {
                throw new ArgumentNullException(nameof(authenticationInfo.Environment));
            }

            if (dic.ContainsKey(authenticationInfo.AuthenticationInfoId))
            {
                dic[authenticationInfo.AuthenticationInfoId] = authenticationInfo;
            }
            else
            {
                dic.Add(authenticationInfo.AuthenticationInfoId, authenticationInfo);
            }
        }

        public IAuthenticationInfo Find(string authenticationId) => authenticationId == null ? null : dic.ContainsKey(authenticationId) ? dic[authenticationId] : null;

        public void Remove(string authenticationId)
        {
            if (dic.ContainsKey(authenticationId))
            {
                var env = dic[authenticationId]?.Environment;
                dic.Remove(authenticationId);
                Change(env);
            }
        }

        public void Change(ServerEnvironment environment)
        {
            //ChangeStatus.Value = (environment, null, null);
        }

        public void ChangeResult(IAuthenticationInfo authenticationInfo, IAuthenticationResult authenticationResult)
        {
            if (authenticationResult == null || authenticationResult.IsSuccessfull == true)
            {
                //ChangeStatus.Value = (authenticationInfo.Environment, authenticationInfo, authenticationResult);
            }
        }

        public IEnumerable<string> AccountList()
            => dic.Keys;

        #region Enumerable

        public IEnumerator<IAuthenticationInfo> GetEnumerator() => dic.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
