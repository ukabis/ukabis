using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class ClientModel
    {
        /// <summary>クライアントID</summary>
        public string ClientId { get; set; }

        /// <summary>クライアントシークレット</summary>
        public string ClientSecret { get; set; }

        /// <summary>システムID</summary>
        public string SystemId { get; set; }

        /// <summary>アクセストークンの有効期限</summary>
        public TimeSpan AccessTokenExpirationTimeSpan { get; set; }

        /// <summary>IsActive</summary>
        public bool IsActive { get; set; }
    }
}
