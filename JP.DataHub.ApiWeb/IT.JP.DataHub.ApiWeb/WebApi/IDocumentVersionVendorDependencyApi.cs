using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/DocumentVersionVendorDependency", typeof(AreaUnitModel))]
    public interface IDocumentVersionVendorDependencyApi : ICommonResource<AreaUnitModel>
    {
    }
}
