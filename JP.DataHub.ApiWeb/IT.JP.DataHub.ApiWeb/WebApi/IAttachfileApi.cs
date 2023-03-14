using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/Attachfile", typeof(AutoKey3DataModel))]
    public interface IAttachfileApi : ICommonResource<AutoKey3DataModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AutoKey3DataModel>> GetAll();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AutoKey3DataModel model);


        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AttachFileBase64Model>> GetAllBase64();

        [WebApi("GetAllCache")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AttachFileBase64Model>> GetAllCache();

        [WebApi("OData?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AttachFileBase64Model>> ODataBase64(string querystring = null);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistBase64(AttachFileBase64Model model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistListBase64(List<AttachFileBase64Model> model);
    }
}
