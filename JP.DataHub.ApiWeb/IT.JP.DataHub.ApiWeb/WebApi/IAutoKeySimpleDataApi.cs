using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/AutoKeySimpleData", typeof(AreaUnitModel))]
    public interface IAutoKeySimpleDataApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApi("Get?id={id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetById(string id);

        [WebApi("Get?AreaUnitCode={areaUnitCode}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByAreaUnitCode(string areaUnitCode);

        [WebApi("Get?AreaUnitCode={areaUnitCode}&AreaUnitName={areaUnitName}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByAreaUnitCodeAndName(string areaUnitCode, string areaUnitName);

        [WebApi("Get?AreaUnitName={areaUnitName}&AreaUnitCode={areaUnitCode}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByAreaUnitNameAndCode(string areaUnitName, string areaUnitCode);

        [WebApi("Get/{areaUnitCode}?AreaUnitName={areaUnitName}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByUrlParamAndQueryString(string areaUnitCode, string areaUnitName);

        [WebApi("Get/{areaUnitCode}/{areaUnitName}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByUrlParam2layer(string areaUnitCode, string areaUnitName);

        [WebApi("Get?AreaUnitCode2={areaUnitCode2}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetWithInvalidQueryString(string areaUnitCode2);

        [WebApi("Get?AreaUnitCode={areaUnitCode}&AreaUnitCode2={areaUnitCode2}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetWithInvalidQueryString2(string areaUnitCode, string areaUnitCode2);

        [WebApi("Get/hoge/fuga/hoge")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByInvalidPath();

        [WebApi("GetByODataQuery?AreaUnitCode={areaUnitCode}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByODataQuery(string areaUnitCode);

        [WebApi("GetByODataQueryOverMeters1000?AreaUnitName={areaUnitName}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByODataQueryOverMeters1000(string areaUnitName);

        [WebApi("GetAdditionalPropertiesFalse?AreaUnitCode={areaUnitCode}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetAdditionalPropertiesFalse(string areaUnitCode);

        [WebApi("OData?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<int>> ODataCount(string querystring);

        [WebApi("OData?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModelEx>> ODataEx(string querystring);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModel model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<AreaUnitModel> model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistListEx(List<AreaUnitModelEx> model);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistEmptyString();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistEmptyObject(AreaUnitModel model);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistEmptyArray(List<AreaUnitModel> model);
    }
}
