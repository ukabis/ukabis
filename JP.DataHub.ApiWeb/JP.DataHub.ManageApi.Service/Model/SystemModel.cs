using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Flags]
    public enum SystemItem
    {
        Head = 0,
        Client = 2,
        Link = 4,
        Admin = 8,
        All = 0xff,
    }

    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class SystemModel
    {
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string OpenIdApplicationId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }

        public IList<ClientModel> ClientList { get; set; }

        public string OpenIdClientSecret { get; set; }

        public bool IsEnable { get; set; }

        public IList<SystemLinkModel> SystemLinkList { get; set; }

        public SystemAdminModel SystemAdmin { get; set; }

        public DateTime SystemUpdDate { get; set; }

        public string RepresentativeMailAddress { get; set; }
    }
}