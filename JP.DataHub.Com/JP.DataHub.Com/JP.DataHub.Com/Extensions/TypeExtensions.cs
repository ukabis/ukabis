using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

#nullable enable

namespace JP.DataHub.Com.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasInheritance<T>(this Type type)
        {
            var _t = typeof(T);
            var inheritancies = type.GetInterfaces();
            return inheritancies.Contains(_t);
        }

        public static T? Create<T>(this Type type) => (T)Activator.CreateInstance(type);

        public static T? Create<T>(this Type type, params object?[]? args) => (T)Activator.CreateInstance(type, args);

        public static bool IsSimpleType(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            type = underlyingType ?? type;
            var simpleTypes = new List<Type>
                               {
                                   typeof(byte),
                                   typeof(sbyte),
                                   typeof(short),
                                   typeof(ushort),
                                   typeof(int),
                                   typeof(uint),
                                   typeof(long),
                                   typeof(ulong),
                                   typeof(float),
                                   typeof(double),
                                   typeof(decimal),
                                   typeof(bool),
                                   typeof(string),
                                   typeof(char),
                                   typeof(Guid),
                                   typeof(DateTime),
                                   typeof(DateTimeOffset),
                                   typeof(byte[])
                               };
            return simpleTypes.Contains(type) || type.IsEnum;
        }

        public static MethodInfo GetGenericMethod(this Type type, string methodName, params Type[] typeArguments)
        {
            var method = type.GetMethods().Where(x => x.Name == methodName && x.ReturnParameter.ToString().IndexOf("[") != -1).FirstOrDefault();
            if (method == null)
            {
                return null;
            }
            return method?.MakeGenericMethod(typeArguments);
        }

        public static T FindField<T>(this Type type, object target, string name)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.Name == name)
                {
                    var val = field.GetValue(target);
                    if (val is string)
                    {
                        return (T)val;
                    }
                }
            }
            return default(T);
        }

        public static T FindProperty<T>(this Type type, object target, string name)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var prop in props)
            {
                if (prop.Name == name)
                {
                    var val = prop.GetValue(target);
                    if (val is string)
                    {
                        return (T)val;
                    }
                }
            }
            return default(T);
        }

        public static void SetPropertyValue<T>(this Type type, object target, string name, T val)
        {
            var prop = type.GetProperty(name);
            if (prop == null)
            {
                throw new Exception($"型({type.Name}に{name}プロパティがありません。");
            }
            if (prop.CanWrite == false)
            {
                throw new Exception($"型({type.Name}の{name}プロパティにsetがありません。");
            }
            prop?.SetValue(target, val);
        }
    }
}
