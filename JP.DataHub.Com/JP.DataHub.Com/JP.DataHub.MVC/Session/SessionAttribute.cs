using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Session
{
    public class SessionAttribute : Attribute
    {
        public string Name { get; set; }

        public SessionAttribute()
        {
        }

        public SessionAttribute(string name)
        {
            Name = name;
        }
    }


    public class SessionParameterAttribute : Attribute
    {
        public string Name { get; set; }

        public SessionParameterAttribute()
        {
        }

        public SessionParameterAttribute(string name)
        {
            Name = name;
        }
    }
}
