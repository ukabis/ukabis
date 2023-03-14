using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    /// <summary>
    /// クライアント情報のエンティティです。
    /// </summary>
    [MessagePackObject]
    class ClientCertificate : IEntity
    {
        /// <summary>クライアント証明書の文字列</summary>
        [Key(0)]
        public ClientCertificateVO ClientCertificateObject { get; set; }

        /// <summary>ベンダーID</summary>
        [Key(1)]
        public VendorId VendorId { get; set; }

        /// <summary>システムID</summary>
        [Key(2)]
        public SystemId SystemId { get; set; }
    }
}
