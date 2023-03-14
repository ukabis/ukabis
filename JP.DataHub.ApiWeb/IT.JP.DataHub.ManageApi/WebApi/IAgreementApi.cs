using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/Agreement", typeof(AgreementModel))]
    public interface IAgreementApi
    {
        [WebApi("GetAgreement?agreementId={agreementId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AgreementModel> GetAgreement(string agreementId);

        [WebApi("GetAgreementList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AgreementModel>> GetAgreementList();

        [WebApi("GetAgreementListByVendorId?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AgreementModel>> GetAgreementListByVendorId(string vendorId);

        [WebApiPost("RegisterAgreement")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AgreementRegisterResponseModel> RegisterAgreement(AgreementModel model);

        [WebApiPost("UpdateAgreement")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AgreementRegisterResponseModel> UpdateAgreement(AgreementModel model);

        [WebApiDelete("DeleteAgreement?agreementId={agreementId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteAgreement(string agreementId);
    }
}
