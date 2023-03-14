using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/CertificationPortal/Private/CertificationApply", typeof(CertificationApplyModel))]
    public interface ICertificationApplyApi: ICommonResource<CertificationApplyModel>
    {
        [WebApiPost("RegisterAsAccreditation?CertificationApplyId={certificationApplyId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<CertificationApplyModel> RegisterAsAccreditation(CertificationApplyModel model, string certificationApplyId);
        
        [WebApiPatch("UpdateAsAccreditation/{certificationApplyId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<CertificationApplyModel> UpdateAsAccreditation(string certificationApplyId, CertificationApplyModel model);
        
        [WebApiPatch("UpdateAsApplicant/{certificationApplyId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<CertificationApplyModel> UpdateAsApplicant(string certificationApplyId, CertificationApplyModel model);
    }
}
