using JP.DataHub.Com.RFC7807;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.ResourceSchemaAdapter
{
    // .NET6
    internal interface IResourceSchemaAdapter
    {
        bool IsAdaptable(out RFC7807ProblemDetailExtendErrors problemDetail);
        void Adapt();
    }
}
