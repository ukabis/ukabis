using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using StackExchange.Profiling.Internal;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Utilities;
using Newtonsoft.Json;
using MessagePack;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.DataContainer;

namespace JP.DataHub.Com.Cache
{
    public static class CacheHelper
    {
        private static Lazy<IHttpContextAccessor> _httpContextAccessor = new Lazy<IHttpContextAccessor>(() => UnityCore.Resolve<IHttpContextAccessor>());
        private static IHttpContextAccessor httpContextAccessor => _httpContextAccessor.Value;

        public static string CreateCacheKey(string keyPrefix, Dictionary<string, string> inputParams = null, IEnumerable<string> headerNames = null, IDictionary<string, string> dicDataContainer = null)
            => new CacheKeyParam(keyPrefix, inputParams, headerNames, dicDataContainer).Rebuild();

        public static CacheKeyParam CreateCacheKeyParam(string keyPrefix, MethodInfo methodInfo = null, Type type = null, object target = null, IParameterCollection arguments = null, List<object> inputs = null)
            => new CacheKeyParam(keyPrefix, methodInfo, type, target, arguments, inputs);

        /// <summary>
        /// キャッシュキーを分解して、KeyPrefix、引数パラメータ、ヘッダーを返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static CacheKeyParam ParseCacheKeyParam(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            var keys = key?.Split('.');
            var result = new CacheKeyParam();
            result.Prefix = keys[0];
            var regex = new Regex("^{'(?<name>.*?)':'(?<value>.*?)'}$", RegexOptions.Singleline);
            for (int i = 1; i < keys.Length; i++)
            {
                var match = regex.Match(keys[i]);
                if (match.Success == true)
                {
                    var name = match?.Groups["name"]?.Value;
                    var value = match?.Groups["value"]?.Value;
                    var isHeader = name.IndexOf("Header-") == -1 ? false : true;
                    var isDataContainer = name.IndexOf("DC-") == -1 ? false : true;
                    if (isHeader == false && isDataContainer == false)
                    {
                        // parameter
                        result.Param.Add(name, value);
                    }
                    else if (isHeader == true)
                    {
                        // header
                        result.Header.Add(name.Replace("Header-", ""), value);
                    }
                    else if (isDataContainer == true)
                    {
                        // DataContainer
                        result.DataContainer.Add(name.Replace("DC-", ""), value);
                    }
                }
            }
            return result;
        }

        private static object ValueObjectTo(object obj)
        {
            if (obj == null)
            {
                return AbstractCache.NullValue;
            }
            else if (obj is IValueObject vo)
            {
                return string.Join("-", vo.EnumerableShallowProperty().ToList());
            }
            else
            {
                return obj;
            }
        }

        public static string CreateKey(params object[] args)
        {
            bool first = true;
            StringBuilder result = new StringBuilder();
            foreach (var arg in args)
            {
                string add = null;
                if (first == false)
                {
                    result.Append(".");
                }
                if (arg == null)
                {
                    add = AbstractCache.NullValue;
                }
                else
                {
                    var type = arg.GetType();
                    var val = arg.ToString();
                    if (type.IsPrimitive == true || type == typeof(string) || type == typeof(Guid) || val != type.FullName)
                    {
                        add = val;
                    }
                    else
                    {
                        val = Convert.ToBase64String(MessagePackSerializer.Serialize(JsonConvert.SerializeObject(arg)));
                        add = val;
                    }
                }
                result.Append(add.Replace("." , "[dot]"));
                first = false;
            }
            return result.ToString();
        }

        /// <summary>
        /// ヘッダー名からヘッダー名をkey、ヘッダーの値をvalueとした、key : value文字列を返す。複数の場合は全てを返す。
        /// </summary>
        /// <param name="headerNames"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetHeaderKey(IEnumerable<string> headerNames)
        {
            if (headerNames != null)
            {
                foreach (var headerName in headerNames)
                {
                    if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey(headerName))
                    {
                        var header = string.Join("l", httpContextAccessor.HttpContext.Request.Headers[headerName].ToArray());
                        if (string.IsNullOrEmpty(header) == false)
                        {
                            yield return $"{{'Header-{headerName}':'{header}'}}";
                        }
                    }
                    else
                    {
                        yield return $"{{'Header-{headerName}':'[null]'}}";
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// DataContainer名からDataContainer名をkey、DataContainerの値をvalueとした、key : value文字列を返す。複数の場合は全てを返す。
        /// </summary>
        /// <param name="dataContainerNames"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetDataContainerKey(IDictionary<string, string> dicDataContainer)
        {
            var dataContainer = DataContainerUtil.ResolveDataContainer();
            if (dicDataContainer != null)
            {
                foreach (var key in dicDataContainer.Keys)
                {
                    var val = dataContainer.FindObjectPath(dicDataContainer[key]);
                    if (val != null)
                    {
                        yield return $"{{'DC-{key}':'{val}'}}";
                    }
                    else
                    {
                        yield return $"{{'DC-{key}':'[null]'}}";
                    }
                }
            }
            yield break;
        }
    }
}
