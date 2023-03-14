using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using JP.DataHub.Com.Misc;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace JP.DataHub.Com.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsConvert<T>(this object obj) => obj.IsConvert(typeof(T));

        public static bool IsConvert(this object obj, Type type)
        {
            try
            {
                if (type == typeof(Guid))
                {
                    Guid x;
                    if (Guid.TryParse(obj?.ToString(), out x) == false)
                    {
                        return false;
                    }
                    return true;
                }
                System.Convert.ChangeType(obj, type);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static T To<T>(this object obj)
        {
            if (obj == null)
            {
                return default(T);
            }
            var type = typeof(T);
            if (Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type.IsPrimitive == true || type == typeof(Guid))
            {
                return (T)Convert(obj, type);
            }
            else
            {
                return obj.Map<T>();
            }
        }

        public static T To<T>(this object obj, T def)
        {
            var type = typeof(T);
            if (Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type.IsPrimitive == true)
            {
                return (T)Convert(obj, type) ?? def;
            }
            else
            {
                return obj.Map<T>();
            }
        }

        public static object To(this object obj, Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type.IsPrimitive == true)
            {
                return Convert(obj, type);
            }
            else
            {
                return obj.Map(type);
            }
        }

        public static List<object> JsonToList(this JToken json, Type type)
        {
            var result = new List<object>();
            foreach (var once in json)
            {
                if (type == typeof(string))
                {
                    result.Add(once.ToString());
                }
                else
                {
                    result.Add(once.ToString().ToJson(type));
                }
            }
            return result;
        }

        public static List<T> ToList<T>(this object response)
        {
            var result = new List<T>();
            if (response is IEnumerable<object> enumlist)
            {
                foreach (var item in enumlist)
                {
                    result.Add(item.To<T>());
                }
            }
            else if (response is List<T> objlist)
            {
                return objlist;
            }
            else if (response is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    result.Add(item.To<T>());
                }
            }
            return result;
        }

        public static object ToList(this object response, Type type)
        {
            var convertedType = typeof(List<>).MakeGenericType(type);
            var result = Activator.CreateInstance(convertedType);

            if (response is IEnumerable<object> enumlist)
            {
                foreach (var item in enumlist)
                {
                    result.ExecuteMethod("Add", item.To(type));
                }
            }
            else if (response is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    result.ExecuteMethod("Add", item.To(type));
                }
            }
            return result;
        }

        public static T Convert<T>(this object obj)
        {
            try
            {
                var type = typeof(T);
                if (type == typeof(Guid))
                {
                    return (T)(object)Guid.Parse(obj?.ToString());
                }
                return (T)System.Convert.ChangeType(obj, type);
            }
            catch
            {
                return default(T);
            }
        }

        public static T Convert<T>(this object obj, T def)
        {
            try
            {
                var type = typeof(T);
                if (type == typeof(Guid))
                {
                    return (T)(object)Guid.Parse(obj?.ToString());
                }
                return (T)System.Convert.ChangeType(obj, type);
            }
            catch
            {
                return def;
            }
        }

        public static object Convert(this object obj, Type type)
        {
            try
            {
                if (type == typeof(Guid))
                {
                    Guid x;
                    if (Guid.TryParse(obj?.ToString(), out x) == false)
                    {
                        return null;
                    }
                    return x;
                }
                return System.Convert.ChangeType(obj, type);
            }
            catch
            {
                return null;
            }
        }

        public static bool TryConvert(this object obj, Type type, out object result)
        {
            result = null;
            try
            {
                if (type == typeof(Guid))
                {
                    if (Guid.TryParse(obj?.ToString(), out var guid))
                    {
                        result = guid;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    result = System.Convert.ChangeType(obj, type);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// デフォルト値かどうか判定する Primitive型にしか対応していない
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDefaultValue(this object obj, Type type)
        {
            return obj switch
            {
                null => false,
                bool b => b == false,
                string s => string.IsNullOrEmpty(s),
                _ => System.Convert.ChangeType(obj, type) == Activator.CreateInstance(type)
            };
        }

        public static int PropertiesGetHashCode(this object obj) => JoinedValueShallowJProperties(obj).GetHashCode();

        public static string JoinedValueShallowJProperties(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var type = obj.GetType();
            //IList IDictionaryのItem のように引数を必要とするgetterがあるクラスだとエラーになるので
            //引数無のGetMethodがあるPropertyのみを抽出する
            var props = type.GetProperties().Where(x => !x.GetGetMethod().GetParameters().Any());

            return string.Join(",", props.Select(x =>
            {
                return x?.GetValue(obj)?.ToString() ?? "[null]";
            }).ToList());
        }

        public static IEnumerable<string> EnumerableShallowProperty(this object obj)
        {
            if (obj != null)
            {
                var type = obj.GetType();
                foreach (var x in type.GetProperties())
                {
                    yield return x?.GetValue(obj)?.ToString() ?? "[null]";
                }
            }
            yield break;
        }

        public static bool IsSameValue(this object me, object other) => SameValueAs.SameValueAsTrue(me, other);

        public static object FindObjectPath(this object obj, string objectPath) => FindObjectPath(obj, objectPath.Split('.'));

        public static T FindObjectPath<T>(this object obj, string objectPath) => (T)System.Convert.ChangeType(FindObjectPath(obj, objectPath)?.ToString(), typeof(T));

        private static Regex regexArray = new Regex("\\[(?<master>.*?)\\]$", RegexOptions.Singleline);

        public static Match ArrayDescriptor(string text)
            => regexArray.Match(text);

        /// <summary>
        /// objectからobjectPathにあるValue（文字列）を取得する
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objectPath"></param>
        /// <returns></returns>
        public static object FindObjectPath(this object obj, string[] objectPath)
        {
            if (obj == null || objectPath == null || objectPath.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < objectPath.Length; i++)
            {
                if (obj == null)
                {
                    break;
                }
                var match = ArrayDescriptor(objectPath[i]);
                if (match.Success == true)
                {
                    var name = objectPath[i].Substring(0, match.Index);
                    PropertyInfo prop = obj.GetType().GetProperty(name);
                    if (prop == null)
                    {
                        return null;
                    }
                    int pos = objectPath[i].Substring<int>(match.Index + 1, match.Length - 2);
                    obj = prop.GetValue(obj);
                    if (obj != null)
                    {
                        obj = GetObjectByPos((IEnumerable)obj, pos);
                    }
                }
                else
                {
                    PropertyInfo prop = obj.GetType().GetProperty(objectPath[i]);
                    if (prop == null)
                    {
                        return null;
                    }
                    obj = prop.GetValue(obj);
                }
            }
            return obj;
        }

        private static object GetObjectByPos(IEnumerable obj, int pos)
        {
            if (obj == null)
            {
                return null;
            }
            int i = 0;
            foreach (var item in obj)
            {
                if (pos == i)
                {
                    return item;
                }
                i++;
            }
            return null;
        }

        public static IList<object> FindObjectPathList(this object obj, string objectPath) => FindObjectPathList(new List<object>() { obj }, objectPath.Split('.'));
        public static IList<object> FindObjectPathList(this object obj, IList<string> objectPath) => FindObjectPathList(new List<object>() { obj }, objectPath);

        /// <summary>
        /// objectからobjectPathにあるValue（文字列）を取得する
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objectPath"></param>
        /// <returns></returns>
        public static IList<object> FindObjectPathList(this List<object> obj, string[] objectPath)
        {
            if (obj == null || objectPath == null || objectPath.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < objectPath.Length; i++)
            {
                if (obj == null || obj.Count() == 0)
                {
                    break;
                }
                var match = ArrayDescriptor(objectPath[i]);
                if (match.Success == true)
                {
                    var name = objectPath[i].Substring(0, match.Index);
                    string pos = objectPath[i].Substring<string>(match.Index + 1, match.Length - 2);
                    foreach (var item in obj)
                    {
                        PropertyInfo prop = item.GetType().GetProperty(name);
                        if (prop == null)
                        {
                            continue;
                        }
                        if (pos.IsConvert<int>())
                        {
                            int num = pos.To<int>();
                            var next = new List<object>();
                            var val = prop.GetValue(item);
                            val = GetObjectByPos((IEnumerable)val, num);
                            if (val != null)
                            {
                                next.Add(val);
                            }
                            obj = next;
                        }
                        else if (pos == "*")
                        {
                            var next = new List<object>();
                            var tmp = prop.GetValue(item);
                            if (tmp is IEnumerable enumitem)
                            {
                                foreach (var x in enumitem)
                                {
                                    if (x != null)
                                    {
                                        next.Add(x);
                                    }
                                }
                            }
                            obj = next;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                else
                {
                    var next = new List<object>();
                    foreach (var item in obj)
                    {
                        PropertyInfo prop = item.GetType().GetProperty(objectPath[i]);
                        if (prop == null)
                        {
                            continue;
                        }
                        var val = prop.GetValue(item);
                        if (val != null)
                        {
                            next.Add(val);
                        }
                    }
                    obj = next;
                }
            }
            return obj;
        }

        public static object SetObjectPath(this object obj, string objectPath, object val) => SetObjectPath(obj, objectPath.Split('.'), val);

        public static object SetObjectPath(this object obj, string[] objectPath, object val)
        {
            if (obj == null || objectPath == null || objectPath.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < objectPath.Length; i++)
            {
                bool isLast = i + 1 == objectPath.Length;
                if (obj == null)
                {
                    break;
                }
                var match = ArrayDescriptor(objectPath[i]);
                if (match.Success == true)
                {
                    var name = objectPath[i].Substring(0, match.Index);
                    PropertyInfo prop = obj.GetType().GetProperty(name);
                    if (prop == null)
                    {
                        return null;
                    }
                    int pos = objectPath[i].Substring<int>(match.Index + 1, match.Length - 2);
                    obj = prop.GetValue(obj);
                    if (obj != null)
                    {
                        if (isLast == true)
                        {
                            prop.SetValue(obj, val, new object[1] { pos });
                            break;
                        }
                        obj = ((object[])obj)[pos];
                    }
                }
                else
                {
                    PropertyInfo prop = obj.GetType().GetProperty(objectPath[i]);
                    if (prop == null)
                    {
                        return null;
                    }
                    if (isLast == true)
                    {
                        prop.SetValue(obj, val);
                        break;
                    }
                    obj = prop.GetValue(obj);
                }
            }
            return obj;
        }

        public static IDictionary<string, object> Merge(this object param, object obj)
        {
            var result = ObjectToDictionary(param);
            return DictionaryExtension.Merge(result, obj);
        }

        public static IDictionary<string, object> ObjectToDictionary(this object param)
        {
            var result = new Dictionary<string, object>();
            param?.GetType().GetProperties().ToList().ForEach(x => result.Add(x.Name, x.GetValue(param)));
            return result;
        }

        public static IDictionary<string, string> ObjectToStringDictionary(this object param)
        {
            var result = new Dictionary<string, string>();
            param?.GetType().GetProperties().ToDictionary(x => x.Name, y => y.GetPropertyValue<string>(y.Name));
            return result;
        }

        /// <summary>
        /// 型Tの変換かのうか？
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">オブジェクト</param>
        /// <returns>変換できたかを返す。true変換できる。false変換できない</returns>
        public static bool IsValid<T>(this object obj)
        {
            // string型には何でも変換ができる
            if (typeof(T) == typeof(string))
            {
                return true;
            }
            // Nullableか？Nullableなら型変換が出来ない場合はnullとするのでtrueを返す
            var type = typeof(T);
            if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();
                if (def.Name.StartsWith("Nullable"))
                {
                    return true;
                }
            }
            // それ以外
            var conv = TypeDescriptor.GetConverter(typeof(T));
            return conv.IsValid(obj);
        }

        public static T DeepCopy<T>(this T src)
        {
            ReadOnlySpan<byte> readOnlySpan = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<T>(src);
            return System.Text.Json.JsonSerializer.Deserialize<T>(readOnlySpan)!;
        }
    }
}
