using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Models
{
    public class WrapString
    {
        public string Value { get; set; }

        public WrapString()
        {
        }

        public WrapString(string value)
        {
            Value = value;
        }
    }
}
