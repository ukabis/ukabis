using JP.DataHub.Batch.DomainDataSync.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;

namespace JP.DataHub.Batch.DomainDataSync.Domain
{
    [WebApiResource("/Manage/Cache", typeof(DomainDataSyncDeleteCacheResponseModel))]
    public interface IDomainDataSyncResource : ICommonResource<DomainDataSyncDeleteCacheResponseModel>
    {
        [WebApiDelete("ClearById?cacheId={cacheId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel ClearById(string cacheId);

        [WebApiDelete("ClearByEntity?entity={entity}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel ClearByEntity(string entity);
    }
}
