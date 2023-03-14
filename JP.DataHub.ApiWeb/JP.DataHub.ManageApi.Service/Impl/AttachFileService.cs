using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class AttachFileService : AbstractService, IAttachFileService
    {
        private Lazy<IAttachFileRepository> _lazyAttachFileRepository = new(() => UnityCore.Resolve<IAttachFileRepository>());
        private IAttachFileRepository _attachFileRepository { get => _lazyAttachFileRepository.Value; }

        public void DeleteByVendorId(string vendor_id)
        {
            if (PerRequestDataContainer.VendorCheck(new { Vendorid = vendor_id }) == false)
            {
                throw new AccessDeniedException("操作へのアクセスが許可されていません");
            }
            _attachFileRepository.DeleteByVendorId(vendor_id);
        }

        public string GetAttachFileStorageIdByVendorId(string vendor_id)
            => _attachFileRepository.GetAttachFileStorageIdByVendorId(vendor_id);

        public IList<AttachFileStorageModel> GetAttachFileStorageList()
            => _attachFileRepository.GetAttachFileStorageList();

        public void RegisterVendorAttachFileStorage(string vendor_id, string attach_file_id, bool is_current)
            => _attachFileRepository.RegisterVendorAttachFileStorage(vendor_id, attach_file_id, is_current);
    }
}
