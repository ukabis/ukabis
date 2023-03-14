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
    internal class RepositoryGroupService : IRepositoryGroupService
    {
        private IRepositoryGroupRepository RepositoryGroupRepository => _lazyRepositoryGroupRepository.Value;
        private Lazy<IRepositoryGroupRepository> _lazyRepositoryGroupRepository = new(() => UnityCore.Resolve<IRepositoryGroupRepository>());
        private IVendorRepository VendorRepository => _lazyVendorRepository.Value;
        private Lazy<IVendorRepository> _lazyVendorRepository = new(() => UnityCore.Resolve<IVendorRepository>());

#if Oracle
            private IStreamingServiceEventRepository StreamingServiceEventRepository => _lazyStreamingServiceEventRepository.Value;
            private Lazy<IStreamingServiceEventRepository> _lazyStreamingServiceEventRepository = new(() => UnityCore.Resolve<IStreamingServiceEventRepository>("DomainDataSyncOci"));
#else
        private IServiceBusEventRepository ServiceBusEventRepository => _lazyServiceBusEventRepository.Value;
        private Lazy<IServiceBusEventRepository> _lazyServiceBusEventRepository = new(() => UnityCore.Resolve<IServiceBusEventRepository>("DomainDataSync"));
#endif

        public RepositoryGroupModel GetRepositoryGroup(string repositoryGroupId)
        {
            var repositoryGroup = RepositoryGroupRepository.GetRepositoryGroup(repositoryGroupId);
            repositoryGroup.PhysicalRepositoryList = RepositoryGroupRepository.GetPhysicalRepository(repositoryGroup.RepositoryGroupId);
            return repositoryGroup;
        }

        public IList<RepositoryGroupModel> GetRepositoryGroupList()
        {
            return RepositoryGroupRepository.GetRepositoryGroupList();
        }


        public IEnumerable<RepositoryGroupModel> GetRepositoryGroupListIncludePhysicalRepository(string vendorId = null)
        {
            var repositoryGroups = RepositoryGroupRepository.GetRepositoryGroupList(vendorId);
            if(repositoryGroups==null || !repositoryGroups.Any())
            {
                throw new NotFoundException($"Not Found Repository Group List");
            }

            foreach (var r in repositoryGroups)
            {
                r.PhysicalRepositoryList = RepositoryGroupRepository.GetPhysicalRepository(r.RepositoryGroupId);
            }

            return repositoryGroups;
        }

        public IEnumerable<RepositoryTypeModel> GetRepositoryGroupTypeList()
        {
            return RepositoryGroupRepository.GetRepositoryTypeList();
        }

        public RepositoryGroupModel MergeRepositoryGroup(RepositoryGroupModel repositoryGroup)
        {
            repositoryGroup.PhysicalRepositoryList?.RemoveAll(x => !x.IsActive && string.IsNullOrEmpty(x.PhysicalRepositoryId));
            if (string.IsNullOrEmpty(repositoryGroup.RepositoryGroupId)) repositoryGroup.RepositoryGroupId = Guid.NewGuid().ToString();

            repositoryGroup.PhysicalRepositoryList?.ForEach(p =>
            {
                if (string.IsNullOrEmpty(p.PhysicalRepositoryId)) p.PhysicalRepositoryId = Guid.NewGuid().ToString();
            });

            var result = RepositoryGroupRepository.MergeRepositoryGroup(repositoryGroup);

            List<Task> tasks = new()
            {
#if Oracle
                StreamingServiceEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = $"RepositoryGroupSync", PkValue = result.RepositoryGroupId })
#else
                ServiceBusEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = $"RepositoryGroupSync", PkValue = result.RepositoryGroupId })
#endif
            };

            repositoryGroup.PhysicalRepositoryList?.ForEach(repository
                => tasks.Add(
#if Oracle
                    StreamingServiceEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = "PhysicalRepositorySync", PkValue = repository.PhysicalRepositoryId })
#else
                    ServiceBusEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = "PhysicalRepositorySync", PkValue = repository.PhysicalRepositoryId })
#endif
                   ));
            
            Task.WaitAll(tasks.ToArray());

            return result;
        }

        public void DeleteReposigoryGroup(string repositoryGroupId)
        {
            //存在チェック
            var result = RepositoryGroupRepository.GetRepositoryGroup(repositoryGroupId);
            if (result == null)
            {
                throw new NotFoundException();
            }

            var physicalRepositories = RepositoryGroupRepository.GetPhysicalRepository(repositoryGroupId);

            RepositoryGroupRepository.DeleteReposigoryGroup(repositoryGroupId);

            var tasks = new List<Task>();
#if Oracle
            StreamingServiceEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = $"RepositoryGroupSync", PkValue = result.RepositoryGroupId });
#else
            ServiceBusEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = "RepositoryGroupSync", PkValue = repositoryGroupId });
#endif
            foreach(var r in physicalRepositories)
                tasks.Add(
#if Oracle
                    StreamingServiceEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = "PhysicalRepositorySync", PkValue = r.PhysicalRepositoryId })
#else
                    ServiceBusEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = "PhysicalRepositorySync", PkValue = r.PhysicalRepositoryId })
#endif
                );

            Task.WaitAll(tasks.ToArray());
        }

        public void ActivateVendorRepositoryGroup(VendorRepositoryGroupModel vendorRepositoryGroupItem)
        {
            if (VendorRepository.Get(vendorRepositoryGroupItem.VendorId) == null)
            {
                throw new NotFoundException($"VendorId {vendorRepositoryGroupItem.VendorId} does not exist.");
            }
            if (!RepositoryGroupRepository.ExistsRepositoryGroup(vendorRepositoryGroupItem.RepositoryGroupId))
            {
                throw new NotFoundException($"RepositoryGroupId {vendorRepositoryGroupItem.RepositoryGroupId} does not exist.");
            }
            if(CheckUsedVendorRepositoryGroup(vendorRepositoryGroupItem.RepositoryGroupId, vendorRepositoryGroupItem.VendorId) && !vendorRepositoryGroupItem.Active)
            {
                throw new InUseException("指定されたベンダーリポジトリグループはAPIで使用中の為削除できません。");
            }

            RepositoryGroupRepository.ActivateVendorRepositoryGroup(vendorRepositoryGroupItem.VendorId, vendorRepositoryGroupItem.RepositoryGroupId, vendorRepositoryGroupItem.Active);
        }

        public void ActivateVendorRepositoryGroupList(IList<VendorRepositoryGroupModel> model)
        {
            // ベンダーごとに更新する
            foreach (var g in model.GroupBy(x => x.VendorId))
            {
                RepositoryGroupRepository.ActivateVendorRepositoryGroupList(g.Key, g.Select(x => x.RepositoryGroupId).ToList());
            }
            
        }

        public bool CheckUsedVendorRepositoryGroup(string repositoryGroupId, string vendorId)
        {
            return RepositoryGroupRepository.CheckUsedVendorRepositoryGroup(repositoryGroupId, vendorId);
        }

        public VendorRepositoryGroupModel GetVendorRepositoryGroup(string repositoryGroupId, string vendorId)
        {
            return RepositoryGroupRepository.GetVendorRepositoryGroupInfo(vendorId, repositoryGroupId);
        }

        public IList<VendorRepositoryGroupModel> GetVendorRepositoryGroupList()
        {
            return RepositoryGroupRepository.GetVendorRepositoryGroupInfoExList();
        }

        public IList<VendorRepositoryGroupModel> GetVendorRepositoryGroupList(string vendorId)
        {
            return RepositoryGroupRepository.GetVendorRepositoryGroupInfoList(vendorId);
        }
    }
}
