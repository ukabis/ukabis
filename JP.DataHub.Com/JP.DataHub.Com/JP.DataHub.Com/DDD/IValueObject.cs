using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace JP.DataHub.Com.DDD
{
    [ContractClass(typeof(ValueObjectContract<>))]
    public partial interface IValueObject
    {
    }


    [ContractClassFor(typeof(IValueObject))]
    public abstract class ValueObjectContract<T> : IValueObject
    {
        public bool SameIdentityAs(T other)
        {
            return SameValueAs.SameValueAsTrue(this, other);
        }
    }
}
