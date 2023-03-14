using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.Api.Core.Exceptions;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class ResourceSharingService : IResourceSharingService
    {
        private IResourceSharingRepository ResourceSharingRepository => _lazyResourceSharingRepository.Value;
        private Lazy<IResourceSharingRepository> _lazyResourceSharingRepository = new(() => UnityCore.Resolve<IResourceSharingRepository>());

        private IVendorRepository _vendorRepository => _lazyVendorRepository.Value;
        private Lazy<IVendorRepository> _lazyVendorRepository = new(() => UnityCore.Resolve<IVendorRepository>());
        private ISystemRepository _systemRepository => _lazySystemRepository.Value;
        private Lazy<ISystemRepository> _lazySystemRepository = new(() => UnityCore.Resolve<ISystemRepository>());
        private IDynamicApiRepository _dynamicApiRepository => _lazyDynamicApiRepository.Value;
        private Lazy<IDynamicApiRepository> _lazyDynamicApiRepository = new(() => UnityCore.Resolve<IDynamicApiRepository>());

        /// <summary>
        /// 共有設定の取得
        /// </summary>
        /// <returns></returns>
        public ResourceSharingRuleModel GetResourceSharingRule(string resourceSharingRuleId) 
        {
            return ResourceSharingRepository.GetResourceSharingRule(resourceSharingRuleId);
        }

        /// <summary>
        /// 共有設定ルールの取得
        /// </summary>
        /// <returns></returns>
        public IList<ResourceSharingRuleModel> GetResourceSharingRuleList(string controllerId, string sharingFromVendorId, string sharingFromSystemId) 
        {
            return ResourceSharingRepository.GetResourceSharingRuleList(controllerId, sharingFromVendorId, sharingFromSystemId);
        }

        /// <summary>
        /// 共有設定新規作成
        /// </summary>
        /// <returns></returns>
        public ResourceSharingRuleModel RegisterResourceSharingRule(ResourceSharingRuleModel resourceSharingRule)
        {
            return MargeResourceSharingRule(resourceSharingRule);
        }

        /// <summary>
        /// 共有設定更新
        /// </summary>
        /// <returns></returns>
        public void UpdateResourceSharingRule(ResourceSharingRuleModel resourceSharingRule)
        {
            if (ResourceSharingRepository.GetResourceSharingRule(resourceSharingRule.ResourceSharingRuleId)?.ResourceSharingRuleId == null)
            {
                throw new NotFoundException($"Not Found ResourceSharingRule. ResourceSharingRuleId = {resourceSharingRule.ResourceSharingRuleId}");
            }
            MargeResourceSharingRule(resourceSharingRule);
        }

        private ResourceSharingRuleModel MargeResourceSharingRule(ResourceSharingRuleModel resourceSharingRule)
        {
            if(!ExistsApiInVendor(resourceSharingRule.ApiId, resourceSharingRule.SharingFromVendorId))
            {
                throw new ForeignKeyException("This ApiId does not exist.");
            }
            //存在チェック
            if (!_vendorRepository.IsExists(resourceSharingRule.SharingFromVendorId))
            {
                throw new ForeignKeyException("This SharingFromVendorId does not exist.");
            }
            if (!_vendorRepository.IsExists(resourceSharingRule.SharingToVendorId))
            {
                throw new ForeignKeyException("This SharingToVendorId does not exist.");
            }
            if (!_systemRepository.IsSystemExists(resourceSharingRule.SharingFromSystemId))
            {
                throw new ForeignKeyException("This SharingFromSystemId does not exist.");
            }
            if (!_systemRepository.IsSystemExists(resourceSharingRule.SharingToSystemId))
            {
                throw new ForeignKeyException("This SharingToSystemId does not exist.");
            }

            return ResourceSharingRepository.MargeResourceSharingRule(resourceSharingRule);
        }

        /// <summary>
        /// 共有設定削除
        /// </summary>
        /// <returns></returns>
        public void DeleteResourceSharingRule(string resourceSharingRuleId)
        {
            //存在チェック
            var result = ResourceSharingRepository.GetResourceSharingRule(resourceSharingRuleId);
            if(result == null)
            {
                throw new NotFoundException();
            }
            ResourceSharingRepository.DeleteResourceSharingRule(resourceSharingRuleId, result.ApiId);
        }

        private bool ExistsApiInVendor(string apiId, string vendorId)
        {
            var api = _dynamicApiRepository.GetController(apiId);
            return (api == null) ? false : api.VendorId.ToLower() == vendorId.ToLower();
        }
    }
}
