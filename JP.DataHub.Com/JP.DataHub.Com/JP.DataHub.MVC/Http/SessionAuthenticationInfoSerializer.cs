using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.MVC.Session;

namespace JP.DataHub.MVC.Http
{
    public class SessionAuthenticationInfoSerializer : IAuthenticationInfoSerializer
    {
        private Lazy<ISession> _session = new Lazy<ISession>(() => UnityCore.Resolve<ISession>());
        private ISession Session => _session.Value;

        public void Set<T>(string key, T obj)
        {
            if (UnityCore.IsRegistered<ISession>())
            {
                Session.SetObject<T>(key, obj);
            }
        }

        public void SetObject(string key, object obj)
        {
            if (UnityCore.IsRegistered<ISession>())
            {
                Type type = obj?.GetType();
                Session.SetString($"{key}-Type", $"{type?.FullName},{type.Assembly.ManifestModule.Name}".Replace(".dll", ""));
                Session.SetObject(key, obj);
            }
        }

        public void SetString(string key, string obj)
        {
            if (UnityCore.IsRegistered<ISession>())
            {
                Session.SetString(key, obj);
            }
        }

        public T Get<T>(string key) where T : new()
        {
            if (UnityCore.IsRegistered<ISession>())
            {
                Session.GetObject<T>(key);
            }
            return default(T);
        }

        public string GetString(string key)
        {
            if (UnityCore.IsRegistered<ISession>())
            {
                return Session.GetString(key);
            }
            return null;
        }

        public object GetObject(string key)
        {
            if (UnityCore.IsRegistered<ISession>())
            {
                var typename = GetString($"{key}-Type");
                if (!string.IsNullOrEmpty(typename))
                {
                    var type = Type.GetType(typename);
                    var str = Session.GetString(key);
                    var x = JsonConvert.DeserializeObject(str, type);
                    return x;
                }
                }
                return null;
        }
    }
}
