using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.ResourceSchemaAdapter;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class AdaptResourceSchemaAction : AbstractDynamicApiAction, IAdaptResourceSchemaAction, IEntity
    {
        public ReadOnlyCollection<RepositoryInfo> RepositoryInfoList { get; set; }

        public override HttpResponseMessage ExecuteAction()
        {
            // リソース・APIの全リポジトリのアダプタを生成
            var adapters = RepositoryInfoList
                .Distinct(x => x.PhysicalRepositoryId.Value.ToLower())
                .Select(x => ResourceSchemaAdapterFactory.CreateResourceSchemaAdapter(x, this.ControllerId, this.ControllerSchema)).ToList();

            // リポジトリごとにリソーススキーマの適用可否を判定
            foreach (var adapter in adapters)
            {
                if (!adapter.IsAdaptable(out var rfc7807))
                {
                    return TupleToHttpResponseMessage(new Tuple<HttpStatusCode, string>(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(rfc7807)));
                }
            }

            // リポジトリごとにリソーススキーマを適用
            adapters.ForEach(x => x.Adapt());

            return TupleToHttpResponseMessage(new Tuple<HttpStatusCode, string>(HttpStatusCode.OK, null));
        }
    }
}
