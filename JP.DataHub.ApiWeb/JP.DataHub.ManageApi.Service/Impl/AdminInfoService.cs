using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.ManageApi.Service.Attributes;
using JP.DataHub.Com.Exceptions;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class AdminInfoService : AbstractService, IAdminInfoService
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<AdminFuncInfomationModel, AdminFuncInfomationModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper _mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IAdminInfoRepository> _lazyAdminInfoRepository = new Lazy<IAdminInfoRepository>(() => UnityCore.Resolve<IAdminInfoRepository>());
        private Lazy<IRoleRepository> _lazyRoleRepository = new Lazy<IRoleRepository>(() => UnityCore.Resolve<IRoleRepository>());
        private IAdminInfoRepository _adminInfoRepository { get => _lazyAdminInfoRepository.Value; }
        private IRoleRepository _roleRepository { get => _lazyRoleRepository.Value; }

        public AdminFuncInfomationModel RegistrationAdminFuncInfomation(AdminFuncInfomationModel funcInfo)
        {
            return _adminInfoRepository.RegistrationAdminFuncInfomation(funcInfo);
        }

        public AdminFuncInfomationModel GetAdminInfo(string adminFuncRoleId)
        {
            return _adminInfoRepository.GetAdminInfo(adminFuncRoleId);
        }

        public IList<AdminFuncRoleInfomationModel> GetAdminFuncRoleInfomationList()
        {
            var roleList = _roleRepository.GetRoleList();
            var adminFuncInfoList = _roleRepository.GetAdminFuncInfomationList();

            var result = roleList
                .Select(role => new AdminFuncRoleInfomationModel
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    AdminFuncInfoList = adminFuncInfoList
                }
                ).ToList();

            if (result == null || result.Count == 0)
            {
                throw new NotFoundException();
            }

            return result;
        }

        public void UpdateAdminFuncInfo(AdminFuncInfomationModel funcInfo)
        {
            _adminInfoRepository.UpdateAdminFuncInfo(funcInfo);
        }

        public void DeleteAdminInfo(string adminFuncRoleId)
        {
            var model = _adminInfoRepository.GetAdminInfo(adminFuncRoleId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            _adminInfoRepository.DeleteAdminInfo(adminFuncRoleId);
        }
    }
}
