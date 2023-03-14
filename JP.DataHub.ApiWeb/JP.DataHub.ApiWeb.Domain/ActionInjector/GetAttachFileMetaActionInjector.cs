using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal class GetAttachFileMetaActionInjector : AbstractAttachFileActionInjector
    {
        public override void Execute(Action action)
        {
            var target = Target as AbstractDynamicApiAction;
            if (!target.IsEnableAttachFile.Value)
            {
                ReturnValue = target.TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotImplemented, null));
                return;
            }
            FileId fileid = GetQueryStringToFileId();
            if (fileid == null)
            {
                ReturnValue = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return;
            }

            // OData用にQuery上書き
            var withAdminFields = (target.XGetInnerAllField?.Value == true);
            var selectFields = GetSelectFiledList(withAdminFields, target.DynamicApiDataStoreRepository[0]);
            var odataParam = new Dictionary<QueryStringKey, QueryStringValue>();
            odataParam.Add(new QueryStringKey("$filter"), new QueryStringValue($"FileId eq {target.Query.Dic[new QueryStringKey("FileId")].Value}"));
            odataParam.Add(new QueryStringKey("$select"), new QueryStringValue(selectFields));
            target.Query = new QueryStringVO(odataParam);

            // RDBMSリポジトリの場合はメタ情報用のテーブルへのアクセスとなるため、
            // 添付ファイルの処理であることを通知するフラグをONにする
            // また、リソースモデルをメタ情報のモデルに差し替える
            target.OperationInfo = new OperationInfo(OperationInfo.OperationType.AttachFileMeta);
            target.ControllerSchema = UnityCore.Resolve<IDynamicApiRepository>().GetSchemaModelById(AttachFileMetaDataSchemaId);

            action();

            // 管理項目ありの場合はExternalAttachFile.Credentialsをマスクして返却する
            var actionResult = (HttpResponseMessage)ReturnValue;
            if (actionResult.IsSuccessStatusCode && withAdminFields)
            {
                var meta = actionResult.Content.ReadAsStringAsync().Result.ToJson();
                if (meta.IsExistProperty(nameof(DynamicApiAttachFileInformation.ExternalAttachFile)) &&
                    (meta[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)]?.Type ?? JTokenType.Null) != JTokenType.Null)
                {
                    for (var i = 0; i < meta[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)][nameof(ExternalAttachFileInfomation.Credentials)].Count(); i++)
                    {
                        meta[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)][nameof(ExternalAttachFileInfomation.Credentials)][i] = new JValue(CREDENTIAL_MASK);
                    }
                }

                ReturnValue = new HttpResponseMessage(actionResult.StatusCode) { Content = new StringContent(meta.ToString(), Encoding.UTF8, MEDIATYPE_JSON) };
            }
        }
    }
}
