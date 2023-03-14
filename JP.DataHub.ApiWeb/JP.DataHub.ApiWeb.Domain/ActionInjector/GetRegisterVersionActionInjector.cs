using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Consts;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal class GetRegisterVersionActionInjector : ActionInjector
    {
        public override void Execute(Action action)
        {
            var target = Target as AbstractDynamicApiAction;

            var repository = target.DynamicApiDataStoreRepository.First();
            var versionInfoStr = repository.ResourceVersionRepository.GetVersionInfo(target.RepositoryKey).ToString();

            var version = Newtonsoft.Json.JsonConvert.DeserializeObject<VersionInfo>(versionInfoStr, new Newtonsoft.Json.JsonSerializerSettings { FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Decimal });
            int max = version.DocumentVersions.Max(x => x.Version);
            // カレントバージョンとバージョンの最大値が同じ場合は登録用のバージョンは存在しないのでゼロを、そうでない場合は、maxバージョンは登録用なのでそれを返す。
            int registerVersion = version.CurrentVersion == max ? 0 : max;
            JToken tokenRegisterVersion = JToken.FromObject($"{{ \"RegisterVersion\" : {registerVersion} }}");
            ReturnValue = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tokenRegisterVersion.ToString(), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
        }
    }
}
