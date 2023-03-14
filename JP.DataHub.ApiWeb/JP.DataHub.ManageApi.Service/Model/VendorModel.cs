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
    public class VendorModel
    {
        public string VendorId { get; set; }

        public string VendorName { get; set; }

        public bool IsDataUse { get; set; }

        public bool IsDataOffer { get; set; }

        public DateTime UpdDate { get; set; }

        public bool IsEnable { get; set; }
        public string AttachFileStorageId { get; set; }

        public IList<AttachFileStorageModel> AttachFileStorage { get; set; }

        public string RepresentativeMailAddress { get; set; }

        public IList<OpenIdCaModel> OpenIdCaList { get; set; }

        public IList<VendorLinkModel> VendorLinkList { get; set; } = new List<VendorLinkModel>();

        public IList<VendorSystemModel> SystemList { get; set; } = new List<VendorSystemModel>();

        public IList<StaffRoleModel> StaffList { get; set; } = new List<StaffRoleModel>();
    }
}
