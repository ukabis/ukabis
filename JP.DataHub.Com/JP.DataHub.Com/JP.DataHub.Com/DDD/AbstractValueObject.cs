using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.DDD
{
    public abstract class AbstractValueObject : IValueObject
    {
        public override int GetHashCode() => this.PropertiesGetHashCode();

        public override string ToString() => this.JoinedValueShallowJProperties();
        public override bool Equals(object other) => this.IsSameValue(other);
    }
}
