using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using JP.DataHub.MVC.Session;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Blazor.Core.Storage
{
    public static class ProtectedSessionStorageExtensions
    {
        public static TValue Get<TValue>(this ProtectedSessionStorage storage, string key)
        {
            var x = storage.GetAsync<TValue>("days");
            var y = x.AsTask();
            var tmp = y.Result;
            return tmp.Success == true ? (TValue)tmp.Value : default(TValue);
        }

        public static async Task Restore(this ProtectedSessionStorage storage, object page)
        {
            var storageType = storage.GetType();
            var ppp = storageType.GetMethods();
            var method = ppp.FirstOrDefault(x => x.Name == "GetAsync");

            var props = page?.GetType().GetProperties().ToList();
            props.ToList().ForEach(prop => {
                var attr = GetAttributeName(prop, page, props);
                if (attr.IsExists == true)
                {
                    var m = method.MakeGenericMethod(prop.PropertyType);
                    var task = m.Invoke(storage, new object[] { attr.Name ?? prop.Name });
                    var valuetasktype = task.GetType();
                    var valuetasktype_result = valuetasktype.GetProperty("Result");
                    var result = valuetasktype_result.GetValue(task);

                    //var val = storage.GetAsync<prop.PropertyType>(attr.Name ?? prop.Name);
                    prop.SetValue(page, null);
                }
            });
        }

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
