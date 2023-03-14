using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/", typeof(AdminInfoModel))]
    public interface IMetaDataApi
    {
        [WebApi("$metadata")]
        [AutoGenerateReturnModel]
        WebApiRequestModel MetaData();

    }
}
