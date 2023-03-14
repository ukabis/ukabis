using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    internal record OpenIdCa : IValueObject
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key(1)]
        public string Id { get; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        [Key(2)]
        public string ApplicationId { get; }

        /// <summary>
        /// アプリケーション名
        /// </summary>
        [Key(3)]
        public string ApplicationName { get; }

        [Key(4)]
        public bool IsActive { get; }

        /// <summary>
        /// アクセス制御（alw:許可 / dny:拒否 / inh:継承）
        /// </summary>
        [Key(5)]
        public string AccessControl { get; }

        public OpenIdCa(string id, string applicationId, string applicationName, bool isActive, string accessControl)
        {
            Id = id;
            ApplicationId = applicationId;
            ApplicationName = applicationName;
            IsActive = isActive;
            AccessControl = accessControl;
        }

        public static bool operator ==(OpenIdCa me, object other) => me?.Equals(other) == true;

        public static bool operator !=(OpenIdCa me, object other) => !me?.Equals(other) == true;
    }
}
