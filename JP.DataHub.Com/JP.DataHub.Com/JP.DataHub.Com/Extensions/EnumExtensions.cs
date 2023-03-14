using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions.Attributes;

namespace JP.DataHub.Com.Extensions
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
            if ((attributes?.Count() ?? 0) <= 0)
            {
                return null;
            }
            return attributes.First();
        }

        public static bool TryParse(this string s, Type type, out object wd)
        {
            return Enum.TryParse(type, s, out wd) && Enum.IsDefined(type, wd);
        }

        public static bool TryParse<TEnum>(this string s, out TEnum wd) where TEnum : struct, Enum
        {
            return Enum.TryParse(s, out wd) && Enum.IsDefined<TEnum>(wd);
        }

        private static object enumobj = new object();
        private static Dictionary<Type, Dictionary<object, string>> enummap = new Dictionary<Type, Dictionary<object, string>>();
        private static Dictionary<Type, Dictionary<string, object>> enummap2 = new Dictionary<Type, Dictionary<string, object>>();
        private static Dictionary<Type, object> enumdef = new Dictionary<Type, object>();
        private static ConcurrentDictionary<Type, Dictionary<string, Type>> enumClassMap = new();

        public static T GetDefaultEnum<T>()
        {
            if (enumdef.ContainsKey(typeof(T)) == false)
            {
                InitEnumMap<T>();
            }
            return (T)(enumdef.ContainsKey(typeof(T)) ? enumdef[typeof(T)] : default(T));
        }

        public static string ToCode<T>(this T obj)
        {
            if (enummap.ContainsKey(typeof(T)) == false)
            {
                InitEnumMap<T>();
            }
            var map = enummap[typeof(T)];
            return map[obj];
        }

        public static T ToEnum<T>(this string obj) => obj.ToEnum<T>(GetDefaultEnum<T>() ?? "xxx".ToEnum<T>());

        public static T ToEnum<T>(this string obj, string def) => ToEnum<T>(obj, def.ToEnum<T>());

        public static T ToEnum<T>(this string obj, T def)
        {
            if (obj == null)
            {
                return def;
            }
            object tmp = null;
            if (obj?.ToString().TryParse(typeof(T), out tmp) == true)
            {
                return (T)tmp;
            }
            if (enummap2.ContainsKey(typeof(T)) == false)
            {
                InitEnumMap<T>();
            }
            var map = enummap2[typeof(T)];
            return obj != null && map.ContainsKey(obj) == true ? (T)map[obj] : def;
        }

        public static Type ToClass<T>(this T obj)
        {
            var dic = enumClassMap.GetOrAdd(typeof(T), (type) =>
            {
                var classMap = new Dictionary<string, Type>();
                foreach (T enumValue in Enum.GetValues(typeof(T)))
                {
                    var x = enumValue.GetType()?.GetMember(enumValue.ToString())?.FirstOrDefault();
                    var attrClass = (EnumClassConvertAttribute)x?.GetCustomAttributes(typeof(EnumClassConvertAttribute), false).FirstOrDefault();
                    if (attrClass == null)
                    {
                        continue;
                    }
                    classMap.Add(enumValue.ToString(), attrClass.Class);

                }
                return classMap;
            });
            return dic[obj.ToString()];
        }

        private static void InitEnumMap<T>()
        {
            lock (enumobj)
            {
                // 二重でガードするのはlockする前のContainsKeyではなかったけど、ここに来てContainsKeyがあって衝突を防止するため
                if (enummap2.ContainsKey(typeof(T)) == false)
                {
                    var map = new Dictionary<object, string>();
                    var map2 = new Dictionary<string, object>();
                    foreach (T value in Enum.GetValues(typeof(T)))
                    {
                        var x = value.GetType()?.GetMember(value.ToString())?.FirstOrDefault();
                        var attrCode = (EnumCodeAttribute)x?.GetCustomAttributes(typeof(EnumCodeAttribute), false).FirstOrDefault();
                        if (attrCode != null)
                        {
                            map.Add(value, attrCode.Code);
                            map2.Add(attrCode.Code, value);
                        }
                        var attrDef = (EnumDefaultAttribute)x?.GetCustomAttributes(typeof(EnumDefaultAttribute), false).FirstOrDefault();
                        if (attrDef != null)
                        {
                            enumdef.Add(typeof(T), value);
                        }
                    }
                    enummap.Add(typeof(T), map);
                    enummap2.Add(typeof(T), map2);
                }
            }
        }
    }
}
