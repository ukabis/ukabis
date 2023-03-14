using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Repository.Attributes
{
    public class CrudRepositoryArrayAttribute : Attribute
    {
        public bool IsArray { get; set; }

        public CrudRepositoryArrayAttribute(bool isArray = true)
        {
            IsArray = isArray;
        }
    }
}
