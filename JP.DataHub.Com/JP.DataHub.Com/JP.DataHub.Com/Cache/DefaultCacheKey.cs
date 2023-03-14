using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Utilities;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.Cache
{
    public class DefaultCacheKey : AbstractTypeCacheAttribute
    {
        public override void Parse(MethodInfo method, object target, IParameterCollection arguments, List<object> inputs)
        {
            var implType = target?.GetType();
            var implMethod = implType.GetMethods().Where(x => x.ToString() == method.ToString()).FirstOrDefault();
            var cas = implMethod?.GetCustomAttributes<CacheArgAttribute>().ToList();
            if (cas == null || cas.Count <= 0)
            {
                cas = method?.GetCustomAttributes<CacheArgAttribute>().ToList();
            }
            var isAllParam = cas?.Where(x => x.IsAllParam == true).FirstOrDefault()?.IsAllParam;            // 引数リストすべてを使うか？
            var KeyNames = new List<string>();
            var KeyParamNames = new List<string>();
            if (isAllParam == true)
            {
                for (int i = 0; i < arguments.Count; i++)
                {
                    var name = arguments.ParameterName(i);
                    KeyNames.Add(name);
                    KeyParamNames.Add(name);
                }
            }
            else
            {
                KeyNames = cas?.Select(x => x.KeyName).ToList();
                KeyParamNames = cas?.Select(x => x.KeyParamName).ToList();
            }
            var crha = method?.GetCustomAttribute<CacheRequestHeaderAttribute>();
            string[] HeaderNames = crha?.RequestHeaderName?.ToArray();

            // 引数リストを作成
            var inputParams = new Dictionary<string, string>();
            foreach (var keyParam in KeyParamNames)
            {
                var keySplits = keyParam.Split('.');
                var val = arguments.MethodArgumentToParamValue(keyParam);
                if (val == "[null]" || val == null)    // 引数リストから見つからない場合。その場合はTaretオブジェクトから探す
                {
                    var tmp = target.FindObjectPath(keyParam);
                    if (tmp is IValueObject vo)
                    {
                        val = vo?.JoinedValueShallowJProperties();
                    }
                    else
                    {
                        val = tmp?.ToString();
                    }
                }
                inputParams.Add(keySplits[0], val);
            }

            // DataContainerNameリスト
            var dicDataContainer = new Dictionary<string, string>();
            method.GetCustomAttributes<CacheDataContainerAttribute>().ForEach(x => dicDataContainer.Add(x.KeyName, x.ObjectPath));

            inputParams?.ToList().ForEach(x => Param.Add(x.Key, CacheKeyParam.CorrectValueString(CacheKeyParam.ValueObjectTo(x.Value))));
            HeaderNames?.ToList().ForEach(x => Header.Add(x, CacheKeyParam.CorrectValueString(CacheKeyParam.GetHeaderValue(x))));
            dicDataContainer?.ToList().ForEach(x => DataContainer.Add(x.Key, CacheKeyParam.CorrectValueString(CacheKeyParam.GetDataContainerObjectPathValue(x.Value))));
        }

        public override string Rebuild()
        {
            List<object> list = new List<object>();
            list.Add(Prefix);
            Param.Keys.ToList().ForEach(x => list.Add($"{{'{CacheKeyParam.CorrectKey(x)}':'{CacheKeyParam.CorrectValueString(Param[x])}'}}"));
            Header.Keys.ToList().ForEach(x => list.Add($"{{'Header-{CacheKeyParam.CorrectKey(x)}':'{CacheKeyParam.CorrectValueString(Header[x])}'}}"));
            DataContainer.Keys.ToList().ForEach(x => list.Add($"{{'DC-{CacheKeyParam.CorrectKey(x)}':'{CacheKeyParam.CorrectValueString(DataContainer[x])}'}}"));
            return CacheHelper.CreateKey(list.ToArray());
        }
    }
}
