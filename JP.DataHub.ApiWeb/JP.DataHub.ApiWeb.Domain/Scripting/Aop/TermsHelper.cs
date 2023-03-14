using AutoMapper;
using JP.DataHub.Com.Unity;
using JP.DataHub.Aop;
using JP.DataHub.ApiWeb.Domain.Service;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Aop
{
    internal class TermsHelper : ITermsHelper
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Service.Model.TermsModel, DataHub.Aop.Models.TermsModel>();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<ITermsService> _lazyTermsService = new Lazy<ITermsService>(() => UnityCore.Resolve<ITermsService>());
        private ITermsService _termsService { get => _lazyTermsService.Value; }


        public List<DataHub.Aop.Models.TermsModel> TermsGetList()
        {
            var list = _termsService.TermsGetList();
            return s_mapper.Map<List<DataHub.Aop.Models.TermsModel>>(list);
        }

        public void Agreement(string openId, string terms_id)
        {
            _termsService.Agreement(openId, terms_id);
        }
    }
}