using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CacheArgAttribute : Attribute
    {
        public string KeyName { get; private set; }
        public string KeyParamName { get; private set; }
        public bool IsAllParam { get; set; }

        public CacheArgAttribute(bool allParam)
        {
            IsAllParam = allParam;
        }

        public CacheArgAttribute(string name)
        {
            KeyName = name;
            KeyParamName = name;
        }

        public CacheArgAttribute(string keyName, string keyParamName)
        {
            KeyName = keyName;
            KeyParamName = keyParamName;
        }
    }
}
