using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http
{
    public class CallApiOptions
    {
        public Dictionary<string, object> Param { get; set; } = new Dictionary<string, object>();

        public CallApiOptions()
        {
        }

        public CallApiOptions(Dictionary<string, object> param)
        {
            if (param != null)
            {
                Param = param;
            }
        }
    }
}
