using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Collections
{
    public class ReadOnlyStringDictionary : ReadOnlyDictionary<string, string>
    {
        public ReadOnlyStringDictionary(IDictionary<string, string> dictionary)
            : base(dictionary)
        {
        }

        public virtual new string ToString()
        {
            var result = new StringBuilder();
            result.Append($"[");
            foreach (var key in Keys)
            {
                result.Append($"{{ '{key}' : '{this[key]}' }},");
            }
            result.Append($"]");
            return result.ToString();
        }
    }
}
