using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    public interface IDynamicApiInterface
    {
        DynamicApiResponse Request(DynamicApiRequestModel request, bool notAuthentication = false);

        HttpResponseMessage Get(DynamicApiRequestModel request, bool notAuthentication = false);

        HttpResponseMessage Post(DynamicApiRequestModel request, bool notAuthentication = false);

        HttpResponseMessage Put(DynamicApiRequestModel request, bool notAuthentication = false);

        HttpResponseMessage Delete(DynamicApiRequestModel request, bool notAuthentication = false);

        HttpResponseMessage Patch(DynamicApiRequestModel request, bool notAuthentication = false);

        AsyncApiResultModel GetStatus(string requestId);
        DynamicApiResponse GetResult(string requestId);

        string GetControllerSchemaByUrl(string url);
        string GetSchemaModelByName(string name);


        bool SetResult(Stream content, string blobPath, string accept = null);
        bool SetResult(string content, string blobPath, string accept = null);
        bool SetResultOverwrite(Stream content, string blobPath, string accept = null);
        bool SetResultOverwrite(string content, string blobPath, string accept = null);
    }
}