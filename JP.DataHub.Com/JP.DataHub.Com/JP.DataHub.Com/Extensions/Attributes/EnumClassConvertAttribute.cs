using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions.Attributes
{
    public class EnumClassConvertAttribute : Attribute
    {
        public EnumClassConvertAttribute(Type @class)
        {
            Class = @class;
        }

        public Type Class { get; }
    }
}
