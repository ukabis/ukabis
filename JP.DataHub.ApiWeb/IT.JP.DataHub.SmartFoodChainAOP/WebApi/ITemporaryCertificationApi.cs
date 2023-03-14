using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/CertificationPortal/Private/TemporaryCertification", typeof(TemporaryCertificationModel))]
    public interface ITemporaryCertificationApi: ICommonResource<TemporaryCertificationModel>
    {
        [WebApiPost("RegisterAsAccreditation?CertificationApplyId={certificationApplyId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<TemporaryCertificationModel> RegisterAsAccreditation(TemporaryCertificationModel model, string certificationApplyId);
        
    }
}
