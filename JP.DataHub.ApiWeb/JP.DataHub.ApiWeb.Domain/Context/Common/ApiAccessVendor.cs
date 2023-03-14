using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ApiAccessVendor : IValueObject
    {
        public ApiAccessVendorId ApiAccessVendorId { get; set; }

        public ApiId ApiId { get; set; }

        public VendorId VendorId { get; set; }

        public ApiAccessKey AccessKey { get; set; }

        public SystemId SystemId { get; set; }

        public IsEnable IsEnable { get; set; }

        public static bool operator ==(ApiAccessVendor me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ApiAccessVendor me, object other) => !me?.Equals(other) == true;
    }
}
