using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CacheDataContainerAttribute : Attribute
    {
        public string KeyName { get; private set; }
        public string ObjectPath { get; private set; }

        public CacheDataContainerAttribute(string keyName)
        {
            KeyName = keyName;
            ObjectPath = keyName;
        }

        public CacheDataContainerAttribute(string keyName, string objectPath)
        {
            KeyName = keyName;
            ObjectPath = objectPath ?? keyName;
        }
    }
}
