using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Filters.Attributes
{
    public class UserRoleCheckControllerAttribute : Attribute
    {
        public string FunctionName { get; private set; }

        public UserRoleCheckControllerAttribute(string functionName)
        {
            FunctionName = functionName;
        }
    }
}
