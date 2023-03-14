using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace JP.DataHub.Com.DDD
{
    [ContractClass(typeof(EntityContract<>))]
    public partial interface IEntity
    {
    }

    [ContractClassFor(typeof(IEntity))]
    public abstract class EntityContract<T> : IEntity
    {
        public bool SameIdentityAs(T other)
        {
            return SameValueAs.SameValueAsTrue(this, other);
        }
    }
}
