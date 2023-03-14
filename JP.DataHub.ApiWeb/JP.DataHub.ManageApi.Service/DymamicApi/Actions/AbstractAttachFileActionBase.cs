using JP.DataHub.Com.Log;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Actions
{
    internal class AbstractAttachFileActionBase
    {
        //protected new JPDataHubLogger Logger = new JPDataHubLogger(typeof(AbstractAttachFileActionBase));

        ////AttachFileAPIはプライマリにメタ用のリポジトリ、セカンダリにファイル用のリポジトリが設定されている
        //private readonly int MetaRepositoryIndex = 0;
        //private readonly int FileRepositoryIndex = 1;
        //protected internal INewDynamicApiDataStoreRepository MetaDataStoreRepository
        //{
        //    get
        //    {
        //        return DynamicApiDataStoreRepository[MetaRepositoryIndex];
        //    }
        //}
        //protected internal IDynamicApiAttachFileRepository FileDataStoreRepository
        //{
        //    get
        //    {
        //        if (AttachFileDynamicApiDataStoreRepository is IDynamicApiAttachFileRepository blob)
        //        {
        //            return blob;
        //        }

        //        return null;
        //    }
        //}
        //protected internal RepositoryInfo MetaDataStoreRepositoryInfo
        //{
        //    get
        //    {
        //        return RepositoryInfo.FirstOrDefault(x => x.Type == DynamicApi.RepositoryInfo.RepositoryType.afm);
        //    }
        //}
        //protected internal RepositoryInfo FileDataStoreRepositoryInfo
        //{
        //    get
        //    {
        //        return RepositoryInfo.FirstOrDefault(x => x.Type == DynamicApi.RepositoryInfo.RepositoryType.afb);
        //    }
        //}
        //protected bool IsKeyUnMatch(DynamicApiAttachFileInformation info)
        //{
        //    var queryKey = new QueryStringKey("Key");
        //    string keyString = "";
        //    if (this.Query.ContainsKey(queryKey))
        //    {
        //        keyString = this.Query[queryKey].Value;
        //    }
        //    if (!info.IsKeyMatch(keyString))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //protected FileId GetQueryStringToFileId()
        //{
        //    var key = new QueryStringKey(nameof(DynamicApiAttachFileInformation.FileId));
        //    if (this.Query.ContainsKey(key))
        //    {
        //        Guid fileId;
        //        if (Guid.TryParse(this.Query[key].Value, out fileId))
        //        {
        //            return new FileId(fileId);
        //        }
        //    }
        //    return null;
        //}

        //public IEnumerable<DocumentHistoryHeader> MakeHistory(string key, string historyFilePath, JToken metadata, bool isDelete, bool isDriveout = false)
        //{
        //    var header = new List<DocumentHistoryHeader>();
        //    if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true && HistoryEvacuationDataStoreRepository == null)
        //    {
        //        Logger.Warn("履歴退避用のRepositoryが設定されていません");
        //    }
        //    else if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true && HistoryEvacuationDataStoreRepository != null)
        //    {
        //        var headerInfo = new List<DocumentHistoryHeaderDocumentData>();
        //        var documentIdToken = metadata.GetPropertyValue("id");
        //        string documentId = null;
        //        if (documentIdToken != null && documentIdToken.Type != JTokenType.Null)
        //            documentId = documentIdToken.ToString();

        //        var documentKey = new AttachFileDocumentKey(RepositoryKey, key, documentId);
        //        var resultHistory = MetaDataStoreRepository.DocumentVersionRepository.GetDocumentVersion(documentKey);
        //        if (resultHistory == null)
        //        {//履歴がない = 初回
        //            resultHistory = MetaDataStoreRepository.DocumentVersionRepository.SaveDocumentVersion(documentKey, new RepositoryKeyInfo(FileDataStoreRepositoryInfo), isDelete);
        //        }
        //        else
        //        {
        //            var latest = resultHistory.DocumentVersions.LastOrDefault();
        //            if (historyFilePath != null)
        //            {
        //                var documentmetainfo = new DocumentHistoryAttachFileMetaData(metadata[nameof(DynamicApiAttachFileInformation.ContentType)].ToString(), metadata[nameof(DynamicApiAttachFileInformation.Key)].ToString());
        //                latest = new DocumentHistory(latest.VersionKey, latest.VersionNo.Value, latest.CreateDate, latest.OpenId, DocumentHistory.StorageLocationType.LowPerformance, HistoryEvacuationDataStoreRepository.RepositoryKeyInfo, historyFilePath, attachFileInfo: documentmetainfo);
        //            }
        //            else if (latest.LocationType != DocumentHistory.StorageLocationType.LowPerformance)
        //            {
        //                latest = new DocumentHistory(latest.VersionKey, latest.VersionNo.Value, latest.CreateDate, latest.OpenId, DocumentHistory.StorageLocationType.Delete, null, null, attachFileInfo: latest.AttachFileMetaInfo);
        //            }

        //            if (isDriveout)
        //            {
        //                resultHistory = MetaDataStoreRepository.DocumentVersionRepository.UpdateDocumentVersion(documentKey, latest);
        //            }
        //            else
        //            {
        //                resultHistory = MetaDataStoreRepository.DocumentVersionRepository.SaveDocumentVersion(documentKey, new RepositoryKeyInfo(FileDataStoreRepositoryInfo), latest, isDelete);
        //            }

        //        }

        //        headerInfo.Add(new DocumentHistoryHeaderDocumentData(key, resultHistory.DocumentVersions.LastOrDefault()?.VersionKey));
        //        header.Add(new DocumentHistoryHeader(isSelfHistory: true, this.ControllerRelativeUrl?.Value, headerInfo));
        //    }
        //    return header.Any() ? header : null;
        //}

        //protected Stream GetDocumentHistoryStream(DocumentHistory target)
        //{
        //    var perRequestDataContainer = DomainUnityContainer.Resolve<IPerRequestDataContainer>();
        //    var param = ValueObject.Create<QueryParam>(this, perRequestDataContainer);
        //    if (HistoryEvacuationDataStoreRepository is INewBlobDataStoreRepository blob)
        //    {
        //        var strs = new List<string>(target.Location.Split('/'));
        //        // locationの最初のセンテンスがContainer名、残りがパスとファイル名
        //        blob.DefaultContainerFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => strs[0];
        //        blob.DefaultFileNameFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => string.Join("/", strs.Skip(1).ToArray());
        //        blob.DefaultRepositoryIds = () => new Tuple<Guid?, Guid?>(target.RepositoryGroupId, target.PhysicalRepositoryId);
        //        return blob.QueryToStream(param);
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //protected Uri GetDocumentHistoryUri(DocumentHistory target)
        //{
        //    var perRequestDataContainer = DomainUnityContainer.Resolve<IPerRequestDataContainer>();
        //    var param = new QueryParam(filePath: new FilePath(target.Location));

        //    if (HistoryEvacuationDataStoreRepository is INewBlobDataStoreRepository blob)
        //    {
        //        var strs = new List<string>(target.Location.Split('/'));
        //        // locationの最初のセンテンスがContainer名、残りがパスとファイル名
        //        blob.DefaultContainerFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => strs[0];
        //        blob.DefaultFileNameFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => string.Join("/", strs.Skip(1).ToArray());
        //        blob.DefaultRepositoryIds = () => new Tuple<Guid?, Guid?>(target.RepositoryGroupId, target.PhysicalRepositoryId);
        //        return blob.GetUriWithSharedAccessSignature(param);
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }

        //}


        //protected void UpdateMeta(JToken infoJson, string etag, bool isUploaded)
        //{
        //    var replaced = infoJson.ReplaceField(nameof(DynamicApiAttachFileInformation.IsUploaded), isUploaded);
        //    var type = replaced.SelectToken("_Type");

        //    if (type != null)
        //    {
        //        type.Parent.Remove();
        //    }
        //    var version = replaced.SelectToken("_Version");
        //    if (version != null)
        //    {
        //        version.Parent.Remove();
        //    }
        //    if (!string.IsNullOrEmpty(etag?.Trim()))
        //    {
        //        replaced["_etag"] = etag;
        //    }
        //    var param = ValueObject.Create<RegisterParam>(this, replaced);
        //    DynamicApiDataStoreRepository[0].RegisterOnce(param);

        //    //ブロックチェーン機能有効な場合はイベントを発行
        //    BlockchainEventHubStoreRepository?.Register(replaced["FileId"].ToString(), JToken.FromObject(replaced), DynamicApiDataStoreRepository[0].RepositoryInfo.Type);
        //}

        //protected string ReplicateFile(JToken infoJson, DynamicApiAttachFileInformation info)
        //{
        //    var srcFileUriWithSas = FileDataStoreRepository.GetUriWithSharedAccessSignature(info);
        //    //作成されるフォルダの名称を変更する
        //    var repositoryKeyForCopy = new RepositoryKey(RepositoryKey.Value.Replace($"/{{{nameof(DynamicApiAttachFileInformation.FileId)}}}", $"_AttachFile/{{{nameof(DynamicApiAttachFileInformation.FileId)}}}"));
        //    var regParam = new RegisterParam(srcFileUriWithSas, infoJson, VendorId, SystemId, OpenId, repositoryKeyForCopy, PartitionKey, Xversion, IsVendor, IsPerson, ApiId, ControllerId, repositoryKeyForCopy, ApiUrl);
        //    var regResult = HistoryEvacuationDataStoreRepository.RegisterOnce(regParam);
        //    return (regResult.Additional["Container"] as string).UrlCombine(regResult.Value);
        //}
    }
}
