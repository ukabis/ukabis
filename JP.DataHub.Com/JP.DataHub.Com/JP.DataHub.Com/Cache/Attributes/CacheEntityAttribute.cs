using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class CacheEntityAttribute : Attribute
    {
        public IReadOnlyCollection<string> Entities { get; set; }
        private List<string> list = new List<string>();

        public CacheEntityAttribute(params object[] member)
        {
            foreach (var m in member)
            {
                list.Add(m.ToString());
            }
            Entities = list;
        }

        public CacheEntityAttribute(params string[] entities)
        {
            if (entities != null)
            {
                list.AddRange(entities);
            }
            Entities = list;
        }
    }
}
