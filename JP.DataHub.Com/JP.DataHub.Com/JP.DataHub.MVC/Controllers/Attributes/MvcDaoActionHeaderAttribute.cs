using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Interception.PolicyInjection.Policies;

namespace JP.DataHub.MVC.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MvcDaoActionHeaderAttribute : Attribute
    {
        public string Key { get; }

        public string[] Value { get; }

        public MvcDaoActionHeaderAttribute(string key, params string[] value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("リクエストヘッダー名をnullまたは空文字にすることはできません。");
            }
            this.Key = key;
            this.Value = value;
        }
    }
}
