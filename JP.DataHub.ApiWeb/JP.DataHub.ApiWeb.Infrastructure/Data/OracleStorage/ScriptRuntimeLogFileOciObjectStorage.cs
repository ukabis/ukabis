using JP.DataHub.ApiWeb.Infrastructure.Configuration;
using Newtonsoft.Json;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.OracleStorage
{
    public class ScriptRuntimeLogFileOciObjectStorage : OciObjectStorage
    {
        public ScriptRuntimeLogFileOciObjectStorage(string connectionString) : base(JsonConvert.DeserializeObject<OciStorageSetting>(connectionString))
        {
        }
    }
}
