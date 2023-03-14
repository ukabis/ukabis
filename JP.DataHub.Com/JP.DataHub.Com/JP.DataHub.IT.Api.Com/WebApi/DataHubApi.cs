using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.IT.Api.Com.WebApi
{
    public class DataHubApi<T> : Resource, IDataHubApi<T>
    {
        public DataHubApi()
            : base()
        {
        }
        public DataHubApi(string serverUrl)
            : base(serverUrl)
        {
        }
        public DataHubApi(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }

        #region Default API

        [WebApi("Get/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<T> Get(string PK) => null;

        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<T>> GetList() => null;

        [WebApi("Exists/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Exists(string PK) => null;

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Register(T requestModel) => null;

        [WebApiPost("RegisterList")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<string>> RegisterList(List<T> requestModel) => null;

        [WebApiDelete("Delete/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Delete(string PK) => null;

        [WebApiDelete("DeleteAll")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> DeleteAll() => null;

        [WebApiPatch("Update/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Update(string PK, T requestModel) => null;

        #endregion

        #region Transparent API
        #endregion
    }
}
