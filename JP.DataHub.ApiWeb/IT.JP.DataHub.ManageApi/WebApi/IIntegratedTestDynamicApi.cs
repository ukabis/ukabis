using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/API/IntegratedTest", typeof(AdminInfoModel))]
    public interface IIntegratedTestDynamicApi
    {
        [WebApi("ManageDynamicApi/CacheDeleteCheck/GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AdminInfoModel> ManageDynamicApiCacheDeleteCheckGetAll();


    }
}
