using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/TermsOn", typeof(AreaUnitModel))]
    public interface ITermasApiOn : ICommonResource<AreaUnitModel>
    {
    }
    [WebApiResource("/API/IntegratedTest/TermsOff", typeof(AreaUnitModel))]
    public interface ITermasApiOff : ICommonResource<AreaUnitModel>
    {
    }
    [WebApiResource("/API/IntegratedTest/TermsTest", typeof(AreaUnitModel))]
    public interface ITermasApiTest : ICommonResource<AreaUnitModel>
    {
    }
}
