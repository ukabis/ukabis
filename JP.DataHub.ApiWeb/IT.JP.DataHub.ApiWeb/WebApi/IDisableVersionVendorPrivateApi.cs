using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/VersionDisable/Vendor", typeof(AreaUnitModel))]
    public interface IDisableVersionVendorPrivateApi : ICommonResource<AreaUnitModel>
    {
    }
}
