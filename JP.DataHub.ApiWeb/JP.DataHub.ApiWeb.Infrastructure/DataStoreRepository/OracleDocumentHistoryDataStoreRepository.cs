using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    // .NET6
    internal class OracleDocumentHistoryDataStoreRepository : OciObjectStorageDataStoreRepository, INewBlobDataStoreRepository
    {
        public Func<string> DefaultFileName { get; set; }


        public OracleDocumentHistoryDataStoreRepository()
        {
            DefaultFileNameFormat = (Dictionary<string, string> dic, JToken sourceJson, string filename) =>
            {
                var xx = filename.Split('/');
                xx[xx.Length - 1] = DefaultFileName != null ? DefaultFileName() : Guid.NewGuid().ToString();
                return string.Join("/", xx);
            };
        }
    }
}