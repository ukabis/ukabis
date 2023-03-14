using System.Collections.Generic;
using System.IO;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/AttachFile", typeof(AreaUnitModel))]
    public interface IStaticApiAttachFileApi : ICommonResource<AreaUnitModel>
    {
        [WebApiPost("CreateFile")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CreateAttachFileResponseModel> CreateFile(CreateAttachFileRequestModel data);

        [WebApiPost("UploadFile?FileId={fileId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel UploadFile(Stream data, string fileId);

        [WebApi("GetImage?FileId={fileId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetImage(string fileId);

        [WebApi("GetMeta?FileId={fileId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GetAttachFileResponseModel> GetMeta(string fileId);

        [WebApi("MetaQuery?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<GetAttachFileResponseModel>> MetaQuery(string querystring);

        [WebApiDelete("Delete?FileId={fileId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<GetAttachFileResponseModel>> DeleteFile(string fileId);
    }
}
