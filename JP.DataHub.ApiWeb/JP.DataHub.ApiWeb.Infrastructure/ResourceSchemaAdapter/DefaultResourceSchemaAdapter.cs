using JP.DataHub.Com.RFC7807;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.ResourceSchemaAdapter;

namespace JP.DataHub.ApiWeb.Infrastructure.ResourceSchemaAdapter
{
    internal class DefaultResourceSchemaAdapter : IResourceSchemaAdapter
    {
        public bool IsAdaptable(out RFC7807ProblemDetailExtendErrors problemDetail)
        {
            problemDetail = null;
            return true;
        }

        public void Adapt()
        {
            // 何もしない
        }
    }
}
