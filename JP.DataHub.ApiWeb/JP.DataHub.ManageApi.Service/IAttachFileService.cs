using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service
{
    internal interface IAttachFileService
    {
        void DeleteByVendorId(string vendor_id);
        string GetAttachFileStorageIdByVendorId(string vendor_id);
        IList<AttachFileStorageModel> GetAttachFileStorageList();
        void RegisterVendorAttachFileStorage(string vendor_id, string attach_file_id, bool is_current);
    }
}
