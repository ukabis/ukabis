using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    // .NET6
    internal interface INewBlobDataStoreRepository : INewDynamicApiDataStoreRepository
    {
        Func<Dictionary<string, string>, JToken, string, string> DefaultContainerFormat { get; set; }
        Func<Dictionary<string, string>, JToken, string, string> DefaultFileNameFormat { get; set; }
        Func<Tuple<Guid?, Guid?>> DefaultRepositoryIds { get; set; }
        Func<Encoding> DefaultEncoding { get; set; }
        Stream QueryToStream(QueryParam queryParam);
        Uri CopyFile(string destBlobName, string destContainerName, string srcBlobName, string srcContainerName);
        Uri GetUriWithSharedAccessSignature(QueryParam query);
    }
}
