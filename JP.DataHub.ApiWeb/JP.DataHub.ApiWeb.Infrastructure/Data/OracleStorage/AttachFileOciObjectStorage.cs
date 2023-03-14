using JP.DataHub.ApiWeb.Infrastructure.Configuration;
using Newtonsoft.Json;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.OracleStorage
{
    public class AttachFileOciObjectStorage : OciObjectStorage
    {
        public AttachFileOciObjectStorage(string connectionString) : base(JsonConvert.DeserializeObject<OciStorageSetting>(connectionString))
        {
        }
    }
}
