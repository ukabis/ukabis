using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class ReflectionExtensions
    {
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return (T)obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

        public static object GetFieldValue(this object obj, string fieldName)
        {
            return obj?.GetType().GetField(fieldName)?.GetValue(obj);
        }

        public static T GetFieldValue<T>(this object obj, string fieldName)
        {
            return (T)obj?.GetType().GetField(fieldName)?.GetValue(obj);
        }

        public static object ExecuteMethod(this object obj, string methodName, params object[] param)
        {
            return obj?.GetType().GetMethod(methodName)?.Invoke(obj, param);
        }

        public static bool ContainsCustomAttribute<T>(this IEnumerable<object> obj)
        {
            return obj.Where(x => x is T).Count() > 0 ? true : false;
        }

        public static T GetCustomAttribute<T>(this IEnumerable<object> obj) where T : Attribute
        {
            return obj.Where(x => x is T).FirstOrDefault() as T;
        }

        public static List<object> RemoveCustomAttribute<T>(this IList<object> obj) where T : Attribute
        {
            return obj.Where(x => !(x is T)).ToList();
        }
    }
}
