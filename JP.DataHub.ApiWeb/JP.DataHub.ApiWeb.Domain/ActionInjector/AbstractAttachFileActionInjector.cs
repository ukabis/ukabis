using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal abstract class AbstractAttachFileActionInjector : ActionInjector
    {
        protected const string MEDIATYPE_JSON = "application/json";
        protected const string CREDENTIAL_MASK = "***";
        protected readonly int MetaRepositoryIndex = 0;
        protected readonly int FileRepositoryIndex = 1;

        protected static DataSchemaId AttachFileMetaDataSchemaId = new DataSchemaId(UnityCore.Resolve<string>("AttachFileMetaDataSchemaId"));

        protected string GetSelectFiledList(bool withAdminFields = false, INewDynamicApiDataStoreRepository repository = null)
        {
            if (withAdminFields)
            {
                // 管理項目と外部添付ファイル情報を含める
                // 以下の項目は除外
                //   id      : データのIDと紛らわしいため除外
                //   FilePath: 内部情報のため除外
                var fields = "\"FileId\",\"FileName\",\"ContentType\",\"FileLength\",\"IsDrm\",\"DrmType\",\"DrmKey\",\"IsUploaded\",\"MetaList\",\"IsExternalAttachFile\",\"ExternalAttachFile\"";
                if (repository?.AttachFileMetaManagementFields?.Any() == true)
                {
                    fields += $",{string.Join(",", repository.AttachFileMetaManagementFields)}";
                }

                return fields;
            }
            else
            {
                return "\"FileId\",\"FileName\",\"ContentType\",\"FileLength\",\"IsDrm\",\"DrmType\",\"DrmKey\",\"IsUploaded\",\"MetaList\"";
            }
        }

        protected FileId GetQueryStringToFileId()
        {
            var target = Target as AbstractDynamicApiAction;
            var key = new QueryStringKey(nameof(DynamicApiAttachFileInformation.FileId));
            if (target.Query.Dic.ContainsKey(key))
            {
                Guid fileId;
                if (Guid.TryParse(target.Query.Dic[key].Value, out fileId))
                {
                    return new FileId(fileId);
                }
            }
            return null;
        }
    }
}
