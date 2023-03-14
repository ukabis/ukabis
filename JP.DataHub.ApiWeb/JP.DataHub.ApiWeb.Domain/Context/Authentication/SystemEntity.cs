using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    // .NET6
    [MessagePackObject]
    internal class SystemEntity : IEntity
    {
        [IgnoreMember]
        protected ISystemRepository SystemRepository => UnityCore.Resolve<ISystemRepository>();

        [IgnoreMember]
        protected ICacheManager CacheManager => UnityCore.Resolve<ICacheManager>();

        [IgnoreMember]
        protected ICache Cache => UnityCore.Resolve<ICache>();

        [Key(0)]
        public SystemId SystemId { get; set; }

        [Key(1)]
        public SystemName SystemName { get; set; }

        [Key(2)]
        public OpenIdApplicationId OpenIdApplicationId { get; set; }

        [Key(3)]
        public VendorId VendorId { get; set; }

        [Key(4)]
        public VendorName VendorName { get; set; }

        [Key(5)]
        public List<Client> ClientList { get; set; }

        [Key(6)]
        public OpenIdClientSecret OpenIdClientSecret { get; set; }

        [IgnoreMember]
        public SystemAdmin SystemAdmin { get; set; }

        [Key(7)]
        public IsEnable IsEnable { get; set; }

        [Key(8)]
        public UpdDate UpdDate { get; set; }

        [Key(9)]
        public List<EmailAddress> RepresentativeMailAddress { get; set; }

        public SystemEntity(SystemId systemId, SystemName systemName = null, OpenIdApplicationId openIdApplicationId = null, VendorId vendorId = null,
            VendorName vendorName = null, List<Client> clientList = null, OpenIdClientSecret openIdClientSecret = null, IsEnable isEnable = null, UpdDate updDate = null, List<EmailAddress> emailAddressList = null)
        {
            if (string.IsNullOrEmpty(systemId?.Value))
            {
                systemId = new SystemId(Guid.NewGuid().ToString());
            }

            SystemId = systemId;
            SystemName = systemName;
            OpenIdApplicationId = openIdApplicationId;
            VendorId = vendorId;
            VendorName = vendorName;
            ClientList = clientList;
            OpenIdClientSecret = openIdClientSecret;
            IsEnable = isEnable;
            UpdDate = updDate;
            RepresentativeMailAddress = emailAddressList;
        }

        public static SystemEntity Create(SystemId systemId, SystemName systemName = null, OpenIdApplicationId openIdApplicationId = null, VendorId vendorId = null,
            VendorName vendorName = null, List<Client> clientList = null, OpenIdClientSecret openIdClientSecret = null, IsEnable isEnable = null, UpdDate updDate = null, List<EmailAddress> emailAddressList = null)
        {
            return new SystemEntity(systemId, systemName, openIdApplicationId, vendorId, vendorName, clientList, openIdClientSecret, isEnable, updDate, emailAddressList);
        }

    }
}
