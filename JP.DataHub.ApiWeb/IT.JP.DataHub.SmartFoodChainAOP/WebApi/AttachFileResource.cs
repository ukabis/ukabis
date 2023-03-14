using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    public class AttachFileResource : Resource
    {
        public AttachFileResource()
            : base()
        {
        }

        public AttachFileResource(string serverUrl)
            : base(serverUrl)
        {
        }

        public AttachFileResource(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }
        
        [WebApiPost("CreateAttachFile")]
        public WebApiRequestModel<ResponseFileIdModel> CreateAttachFile(AttachFileCreateModel body) => MakeApiRequestModel<WebApiRequestModel<ResponseFileIdModel>>(new object[]{body});
        
        [WebApiPost("UploadAttachFile/{fileId}")]
        public WebApiRequestModel<string> UploadAttachFile(string fileId,Stream body)=> MakeApiRequestModel<WebApiRequestModel<string>>(new object[]{fileId, body});
 
        [WebApi("GetAttachFile/{fileId}")]
        public WebApiRequestModel<Stream> GetAttachFile(string fileId) => MakeApiRequestModel<WebApiRequestModel<Stream>>(new object[] { fileId });
 
        [WebApi("GetAttachFileFullAccess/{fileId}")]
        public WebApiRequestModel<Stream> GetAttachFileFullAccess(string fileId) => MakeApiRequestModel<WebApiRequestModel<Stream>>(new object[] { fileId });
 
        [WebApiDelete("DeleteAttachFile/{fileId}")]
        public WebApiRequestModel<string> DeleteAttachFile(string fileId) => MakeApiRequestModel<WebApiRequestModel<string>>(new object[] { fileId });
    }
}
