using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/CertificationPortal/Private/CertificationIssueHistory", typeof(CertificationIssueHistoryModel))]
    public interface ICertificationIssueHistoryApi: ICommonResource<CertificationIssueHistoryModel>
    {
        [WebApiPost("RegisterAsAccreditation?CertificationApplyId={certificationApplyId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<CertificationIssueHistoryModel> RegisterAsAccreditation(CertificationIssueHistoryModel model, string certificationApplyId);
    }
}
