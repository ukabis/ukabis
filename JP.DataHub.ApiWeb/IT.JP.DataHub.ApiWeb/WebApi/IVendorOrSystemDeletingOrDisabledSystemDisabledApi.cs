using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/VenderOrSystemDeletingOrDisabled/SystemDisabled", typeof(string))]
    public interface IVendorOrSystemDeletingOrDisabledSystemDisabledApi : ICommonResource<string>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<string>> GetAll();
    }
}
