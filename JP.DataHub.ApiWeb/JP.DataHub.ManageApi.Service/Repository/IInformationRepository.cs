using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IInformationRepository
    {
        InformationModel Get(string informationId);

        List<InformationModel> GetList(int? getInformationCount, bool isVisibleApi, bool isVisibleAdmin);

        InformationModel Registration(InformationModel information);

        void Delete(string informationId);

        void Update(InformationModel information);
    }
}
