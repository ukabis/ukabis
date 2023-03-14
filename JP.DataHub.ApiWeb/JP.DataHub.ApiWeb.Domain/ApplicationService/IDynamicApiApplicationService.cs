using System.ComponentModel.DataAnnotations;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    [Log]
    internal interface IDynamicApiApplicationService
    {
        DynamicApiResponse Request([Required] HttpMethodType httpMethodType, [Required] RequestRelativeUri relativeUri, [Required] Contents contents, QueryString queryString, HttpHeader header, MediaType mediaType, Accept accept, ContentRange contentRange, ContentType contentType, ContentLength contentLength, NotAuthentication? notAuthentication = null);

        AsyncApiResult GetStatus(AsyncRequestId requestId);
        DynamicApiResponse GetResult(AsyncRequestId requestId);

        string GetControllerSchemaByUrl(string url);
        string GetSchemaModelByName(string name);

        bool SetResult(Stream content, string blobPath, string accept);
        bool SetResultOverwrite(Stream content, string blobPath, string accept);
        bool SetResultOverwrite(string content, string blobPath, string accept);
    }
}
