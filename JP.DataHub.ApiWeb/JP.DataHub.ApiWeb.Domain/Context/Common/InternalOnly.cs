using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record InternalOnly : IValueObject
    {
        public bool IsInternalOnly { get; }
        
        public string InternalOnlyKeyword { get; }

        public InternalOnly(bool isInternalOnly, string internalOnlyKeyword = null)
        {
            this.IsInternalOnly = isInternalOnly;
            this.InternalOnlyKeyword = internalOnlyKeyword;
        }

        public static bool operator ==(InternalOnly me, object other) => me?.Equals(other) == true;

        public static bool operator !=(InternalOnly me, object other) => !me?.Equals(other) == true;
    }
}