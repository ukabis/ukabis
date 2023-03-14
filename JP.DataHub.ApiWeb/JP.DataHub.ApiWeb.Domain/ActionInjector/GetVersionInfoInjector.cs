using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.Consts;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal class GetVersionInfoInjector : ActionInjector
    {
        public override void Execute(Action action)
        {
            var target = Target as AbstractDynamicApiAction;
            var repository = target.DynamicApiDataStoreRepository.First();
            var versionInfoStr = repository.ResourceVersionRepository.GetVersionInfo(target.RepositoryKey)?.ToString();

            var version = JsonConvert.DeserializeObject<VersionInfo>(versionInfoStr, new JsonSerializerSettings { FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Decimal });
            if (version?.CurrentVersion == 0)
            {
                // カレントバージョンがないのはデータが古い。正しいデータに修正をする
                target.DynamicApiDataStoreRepository.ToList().ForEach(x =>
                {
                    try
                    {
                        x.ResourceVersionRepository.RefreshVersion(target.RepositoryKey);
                    }
                    catch (NotImplementedException)
                    {
                        // ignore
                    }
                });
            }
            versionInfoStr = JsonConvert.SerializeObject(version);
            ReturnValue = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(versionInfoStr, Encoding.UTF8, MediaTypeConst.ApplicationJson) };
        }
    }
}
