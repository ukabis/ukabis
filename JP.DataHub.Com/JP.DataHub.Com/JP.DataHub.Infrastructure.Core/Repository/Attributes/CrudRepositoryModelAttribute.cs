using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Repository.Attributes
{
    public class CrudRepositoryModelAttribute : Attribute
    {
        public Type? Type { get; set; }

        public CrudRepositoryModelAttribute()
        {
        }

        public CrudRepositoryModelAttribute(Type type)
        {
            Type = type;
        }
    }
}
