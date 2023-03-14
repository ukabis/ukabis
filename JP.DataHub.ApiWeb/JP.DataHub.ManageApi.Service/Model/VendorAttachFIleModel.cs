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
    public class VendorAttachFileModel
    {
        public string VendorId { get; set; }

        public string AttachFileStorageId { get; set; }
    }
}
