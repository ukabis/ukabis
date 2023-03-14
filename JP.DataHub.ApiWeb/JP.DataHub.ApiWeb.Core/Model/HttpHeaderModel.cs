using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public class HttpHeaderModel : Dictionary<string, List<string>>
    {
        public HttpHeaderModel()
        {
        }

        public HttpHeaderModel(Dictionary<string, List<string>> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public void Add(string key, string val)
        {
            Add(key, new List<string>() { val });
        }
    }
}
