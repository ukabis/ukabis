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
using JP.DataHub.ManageApi.Core.DataContainer;
using JP.DataHub.Com.Exceptions;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class AgreementService : AbstractService, IAgreementService
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

        private Lazy<IAgreementRepository> _lazyAgreementRepository = new Lazy<IAgreementRepository>(() => UnityCore.Resolve<IAgreementRepository>());
        private IAgreementRepository _agreementRepository { get => _lazyAgreementRepository.Value; }

        List<AgreementModel> IAgreementService.GetAgreementList(string? vendorId = null)
        {
            return _agreementRepository.GetAgreementList(vendorId);
        }
        AgreementModel IAgreementService.GetAgreement(string agreementId)
        {
            return _agreementRepository.GetAgreement(agreementId);
        }

        AgreementModel IAgreementService.RegistAgreement(AgreementModel agreement)
        {
            if (string.IsNullOrEmpty(agreement.AgreementId))
            {
                agreement.AgreementId = Guid.NewGuid().ToString();
            }
            return _agreementRepository.RegistAgreement(agreement);
        }

        AgreementModel IAgreementService.UpdateAgreement(AgreementModel agreement)
        {
            return _agreementRepository.UpdateAgreement(agreement);
        }

        void IAgreementService.DeleteAgreement(string agreementId)
        {
            var model = _agreementRepository.GetAgreement(agreementId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            _agreementRepository.DeleteAgreement(agreementId);
        }
    }
}
