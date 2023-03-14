using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record RepositoryKey : IValueObject
    {
        public string Value { get; }

        public string Type { get; internal set; }

        public IEnumerable<string> LogicalKeys { get; internal set; }

        public bool IsExsitsLogicalKey { get => LogicalKeys.Any(); }

        public bool IsLogicalKeyOnce { get; internal set; }

        public string LogicalKeyFirst { get; internal set; }

        public RepositoryKey(string value)
        {
            Value = value;
            LogicalKeys = EditLogicalKeys().Distinct().ToList();
            IsLogicalKeyOnce = LogicalKeys.Count() == 1;
            LogicalKeyFirst = LogicalKeys.Count() == 1 ? LogicalKeys.FirstOrDefault() : null;
            Type = EditType();
        }

        private IEnumerable<string> EditLogicalKeys()
        {
            if (string.IsNullOrEmpty(Value) == true)
            {
                yield break;
            }
            foreach (var key in Value.Split("/".ToCharArray()).ToList().Where(x => string.IsNullOrEmpty(x) == false))
            {
                if (key.StartsWith("{") == true && key.EndsWith("}") == true)
                {
                    yield return key.Substring(1, key.Length - 2);

                }
            }
            yield break;
        }

        private string EditType()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return "";
            }
            return string.Join("~", Value.Split('/').Where(x => string.IsNullOrEmpty(x) == false && !(x.StartsWith("{") == true && x.EndsWith("}") == true)));
        }

        public string CreateUrl(JToken token)
        {
            string result = Value.Replace(".", "/");
            foreach (var k in LogicalKeys)
            {
                var val = token[k]?.ToString();
                result = result.Replace($"{{{k}}}", val);
            }
            return result;
        }

        public static bool operator ==(RepositoryKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(RepositoryKey me, object other) => !me?.Equals(other) == true;
    }

    internal static class RepositoryKeyExtension
    {
        public static RepositoryKey ToRepositoryKey(this string str) => str == null ? null : new RepositoryKey(str);
    }
}
