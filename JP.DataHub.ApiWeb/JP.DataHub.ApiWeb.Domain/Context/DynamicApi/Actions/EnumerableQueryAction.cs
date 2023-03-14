using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class EnumerableQueryAction : AbstractDynamicApiAction, IEntity
    {
        public override HttpResponseMessage ExecuteAction()
        {
            return new HttpResponseMessage()
            {
                Content = new EnumerableQueryContent(GetResponseMessage())
            };
        }

        private IEnumerable<string> GetResponseMessage()
        {
            foreach (var repository in this.DynamicApiDataStoreRepository)
            {
                var count = 0;

                var text = GetRepositoryData(repository);
                foreach (var result in text)
                {
                    count++;
                    yield return result;
                }
                if (count > 0)
                {
                    yield break;
                }
            }
        }

        private IEnumerable<string> GetRepositoryData(INewDynamicApiDataStoreRepository repository)
        {
            IEnumerable<JsonDocument> result = null;
            if (!string.IsNullOrEmpty(this.RepositoryKey.Value) || !string.IsNullOrEmpty(this.ApiQuery.Value))
            {
                result = repository.QueryEnumerable(ValueObjectUtil.Create<QueryParam>(this));
            }
            if (result == null || !result.Any())
            {
                return new List<string>();
            }
            return result.Select(x => x.RemoveTokenToJson(XGetInnerAllField.Value, GetRemoveIgnoreFields()));
        }
    }
}
