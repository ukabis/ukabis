using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class AdminAuthResult : IValueObject
    {
        public bool Value { get; }

        public AdminAuthResult(bool result)
        {
            this.Value = result;
        }

        public static bool operator ==(AdminAuthResult me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AdminAuthResult me, object other) => !me?.Equals(other) == true;
    }

    internal static class AdminAuthResultExtension
    {
        public static AdminAuthResult ToAdminAuthResult(this bool val) => new AdminAuthResult(val);
    }
}
