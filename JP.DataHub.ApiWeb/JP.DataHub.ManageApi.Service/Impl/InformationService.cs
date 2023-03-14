using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class InformationService : AbstractService, IInformationService
    {
        private Lazy<IInformationRepository> _lazyInformationRepository = new Lazy<IInformationRepository>(() => UnityCore.Resolve<IInformationRepository>());
        private IInformationRepository _informationRepository { get => _lazyInformationRepository.Value; }

        public List<InformationModel> GetList(int? getInformationCount, bool isVisible, bool isVisibleAdmin)
        {
            return _informationRepository.GetList(getInformationCount, isVisible, isVisibleAdmin);
        }

        public InformationModel Get(string informationId)
        {
            return _informationRepository.Get(informationId);
        }
        public InformationModel Registration(InformationModel information)
        {
            return _informationRepository.Registration(information);
        }

        public InformationModel Update(InformationModel information)
        {
            _informationRepository.Update(information);
            return information;
        }

        public void Delete(string informationId)
        {
            _informationRepository.Delete(informationId);
        }
    }
}
