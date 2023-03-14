using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.MVC.Session
{
    public static class SessionExtensions
    {
        public static void SetObject<TObject>(this ISession session, string key, TObject obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            session.SetString(key, json);
        }

        // セッションからオブジェクトを読み込む
        public static TObject GetObject<TObject>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return string.IsNullOrEmpty(json) ? default(TObject) : JsonConvert.DeserializeObject<TObject>(json);
        }

        public static object GetObject(this ISession session, string key, Type type)
        {
            var json = session.GetString(key);
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject(json, type);
        }

        public static T Get<T>(this ISession session, string key)
            => session.GetString(key).ToJson<T>();

        public static T Get<T>(this ISession session, string key, T def)
        {
            var str = session.GetString(key);
            return str == null ? def : str.ToJson<T>();
        }

        public static void To(this ISession session, string key, object obj)
            => session.SetString(key, obj == null ? null : obj.ToJsonString());

        public static void Store(this ISession session, object page)
        {
            var props = page?.GetType().GetProperties().ToList();
            props.ForEach(prop => {
                var attr = GetAttributeName(prop, page, props);
                if (attr.IsExists == true)
                {
                    session.SetObject(attr.Name ?? prop.Name, prop.GetValue(page));
                }
            });
        }

        public static void Restore(this ISession session, object page)
        {
            var props = page?.GetType().GetProperties().ToList();
            props.ToList().ForEach(prop => {
                var attr = GetAttributeName(prop, page, props);
                if (attr.IsExists == true)
                {
                    prop.SetValue(page, session.GetObject(attr.Name ?? prop.Name, prop.PropertyType));
                }
            });
        }

        public static Task StoreAsync(this ISession session, object page)
            => Task.Run(() => Store(session, page));

        public static Task RestoreAsync(this ISession session, object page)
            => Task.Run(() => Restore(session, page));

        private static (bool IsExists, string Name) GetAttributeName(PropertyInfo prop, object page, List<PropertyInfo> props)
        {
            var attr = prop.GetCustomAttribute<SessionAttribute>();
            if (attr != null)
            {
                return (true, attr.Name);
            }
            var attrparam = prop.GetCustomAttribute<SessionParameterAttribute>();
            if (attrparam != null)
            {
                var p = props.FirstOrDefault(x => x.Name == attrparam.Name);
                if (p != null)
                {
                    return (true, p.GetValue(page)?.ToString());
                }
            }
            return (false, null);
        }
    }
}
