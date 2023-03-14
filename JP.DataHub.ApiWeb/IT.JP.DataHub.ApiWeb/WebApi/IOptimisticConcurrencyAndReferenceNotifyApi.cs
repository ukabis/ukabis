using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/OptimisticConcurrencyAndReferenceNotify", typeof(ReferenceNotifyModel))]
    public interface IOptimisticConcurrencyAndReferenceNotifyApi : ICommonResource<ReferenceNotifyModel>
    {
        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegisterFirst(ReferenceNotifyFirstModel model);
    }
}
