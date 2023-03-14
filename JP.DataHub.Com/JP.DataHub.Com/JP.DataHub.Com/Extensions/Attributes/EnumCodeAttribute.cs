using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions.Attributes
{
    public class EnumCodeAttribute : Attribute
    {
        public string Code { get; set; }
        public EnumCodeAttribute(string code)
        {
            Code = code;
        }
    }
}
