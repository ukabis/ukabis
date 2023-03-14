using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/Information", typeof(InformationModel))]
    public interface IInformationApi 
    {
        [WebApi("GetInformation?informationId={informationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<InformationModel> GetInformation(string informationId);

        [WebApi("GetInformationList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<InformationModel>> GetInformationList();

        [WebApiPost("RegisterInformation")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<InformationModel> RegisterInformation(InformationModel model);

        [WebApiPost("UpdateInformation")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<InformationModel> UpdateInformation(InformationModel model);

        [WebApiDelete("DeleteInformation?informationId={informationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteInformation(string informationId);
    }
}
