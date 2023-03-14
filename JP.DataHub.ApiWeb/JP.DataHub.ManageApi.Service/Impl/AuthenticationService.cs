using JP.DataHub.Com.Cache;
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
using JP.DataHub.ManageApi.Core.DataContainer;
using AutoMapper;
using JP.DataHub.Com.Exceptions;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class AuthenticationService : IAuthenticationService
    {
        private IRoleRepository RoleRepository => _lazyRoleRepository.Value;
        private Lazy<IRoleRepository> _lazyRoleRepository = new(() => UnityCore.Resolve<IRoleRepository>());

        private ICommonIpFilterRepository CommonIpFilterRepository => _lazyCommonIpFilterRepository.Value;
        private Lazy<ICommonIpFilterRepository> _lazyCommonIpFilterRepository = new(() => UnityCore.Resolve<ICommonIpFilterRepository>());

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CommonIpFilterGroupNameModel, CommonIpFilterGroupModel>();
            }).CreateMapper();
        });
        private static IMapper Mapper { get { return s_lazyMapper.Value; } }

        public IList<RoleDetailModel> GetRoleDetail()
        {
            return RoleRepository.GetRoleDetail();
        }

        public IList<RoleDetailModel> GetRoleDetailEx(string openId)
        {
            return RoleRepository.GetRoleDetailEx(openId);
        }

        public IList<CommonIpFilterGroupInfoModel> GetCommonIPFilterList(List<string> commonIpFilterGroupNames)
        {
            if(commonIpFilterGroupNames.Any(a => a == null))
                throw new ArgumentNullException();
            return CommonIpFilterRepository.GetCommonIPFilter(commonIpFilterGroupNames);
        }

        public void DeleteCommonIpFilterGroup(string commonIpFilterGroupId)
        {
            //存在チェック
            var result = CommonIpFilterRepository.GetCommonIpFilterGroup(commonIpFilterGroupId);
            if (result == null)
            {
                throw new NotFoundException();
            }
            CommonIpFilterRepository.DeleteCommonIpFilterGroup(commonIpFilterGroupId);
        }

        public CommonIpFilterGroupModel GetCommonIpFilterGroup(string commonIpFilterGroupId)
        {
            var result = CommonIpFilterRepository.GetCommonIpFilterGroup(commonIpFilterGroupId);
            result.IpList = CommonIpFilterRepository.GetCommonIpFilterList(commonIpFilterGroupId).ToList();
            return result;
        }

        public IList<CommonIpFilterGroupNameModel> GetCommonIpFilterGroupList()
        {
            return CommonIpFilterRepository.GetCommonIpFilterGroupList();
        }

        public IList<CommonIpFilterGroupModel> GetCommonIpFilterGroupListWithIpAddress()
        {
            var result = Mapper.Map<IList<CommonIpFilterGroupModel>>(CommonIpFilterRepository.GetCommonIpFilterGroupList());
            var ttt = CommonIpFilterRepository.GetCommonIpFilterGroups(result);
            return result;
        }

        public CommonIpFilterGroupModel RegistrationCommonIpFilterGroup(CommonIpFilterGroupModel ipFilterGroup)
        {
            return CommonIpFilterRepository.RegistrationCommonIpFilterGroup(ipFilterGroup);
        }

        public CommonIpFilterGroupModel UpdateCommonIpFilterGroup(CommonIpFilterGroupModel ipFilterGroup)
        {
            if (IsExistsIpAddress(ipFilterGroup))
            {
                throw new AlreadyExistsException();
            }
            //IDがないものは新規にIDをふってあげる
            ipFilterGroup.IpList.Where(x => x.CommonIpFilterId == null).ToList().ForEach(x => x.CommonIpFilterId = Guid.NewGuid().ToString());
            return CommonIpFilterRepository.UpdateCommonIpFilterGroup(ipFilterGroup);
        }

        private bool IsExistsIpAddress(CommonIpFilterGroupModel ipFilterGroup)
        {
            var ipList = CommonIpFilterRepository.GetCommonIpFilterList(ipFilterGroup.CommonIpFilterGroupId);

            if (ipList == null || ipList.Count <= 0)
            {
                return false;
            }

            foreach (var commonIpFilter in ipFilterGroup.IpList)
            {
                foreach (var item in ipList)
                {
                    if (commonIpFilter.IpAddress == item.IpAddress &&
                        commonIpFilter.CommonIpFilterId != item.CommonIpFilterId)
                    {
                        // 登録済みデータを新規登録しようとしているためNG
                        return true;
                    }
                }
            }
            return false;
        }
    }
}