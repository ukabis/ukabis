using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class ResourceSharingPersonService : IResourceSharingPersonService
    {
        private IResourceSharingPersonRepository _resourceSharingPersonRepository => _lazyResourceSharingPersonRepository.Value;
        private Lazy<IResourceSharingPersonRepository> _lazyResourceSharingPersonRepository = new(() => UnityCore.Resolve<IResourceSharingPersonRepository>());

        private IVendorRepository _vendorRepository => _lazyVendorRepository.Value;
        private Lazy<IVendorRepository> _lazyVendorRepository = new(() => UnityCore.Resolve<IVendorRepository>());
        private ISystemRepository _systemRepository => _lazySystemRepository.Value;
        private Lazy<ISystemRepository> _lazySystemRepository = new(() => UnityCore.Resolve<ISystemRepository>());

        /// <summary>
        /// 指定されたIDの個人リソースシェアリングルールを削除します。
        /// </summary>
        /// <param name="id">個人リソースシェアリングルールID</param>
        public void Delete(string id)
            => _resourceSharingPersonRepository.Delete(id);

        /// <summary>
        /// 指定されたIDの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="id">個人リソースシェアリングルールID</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public ResourceSharingPersonRuleModel Get(string id)
            => _resourceSharingPersonRepository.Get(id);

        /// <summary>
        /// 指定されたリソースパスでの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">リソースパス</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public IEnumerable<ResourceSharingPersonRuleModel> GetList(string path)
            => _resourceSharingPersonRepository.GetList(path);

        /// <summary>
        /// 個人リソースシェアリングルールを登録します。
        /// </summary>
        /// <param name="model">個人リソースシェアリングルール</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public ResourceSharingPersonRuleModel Register(ResourceSharingPersonRuleModel model)
        {
            if (model.SharingToVendorId != null && !_vendorRepository.IsExists(model.SharingToVendorId.ToString()))
            {
                throw new ForeignKeyException("This SharingFromVendorId does not exist.");
            }
            if (model.SharingToSystemId != null && !_systemRepository.IsSystemExists(model.SharingToSystemId.ToString()))
            {
                throw new ForeignKeyException("This SharingFromSystemId does not exist.");
            }
            return _resourceSharingPersonRepository.Register(model);
        }

        /// <summary>
        /// 個人リソースシェアリングルールを更新します。
        /// </summary>
        /// <param name="model">個人リソースシェアリングルール</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public ResourceSharingPersonRuleModel Update(ResourceSharingPersonRuleModel model)
        {
            if (model.SharingToVendorId != null && !_vendorRepository.IsExists(model.SharingToVendorId.ToString()))
            {
                throw new ForeignKeyException("This SharingFromVendorId does not exist.");
            }
            if (model.SharingToSystemId != null && !_systemRepository.IsSystemExists(model.SharingToSystemId.ToString()))
            {
                throw new ForeignKeyException("This SharingFromSystemId does not exist.");
            }
            return _resourceSharingPersonRepository.Update(model);
        }
    }
}
