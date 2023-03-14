using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Unity;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class RoleService : AbstractService, IRoleService
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

        private Lazy<IRoleRepository> _lazyRoleRepository = new Lazy<IRoleRepository>(() => UnityCore.Resolve<IRoleRepository>());
        private IRoleRepository _roleRepository { get => _lazyRoleRepository.Value; }

        public RoleModel GetRole(Guid roleId)
        {
            return _roleRepository.GetRole(roleId);
        }

        public IList<RoleModel> GetRoleList()
        {
            return _roleRepository.GetRoleList();
        }

        public RoleModel RegistrationRole(RoleModel role)
        {
            return _roleRepository.RegistrationRole(role);
        }

        public void UpdateRole(RoleModel role)
        {
            _roleRepository.UpdateRole(role);
        }

        public void DeleteRole(Guid roleId)
        {
            var model = _roleRepository.GetRole(roleId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            _roleRepository.DeleteRole(roleId);
        }
    }
}
