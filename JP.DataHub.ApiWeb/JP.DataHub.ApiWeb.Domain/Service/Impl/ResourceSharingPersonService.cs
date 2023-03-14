using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Service.Impl
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
        /// 指定されたリソースパスの指定されたユーザIDからの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">リソースパス</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public IEnumerable<ResourceSharingPersonRuleModel> GetFromList(string path, string userId)
            => _resourceSharingPersonRepository.GetFromList(path, userId);

        /// <summary>
        /// 指定されたリソースパスの指定されたユーザIDへの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">リソースパス</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public IEnumerable<ResourceSharingPersonRuleModel> GetToList(string path, string userId)
            => _resourceSharingPersonRepository.GetToList(path, userId);

        /// <summary>
        /// 個人リソースシェアリングルールを登録します。
        /// </summary>
        /// <param name="model">個人リソースシェアリングルール</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public ResourceSharingPersonRuleModel Register(ResourceSharingPersonRuleModel model)
        {
            if ((model.SharingToVendorId != null || model.SharingToSystemId != null) &&
                !_resourceSharingPersonRepository.IsVendorSystemExists(model.SharingToVendorId.ToString(), model.SharingToSystemId.ToString()))
            {
                throw new ForeignKeyException("This SharingToVendorId/SharingToSystemId does not exist.");
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
            if ((model.SharingToVendorId != null || model.SharingToSystemId != null) &&
                !_resourceSharingPersonRepository.IsVendorSystemExists(model.SharingToVendorId.ToString(), model.SharingToSystemId.ToString()))
            {
                throw new ForeignKeyException("This SharingToVendorId/SharingToSystemId does not exist.");
            }
            return _resourceSharingPersonRepository.Update(model);
        }
    }
}
