using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class NewDocumentHistoryDataStoreRepository : NewBlobDataStoreRepository, INewDocumentHistoryDataStoreRepository
    {
        public Func<string> DefaultFileName { get; set; }

        public NewDocumentHistoryDataStoreRepository()
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
