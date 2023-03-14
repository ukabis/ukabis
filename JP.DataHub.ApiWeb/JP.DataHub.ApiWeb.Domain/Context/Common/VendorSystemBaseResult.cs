using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public class VendorSystemBaseResult : IValueObject
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="vendorName">ベンダー名</param>
        /// <param name="systemId">システムID</param>
        /// <param name="systemName">システム名</param>
        public VendorSystemBaseResult(string vendorId, string vendorName, string systemId, string systemName)
        {
            VendorId = vendorId;
            VendorName = vendorName;
            SystemId = systemId;
            SystemName = systemName;
        }

        public static bool operator ==(VendorSystemBaseResult me, object other) => me?.Equals(other) == true;

        public static bool operator !=(VendorSystemBaseResult me, object other) => !me?.Equals(other) == true;
    }
}
