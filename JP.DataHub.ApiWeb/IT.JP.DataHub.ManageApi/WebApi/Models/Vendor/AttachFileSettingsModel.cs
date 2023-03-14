using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Vendor
{
    public class AttachFileSettingsModel
    {
        public string AttachFileStorageId { get; set; }

        public string AttachFileStorageName { get; set; }
    }
    public class GetAttachFileSettingsResponseModel
    {
        public string AttachFileStorageId { get; set; }
    }
    public class RegisterAttachFileSettingsResponseModel
    {
        public string attachFileId { get; set; }
    }
}
