using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/AttachFileDocumentVersionVendorDependency", typeof(AttachFileBase64Model))]
    public interface IAttachFileDocumentVersionVendorDependencyApi : ICommonResource<AttachFileBase64Model>
    {
    }
}
