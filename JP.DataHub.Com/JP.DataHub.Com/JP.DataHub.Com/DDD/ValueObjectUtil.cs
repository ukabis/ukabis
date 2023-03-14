using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.DDD
{
    public static class ValueObjectUtil
    {
        /// <summary>
        /// Tで指定されたValueObjectのインスタンスを作成する
        /// その際コンストラクタは、objで得られたクラスからコンストラクタの引数の型にあうものを探す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Create<T>(params object[] obj) where T : class
        {
            var objProps = new Dictionary<object, List<PropertyInfo>>();
            foreach (object src in obj)
            {
                if (src != null)
                {
                    objProps.Add(src, new List<PropertyInfo>());
                }
            }

            var type = typeof(T);
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors != null)
            {
                var hit = constructors.Where(x => x.GetCustomAttribute<DefaultValueObjectAttribute>() != null).FirstOrDefault();
                if (hit == null)
                {
                    hit = constructors.FirstOrDefault();
                }
                if (hit != null)
                {
                    var parameters = hit.GetParameters();
                    if (parameters == null || parameters.Count() == 0)
                    {
                        return hit.Invoke(null) as T;
                    }
                    else
                    {
                        var arguments = new object[parameters.Count()];
                        for (int i = 0; i < parameters.Count(); i++)
                        {
                            arguments[i] = FindPropertyObject(parameters[i], objProps);
                        }
                        return hit.Invoke(arguments) as T;
                    }
                }
            }
            throw new Exception("ValueObjectが作れません");
        }

        private static object FindPropertyObject(ParameterInfo param, Dictionary<object, List<PropertyInfo>> objProps)
        {
            foreach (var obj in objProps)
            {
                if (obj.Value.Count() == 0)
                {
                    var type = obj.Key.GetType();
                    var x = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                    obj.Value.AddRange(x);
                }
                //型がそのまま指定された場合はそのまま返す
                if (obj.Key is JToken)
                {
                    if (param.ParameterType == typeof(JToken))
                    {
                        return obj.Key;
                    }
                }
                else if (param.ParameterType == obj.Key.GetType())
                {
                    return obj.Key;
                }

                //TypeとNameが一致しているものを取得
                foreach (var prop in obj.Value)
                {
                    if (param.ParameterType == prop.PropertyType && param.Name.ToLower() == prop.Name.ToLower())
                    {
                        return prop.GetValue(obj.Key);
                    }
                }
                //TypeとNameが一致しなかった場合はTypeのみで一致させる
                foreach (var prop in obj.Value)
                {
                    if (param.ParameterType == prop.PropertyType)
                    {
                        return prop.GetValue(obj.Key);
                    }
                }
            }
            return null;
        }
    }
}
