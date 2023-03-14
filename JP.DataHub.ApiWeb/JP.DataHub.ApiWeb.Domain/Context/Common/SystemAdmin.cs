using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    class SystemAdmin : IValueObject
    {
        [Key(0)]
        public string SystemAdminId { get; }

        [Key(1)]
        public bool IsActive { get; }

        [Key(2)]
        public string AdminSecret { get; }

        public SystemAdmin(string systemAdminId, bool isActive, string adminSecret)
        {
            SystemAdminId = systemAdminId;
            IsActive = isActive;
            AdminSecret = adminSecret ?? throw new ArgumentNullException(nameof(adminSecret));
        }

        public static bool operator ==(SystemAdmin me, object other) => me?.Equals(other) == true;

        public static bool operator !=(SystemAdmin me, object other) => !me?.Equals(other) == true;
    }
}
