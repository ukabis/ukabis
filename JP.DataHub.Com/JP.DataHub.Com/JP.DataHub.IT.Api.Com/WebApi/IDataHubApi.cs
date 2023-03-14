using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;

namespace JP.DataHub.IT.Api.Com.WebApi
{
    public interface IDataHubApi<T>
    {
        [WebApi("Get/{PK}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<T> Get(string PK);

        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<T>> GetList();

        [WebApi("Exists/{PK}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> Exists(string PK);

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> Register(T requestModel);

        [WebApiPost("RegisterList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<string>> RegisterList(List<T> requestModel);

        [WebApiDelete("Delete/{PK}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> Delete(string PK);

        [WebApiDelete("DeleteAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> DeleteAll();

        [WebApiPatch("Update/{PK}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> Update(string PK, T requestModel);
    }
}
