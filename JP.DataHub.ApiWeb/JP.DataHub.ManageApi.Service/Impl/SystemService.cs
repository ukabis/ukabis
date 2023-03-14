using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Cryptography;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class SystemService : ISystemService
    {
        private Lazy<ISystemRepository> _lazySystemRepository = new(() => UnityCore.Resolve<ISystemRepository>());
        private ISystemRepository _systemRepository { get => _lazySystemRepository.Value; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterClientModel, UpdateClientModel>()
                    .ForMember(dst => dst.ClientId, ops => ops.MapFrom(src => Guid.NewGuid().ToString()));
                cfg.CreateMap<ClientModel, RegisterClientModel>();
            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        public SystemModel GetSystem(string systemId, SystemItem getItem)
        {
            var result = _systemRepository.GetSystem(systemId);
            if (getItem.HasFlag(SystemItem.Client))
            {
                result.ClientList = _systemRepository.GetClientBySystemId(systemId);
            }
            if (getItem.HasFlag(SystemItem.Link))
            {
                result.SystemLinkList = _systemRepository.GetLinkBySystemId(systemId);
            }
            if (getItem.HasFlag(SystemItem.Admin))
            {
                result.SystemAdmin = _systemRepository.GetAdminBySystemId(systemId);
            }
            return result;
        }

        public IList<SystemModel> GetSystemListByVendorId(string vendorId)
        {
            var result = _systemRepository.GetSystemListByVendorId(vendorId);
            if (result.Any() == false)
            {
                throw new NotFoundException("ベンダーがありません");
            }
            var clientList = _systemRepository.GetClientListBySystemIds(result.Select(s => s.SystemId).ToList());
            result.ForEach(s => s.ClientList = clientList.Where(cl => s.SystemId == cl.SystemId).ToList());
            return result;
        }

        public void DeleteSystem(string systemId)
        {
            _systemRepository.DeleteSystem(systemId);
        }

        public SystemModel UpdateSystem(SystemModel model, SystemItem getItem = SystemItem.All)
        {
            var system = _systemRepository.GetSystem(model.SystemId);
            if (system == null)
            {
                throw new NotFoundException($"Not Found SystemId={model.SystemId}");
            }
            model.VendorId = system.VendorId;
            _systemRepository.UpdateSystem(model);
            return model;
        }

        public SystemModel RegisterSystem(SystemModel model, SystemItem getItem = SystemItem.All)
        {
            _systemRepository.RegisterSystem(model);
            if (getItem.HasFlag(SystemItem.Client) && model.ClientList?.Any() == true)
            {
                var clientActive = model.ClientList.Where(x => x.IsActive == true).FirstOrDefault();
                if (clientActive == null)
                {
                    throw new DomainException("システムクライアントのアクティブは必ず１つで無ければなりません");
                }
                clientActive.SystemId = model.SystemId;
                RegisterClient(s_mapper.Map<RegisterClientModel>(clientActive));
            }
            if (getItem.HasFlag(SystemItem.Link) && model.SystemLinkList?.Any() == true)
            {
                foreach(var systemLink in model.SystemLinkList)
                {
                    systemLink.SystemId = model.SystemId;
                }
                _systemRepository.UpsertSystemLink(model.SystemLinkList);
            }
            if (getItem.HasFlag(SystemItem.Admin) && model.SystemAdmin != null)
            {
                model.SystemAdmin.SystemId = model.SystemId;
                _systemRepository.RegisterAdmin(model.SystemAdmin);
            }
            return model;
        }

        public SystemAdminModel GetAdminBySystemId(string systemId)
            => _systemRepository.GetAdminBySystemId(systemId);

        public SystemAdminModel RegisterAdmin(SystemAdminModel model)
            => _systemRepository.RegisterAdmin(model);

        public void DeleteAdminBySystemId(string systemId)
            => _systemRepository.DeleteAdminBySystemId(systemId);

        public IList<SystemLinkModel> GetSystemLinkListBySystemId(string systemId)
            => _systemRepository.GetSystemLinkListBySystemId(systemId);

        public SystemLinkModel GetSystemLink(string systemLinkId)
            => _systemRepository.GetSystemLink(systemLinkId);

        public IList<SystemLinkModel> UpsertSystemLink(IList<SystemLinkModel> model)
            => _systemRepository.UpsertSystemLink(model);

        public void DeleteSystemLink(string systemLinkId)
            => _systemRepository.DeleteSystemLink(systemLinkId);

        public IList<ClientModel> GetClientListBySystemId(string systemId)
        {
            var result = _systemRepository.GetClientListBySystemId(systemId);
            if (result?.Count == 0)
            {
                throw new NotFoundException();
            }
            return result;
        }

        public ClientModel GetClient(string clientId) 
            => _systemRepository.GetClient(clientId);


        public ClientModel RegisterClient(RegisterClientModel model)
        {
            var tmp = s_mapper.Map<UpdateClientModel>(model);
            tmp.ClientSecret = PasswordHashArgorithm.GetHashedPassword(tmp.ClientSecret, tmp.ClientId.ToString().ToLower());
            return _systemRepository.RegisterClient(tmp);
        }

        public ClientModel UpdateClient(UpdateClientModel model)
        {
            // clientを取得
            var client = _systemRepository.GetClient(model.ClientId);
            if (client == null)
            {
                throw new NotFoundException();
            }
            if (String.IsNullOrEmpty(model.ClientSecret))
            {
                // ClientSecretが変更されない場合、既存のClientSecretを使用
                model.ClientSecret = client.ClientSecret;
            }
            else
            {
                model.ClientSecret = PasswordHashArgorithm.GetHashedPassword(model.ClientSecret, model.ClientId.ToString().ToLower());
            }
            return _systemRepository.UpdateClient(model);
        }

        public void DeleteClient(string clientId)
            => _systemRepository.DeleteClient(clientId);
    }
}
