using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IAttachFileRepository
    {
        string GetAttachFileStorageIdByVendorId(string vendorId);
        void DeleteByVendorId(string vendor_id, string excluideVendorAttachFileStorageId = null);
        IList<AttachFileStorageModel> GetAttachFileStorageList();
        void RegisterVendorAttachFileStorage(string vendor_id, string attach_file_id, bool is_current);
    }
}
