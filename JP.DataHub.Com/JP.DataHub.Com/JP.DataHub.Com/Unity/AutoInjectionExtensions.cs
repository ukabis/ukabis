using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Com.Unity
{
    public static class AutoInjectionExtensions
    {
        public static void AutoInjection(this object obj, params object[] param)
        {
            if (obj != null)
            {
                AutoInjectionProperty(obj, param);
                AutoInjectionField(obj, param);
            }
        }

        private static void AutoInjectionProperty(object obj, params object[] param)
        {
            var type = obj.GetType();
            foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty))
            {
                var ca = p.GetCustomAttribute<DependencyAttribute>();
                if (ca != null)
                {
                    var value = ca.Name == null ? UnityCore.UnityContainer.Resolve(p.PropertyType) : UnityCore.UnityContainer.Resolve(p.PropertyType, ca.Name);
                    if (value != null)
                    {
                        p.SetValue(obj, value);
                    }
                }
            }
        }

        private static void AutoInjectionField(object obj, params object[] param)
        {
            var type = obj.GetType();
            foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField))
            {
                var ca = f.GetCustomAttribute<DependencyAttribute>();
                if (ca != null)
                {
                    var value = UnityCore.UnityContainer?.Resolve(f.FieldType, ca.Name);
                    if (value != null)
                    {
                        f.SetValue(obj, value);
                    }
                }
            }
        }
    }
}
