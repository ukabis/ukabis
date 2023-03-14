using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Unity.Interception.PolicyInjection.Pipeline;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.DataContainer;

namespace JP.DataHub.Com.Cache
{
    public class CacheKeyParam
    {
        private static Lazy<IHttpContextAccessor> _httpContextAccessor = new Lazy<IHttpContextAccessor>(() => UnityCore.Resolve<IHttpContextAccessor>());
        private static IHttpContextAccessor httpContextAccessor => _httpContextAccessor.Value;

        public string Prefix { get; set; }
        public Dictionary<string, string> Param { get; set; } = new();
        public Dictionary<string, string> Header { get; set; } = new();
        public Dictionary<string, string> DataContainer { get; set; } = new Dictionary<string, string>();
        public MethodInfo Method { get; }
        public bool IsNoCache { get; private set; }
        public List<object> Inputs { get; private set; }
        public IParameterCollection Arguments { get; private set; }
        public object Target { get; private set; }

        private Type Type { get; set; }
        private ITypeCacheAttribute TypeCache { get; set; }

        public CacheKeyParam()
        {
        }

        public CacheKeyParam(string keyPrefix, Dictionary<string, string> inputParams = null, IEnumerable<string> headerNames = null, IDictionary<string, string> dicDataContainer = null, MethodInfo methodInfo = null)
        {
            Type = typeof(DefaultCacheKey);
            Prefix = keyPrefix;
            inputParams?.ToList().ForEach(x => Param.Add(x.Key, CorrectValueString(ValueObjectTo(x.Value))));
            headerNames?.ToList().ForEach(x => Header.Add(x, CorrectValueString(GetHeaderValue(x))));
            dicDataContainer?.ToList().ForEach(x => DataContainer.Add(x.Key, CorrectValueString(GetDataContainerObjectPathValue(x.Value))));
            Method = methodInfo;
            TypeCache = Activator.CreateInstance(Type) as ITypeCacheAttribute;
            TypeCache.Prefix = Prefix;
            TypeCache.Param = Param;
            TypeCache.Header = Header;
            TypeCache.DataContainer = DataContainer;
        }

        public CacheKeyParam(string keyPrefix, MethodInfo methodInfo = null, Type type = null, object target = null, IParameterCollection arguments = null, List<object> inputs = null)
        {
            Type = type;
            Prefix = keyPrefix;
            Method = methodInfo;
            Arguments = arguments;
            Inputs = inputs;
            Target = target;
            TypeCache = Activator.CreateInstance(Type) as ITypeCacheAttribute;
            TypeCache.Prefix = Prefix;
            TypeCache.Parse(Method, Target, Arguments, Inputs);

            Param = TypeCache.Param;
            Header = TypeCache.Header;
            DataContainer = TypeCache.DataContainer;
            if (type != null)
            {
                IsNoCache = false;
            }
            else if (Prefix == null)
            {
                IsNoCache = true;
            }
        }

        public void AddParam(string key, object obj) => Param.Add(CorrectKey(key), CorrectValueString(ValueObjectTo(obj)));
        public void AddHeader(string key, string obj) => Header.Add(CorrectKey(key), CorrectValueString(obj));
        public void AddDataContainer(string key, object obj) => DataContainer.Add(CorrectKey(key), CorrectValueString(obj));

        public string Rebuild()
        {
            return TypeCache.Rebuild();
        }

        public static string CorrectKey(string str)
        {
            return str?.Replace("-", "[hyphen]");
        }

        public static string CorrectValueString(object obj)
        {
            string result;
            if (obj is Guid)
            {
                result = obj?.ToString().ToUpper();
            }
            else if (obj is string str)
            {
                result = str;
            }
            else
            {
                result = obj == null ? null : obj.ToString();
            }
            return result == null ? "[null]" : result?.Replace(".", "[dot]").Replace(":", "[colon]").Replace("'", "[s-quote]").Replace("\"", "[d-quote]");
        }

        public static object ValueObjectTo(object obj)
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

        public static string GetHeaderValue(string headerName)
        {
            if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey(headerName))
            {
                return string.Join("l", httpContextAccessor.HttpContext.Request.Headers[headerName].ToArray());
            }
            return null;
        }

        public static object GetDataContainerObjectPathValue(string objectPath)
        {
            var dataContainer = DataContainerUtil.ResolveDataContainer();
            return dataContainer.FindObjectPath(objectPath);
        }
    }
}
