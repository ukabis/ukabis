using System.Linq;
using Unity;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Attributes;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    internal abstract class NewAbstractDynamicApiDataStoreRepository : INewDynamicApiDataStoreRepository
    {
        public virtual bool CanOptimisticConcurrency { get => false; }
        public virtual bool CanQuery { get => false; }
        public virtual bool CanVersionControl { get => true; }
        public virtual bool CanQueryAttachFileMetaByOData { get => false; }
        public virtual ODataPatchSupport ODataPatchSupport => 
            (RepositoryInfo.Type.GetType().GetField(RepositoryInfo.Type.ToString()).GetCustomAttributes(false)
                .FirstOrDefault(y => y is ODataPatchSupportedAttribute) as ODataPatchSupportedAttribute)?.ODataPatchSupport ?? ODataPatchSupport.None;
        public virtual string VersionInfoQuery { get; }
        public virtual string DocumentVersionQuery => throw new NotImplementedException();
        public virtual string RepositoryName { get; }
        public virtual IEnumerable<string> AttachFileMetaManagementFields => throw new NotImplementedException();

        public RepositoryInfo RepositoryInfo { get; set; }

        public RepositoryKeyInfo RepositoryKeyInfo { get => new RepositoryKeyInfo(RepositoryInfo.RepositoryGroupId, RepositoryInfo.PhysicalRepositoryInfoList.Where(x => x.Isfull == false).Select(x => x.PhysicalRepositoryId).FirstOrDefault()); }

        [Dependency]
        public IPerRequestDataContainer PerRequestDataContainer { get; set; }
        //[Dependency]
        public IResourceVersionRepository ResourceVersionRepository { get; set; }
        public IDocumentVersionRepository DocumentVersionRepository { get; set; }
        private Lazy<IKeyManagement> keyManagement => new Lazy<IKeyManagement>(() => UnityCore.Resolve<IKeyManagement>(this.RepositoryInfo.Type.ToCode()));
        public IKeyManagement KeyManagement { get => keyManagement.Value; }

        public NewAbstractDynamicApiDataStoreRepository()
        {
        }

        public T QueryOnce<T>(QueryParam param)
        {
            var doc = QueryOnce(param);
            return doc?.Value != null ? doc.Value.ToObject<T>() : default(T);
        }
        abstract public IEnumerable<JsonDocument> QueryEnumerable(QueryParam param);
        abstract public IList<JsonDocument> Query(QueryParam param, out XResponseContinuation XResponseContinuation);
        abstract public JsonDocument QueryOnce(QueryParam param);

        abstract public RegisterOnceResult RegisterOnce(RegisterParam param);
        abstract public void DeleteOnce(DeleteParam param);
        abstract public IEnumerable<string> Delete(DeleteParam param);
        virtual public int ODataPatch(QueryParam queryParam, JToken updateData) => throw new NotImplementedException();

        virtual public string GetInternalAddWhereString(QueryParam param, out IDictionary<string, object> additionalParameters) => throw new NotImplementedException();

        private Lazy<bool> xRequestContinuationNeedsTopCount => new Lazy<bool>(() => UnityCore.Resolve<bool>("XRequestContinuationNeedsTopCount"));
        public bool XRequestContinuationNeedsTopCount { get => xRequestContinuationNeedsTopCount.Value; }
    }
}
