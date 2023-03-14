using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using AutoMapper;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class UpdateAction : AbstractDynamicApiAction, IEntity
    {
        private static readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
        }).CreateMapper();

        /// <summary>
        /// 初期化を行う
        /// 基底クラスの初期化は必ず呼び出すこと
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.IsOverrideId = new IsOverrideId(false);
        }

        public override HttpResponseMessage ExecuteAction()
        {
            if (this.MethodType.IsPatch != true)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }

            if (string.IsNullOrEmpty(this.RepositoryKey.Value))
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }

            // スキーマのチェック
            HttpResponseMessage schemaCheckResponse;
            if (IsValidUrlModelSchema(out schemaCheckResponse) == false)
            {
                return schemaCheckResponse;
            }

            // jsonデータ
            JToken inputJson;
            try
            {
                var tempobj = JsonConvert.DeserializeObject(Contents.ReadToString(), new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal });
                inputJson = JToken.FromObject(tempobj);    //JToken.Parse(Contents.Value);
            }
            catch (Exception ex)
            {
                var log = new JPDataHubLogger(this.GetType());
                log.Error(ex);
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10406, this.RelativeUri?.Value);
                msg.Detail = ex.Message.Replace("\r", "").Replace("\n", "");
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, msg.ToJson().ToString()));
            }

            // Patch対応の前のデータを取得する。その対象データが見つからない場合はNotFound
            var original = GetRepositoryData(this.DynamicApiDataStoreRepository);
            if (original.Count == 0)
            {
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10407, this.RelativeUri?.Value);
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotFound, msg.ToJson().ToString()));
            }
            if (original.Count > 1)
            {
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10408, this.RelativeUri?.Value);
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, msg.ToJson().ToString()));
            }

            if (inputJson.Type == JTokenType.Array)
            {
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10409, this.RelativeUri?.Value);
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, msg.ToJson().ToString()));
            }

            // 登録用データを作成する

            //通常のエラー
            List<string> errors = new List<string>();
            //JsonValidationエラー
            var jsonValidationErros = new List<RFC7807ProblemDetailExtendErrors>();

            var header = new Dictionary<string, string>();
            var refSourceHeaders = new List<ResponseHeader>();
            var resolvedReferenecDatas = new List<JToken>();
            JsonPropertyFormatProtect update = null;
            var registerData = CreateRegisterData(inputJson, original.JToken, errors, refSourceHeaders, ref jsonValidationErros, ref resolvedReferenecDatas, out update);
            if (errors.Count > 0 || jsonValidationErros.Count > 0)
            {
                header = MergeResponseHeader(header, ToDictionary(refSourceHeaders));
                var msgJson = new JArray();
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                if (jsonValidationErros.Count != 0)
                {
                    jsonValidationErros.ForEach(x => msgJson.Add(x.ToJson()));
                }
                if (errors.Count != 0)
                {
                    var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10406, this.RelativeUri?.Value);
                    msg.Detail = string.Join("\r\n", errors);
                }

                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(msgJson.Count == 1 ? msgJson[0] : msgJson), header));
            }

            bool isConflictOccurred = false;
            if (this.DynamicApiDataStoreRepository.Count > 0)
            {
                try
                {
                    var backupRepositoryKey = RepositoryKey;
                    this.RepositoryKey = new RepositoryKey("{id}");
                    RegistJson(this.DynamicApiDataStoreRepository[0], new JsonDocument(registerData.Item1.jToken));
                    RepositoryKey = backupRepositoryKey;

                    //既存Base64AttachFileの削除
                    DeleteBase64AttachFiles(registerData.Item1.id);
                    //Base64AttachFileのUpload
                    registerData.Item1.Base64AttachFiles.ToList().ForEach(y => UploadBase64AttachFile(y.Key, y.Value));

                    // メールテンプレートまたはWebhookがある場合、イベントハブに通知する
                    var enableWebHook = UnityCore.Resolve<bool>("EnableWebHookAndMailTemplate");
                    if (enableWebHook == true && (HasMailTemplate?.Value == true || HasWebhook?.Value == true))
                    {
                        var eventHubRepository = UnityCore.Resolve<IResourceChangeEventHubStoreRepository>();
                        eventHubRepository.Update(this, registerData.Item1.jToken, inputJson);
                    }


                    // 履歴
                    header = MakeHistory(registerData.Item1.id, original.JToken, false);
                }
                catch (ConflictException)
                {
                    isConflictOccurred = true;
                    if (EnableJsonDocumentReference == true && update != null)
                    {
                        //ロールバックする
                        OtherResourceRollback(update, ref errors, ref refSourceHeaders);
                    }
                }
            }

            //スナップショットを追加
            AddSnapshotToHistory(header, resolvedReferenecDatas);

            //自身の履歴OFFでもReference先の履歴がONの場合は、自身に空の履歴ヘッダを作る
            // ただし自身の履歴OFFで、Reference先も履歴OFFの場合は、履歴ヘッダ無くて良いので、refSourceHeadersをクリアする
            (header, refSourceHeaders) = MakeEmptyHistoryHeaderIfNoHistory(header, refSourceHeaders);

            //Notify先からのレスポンスヘッダがあればセットする
            header = MergeResponseHeader(header, ToDictionary(MergeHistoryRefSourceHeader(this.ControllerRelativeUrl?.Value, refSourceHeaders)));
            string versionKey = null;
            if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true)
            {
                var historyHeaderInfo = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(header?.SingleOrDefault(x => x.Key == DOCUMENTHISTORY_HEADERNAME).Value).SingleOrDefault(x => x.isSelfHistory);
                versionKey = historyHeaderInfo.documents.SingleOrDefault(x => x.documentKey == registerData.Item1.id)?.versionKey;
            }

            //ブロックチェーン機能有効な場合はイベントハブに通知する
            RegisterBlockchainEvent(registerData.Item1.jToken, DynamicApiDataStoreRepository[0].RepositoryInfo.Type, RepositoryInfo[0], versionKey);

            Parallel.ForEach(
                this.DynamicApiDataStoreRepository.Skip(1),
                repository =>
                {
                    var threadPerRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                    mapper.Map<IPerRequestDataContainer, IPerRequestDataContainer>(PerRequestDataContainer, threadPerRequestDataContainer);
                    RegistJson(repository, new JsonDocument(registerData.Item1.jToken));
                }
            );

            // remove cache
            try
            {
                string keyCache = CreateResourceCacheKey();
                if (keyCache != null)
                {
                    //自身のリソースのキャッシュを削除する
                    RefreshApiResourceCache(keyCache);
                }
            }
            catch (NotImplementedException)
            {
                // chche none....
            }
            if (isConflictOccurred)
            {
                string result = "{'Error':'data conflict. please set the latest _etag.'}".ToJson().ToString();
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(HttpStatusCode.Conflict, result, header));
            }

            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(HttpStatusCode.NoContent, registerData.Item2?.ToString(), header));
        }

        private string[] RemoveFieldPickup(JToken json)
        {
            List<string> result = new List<string>();
            for (JToken token = json.First(); token != null; token = token.Next)
            {
                if (token != null && token.Path != null && token.Path[0] == '_')
                {
                    result.Add(token.Path);
                }
            }
            result.Add(ID);
            return result.ToArray();
        }

        private JsonSearchResult GetRepositoryData(ReadOnlyCollection<INewDynamicApiDataStoreRepository> repositories)
        {
            using (new Pushd<bool>(() => PerRequestDataContainer.XgetInternalAllField, () => PerRequestDataContainer.XgetInternalAllField = true, (bool before) => PerRequestDataContainer.XgetInternalAllField = before))
            {
                foreach (var repository in repositories)
                {
                    JsonSearchResult result = new JsonSearchResult(this.ApiQuery, this.PostDataType, this.ActionType);
                    try
                    {
                        var queryParam = ValueObjectUtil.Create<QueryParam>(this, new HasSingleData(false), new XResourceSharingPerson(PerRequestDataContainer.XResourceSharingPerson), new XResourceSharingWith(PerRequestDataContainer.XResourceSharingWith));
                        var queryResult = repository.QueryEnumerable(queryParam).ToList();
                        if (queryResult.Any())
                        {
                            result.BeginData();
                            result.AddString(queryResult.FirstOrDefault()?.Value.ToString());
                            result.EndData();
                        }
                        else if (queryParam.QueryString != null)
                        {
                            //0件のときに、パラメータのスペースの16進数変換を行い再取得する
                            if (ConvertHexToSpace(queryParam, out var queryParam2))
                            {
                                queryResult = repository.QueryEnumerable(queryParam2).ToList();
                                if (queryResult.Any())
                                {
                                    result.BeginData();
                                    result.AddString(queryResult.FirstOrDefault()?.Value.ToString());
                                    result.EndData();
                                }
                            }
                        }
                    }
                    catch (NotImplementedException)
                    {
                        result.BeginData();
                        result.EndData();
                    }
                    if (result.Count != 0)
                    {
                        return result;
                    }
                }
            }
            var tempRet = new JsonSearchResult(this.ApiQuery, this.PostDataType, this.ActionType);
            tempRet.BeginData();
            tempRet.EndData();
            return tempRet;
        }

        private void RegistJson(INewDynamicApiDataStoreRepository repository, JsonDocument mergedResult)
        {
            repository.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(this, mergedResult, new XResourceSharingPerson(PerRequestDataContainer.XResourceSharingPerson), new XResourceSharingWith(PerRequestDataContainer.XResourceSharingWith)));
        }

        private Tuple<JToken, JToken> MergeJson(JToken target, JToken source)
        {
            var tmp = target.ToString().ToJson();

            JToken before = null;

            var targetJObject = (JObject)tmp;
            var sourceJObject = (JObject)source;
            if (IsInernalMerge(sourceJObject))
            {
                before = InternalMerge(sourceJObject, targetJObject);
            }
            else
            {
                targetJObject.Merge(
                    sourceJObject,
                    new JsonMergeSettings()
                    {
                        MergeArrayHandling = MergeArrayHandling.Replace,
                        MergeNullValueHandling = MergeNullValueHandling.Merge
                    });
            }
            var old = XGetInnerAllField;
            XGetInnerAllField = new XGetInnerField(true);
            var result = CloneToken(JToken.Parse(targetJObject.ToString()));
            XGetInnerAllField = old;
            return new Tuple<JToken, JToken>(result, before);
        }

        private void EditJsonSchemaErrorMessage(IList<ValidationError> errors, ref List<RFC7807ProblemDetailExtendErrors> problems)
        {
            CreateOrUpdateJsonErrorDictionary(errors, ref problems);
        }

        /// <summary>
        /// 登録データリストの生成
        /// </summary>
        private Tuple<RegisterData, JToken> CreateRegisterData(JToken input, JToken original, List<string> errors, List<ResponseHeader> refSourceHeaders, ref List<RFC7807ProblemDetailExtendErrors> jsonValidationErros, ref List<JToken> resolvedReferenecDatas, out JsonPropertyFormatProtect trueUpdate)
        {

            var originalTrue = original.DeepClone();
            // Updateをする前のデータ。それを$Referenceなどを解決したものを作る
            original = RecoveryReferenceAttribute(original.DeepClone().ToString());
            JToken before = null;

            // スキーマのチェック（この時は$Referenceなどを解決したoriginalを使う）
            var schema = GetRequestSchema()?.ToJSchema();
            // 他のリソースを更新
            trueUpdate = null;
            if (PerRequestDataContainer.IsSkipJsonFormatProtect == false)
            {
                trueUpdate = new JsonPropertyFormatProtect(true) { Schema = schema, Update = input, GetOriginalData = (JToken token, bool onlyDataResolve) => originalTrue, GetOtherResource = GetOtherResource };
            }

            if (schema != null)
            {
                if (IsSkipJsonSchemaValidation == null || IsSkipJsonSchemaValidation.Value == false)
                {
                    //チェック時に不要な項目は削除する
                    var mergedJson = MergeJson(original, input);
                    mergedJson.Item1.RemoveFields(RemoveFieldPickup(mergedJson.Item1));
                    before = mergedJson.Item2;

                    var validationSet = new List<(JSchema Schema, JToken Data)>();
                    IList<ValidationError> validationErrors;
                    if (UnityCore.Resolve<bool>("UseStrictValidationOnUpdate"))
                    {
                        // リクエストモデルがあれば入力データを検証
                        // リソースモデルがあれば登録データを検証
                        var requestSchema = RequestSchema?.ToJSchema();
                        var controllerSchema = ControllerSchema?.ToJSchema();
                        if (requestSchema != null)
                        {
                            validationSet.Add((requestSchema, input));
                        }
                        if (controllerSchema != null)
                        {
                            validationSet.Add((controllerSchema, mergedJson.Item1));
                        }
                    }
                    else
                    {
                        // リクエストモデル(なければリソースモデル)で登録データを検証
                        validationSet.Add((schema, mergedJson.Item1));
                    }

                    foreach (var validation in validationSet)
                    {
                        // AdditionalPropertiesがfalseの場合"id"プロパティが存在するとValidationエラーとなる。
                        // "id"はキーの値を意味するものなので、AdditionalPropertiesがfalseでも"id"は許可にしたい。
                        // よってValidationするときは"id"を削除してからチェックするようにした。
                        var data = validation.Data;
                        if (validation.Schema.AllowAdditionalProperties == false)
                        {
                            data = validation.Data.DeepClone();
                            data.RemoveField("id");
                        }
                        if (data.IsValid(validation.Schema, out validationErrors) == false)
                        {
                            EditJsonSchemaErrorMessage(validationErrors, ref jsonValidationErros);
                            return new Tuple<RegisterData, JToken>(new RegisterData(input), before);
                        }
                    }
                }
            }

            // base64を復元
            input.RemoveFields(RemoveFieldPickup(input));
            if (this.IsEnableAttachFile != null && this.IsEnableAttachFile.Value)
            {
                Dictionary<string, string> base64PathList = new Dictionary<string, string>();
                originalTrue = ReplaceJtoken(originalTrue, base64PathList, "", ReplaceJtokenPathToBase64);
            }

            var mergeJson = MergeJson(originalTrue, input);
            if (PerRequestDataContainer.IsSkipJsonFormatProtect == false)
            {
                // Protect属性の適用（変更されていたら戻す）
                var json = new JsonPropertyFormatProtect(true) { Schema = (JSchema)schema, Update = mergeJson.Item1, GetOriginalData = (JToken token, bool onlyDataResolve) => originalTrue };
                json.Recovery();

                try
                {
                    trueUpdate.Recovery();
                }
                catch (InvalidNotifyDataException nex)
                {
                    errors.Add(nex.Message);
                    return null;
                }

                bool isDataNotified = false;
                if (OtherResourceUpdate(null, trueUpdate.UpdateOtherResource, ref errors, ref refSourceHeaders, ref jsonValidationErros, ref isDataNotified) == false)
                {
                    refSourceHeaders.Clear();
                    OtherResourceRollback(trueUpdate, ref errors, ref refSourceHeaders);
                }

                if (isDataNotified)
                {
                    resolvedReferenecDatas.Add(RecoveryReferenceAttribute(originalTrue.ToString()));
                }
            }
            // UPD
            var time = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            var first = mergeJson.Item1.First;
            if (mergeJson.Item1[REGDATE] == null)
            {
                first.AddAfterSelf(new JProperty(REGDATE, original[REGDATE]));
            }
            if (mergeJson.Item1[REGUSERID] == null)
            {
                first.AddAfterSelf(new JProperty(REGUSERID, original[REGUSERID]));
            }
            mergeJson.Item1.RemoveFields(new string[] { UPDDATE, UPDUSERID });
            first.AddAfterSelf(new JProperty(UPDDATE, time));
            first.AddAfterSelf(new JProperty(UPDUSERID, OpenId?.Value));

            //Base64AttachFileのデータが存在する場合はBase64StringをURIに置き換える
            var regData = new Tuple<RegisterData, JToken>(new RegisterData(mergeJson.Item1), before);
            var id = regData.Item1.jToken.Value<string>(ID);
            if (id != null)
            {
                regData.Item1.id = id;
                regData.Item1.Base64AttachFiles = ReplaceBase64AttachFileJson(regData.Item1.jToken, id);
            }

            ValidateRegisterData(regData.Item1, ref errors);
            return regData;
        }

        private bool ValidateRegisterData(RegisterData registerData, ref List<string> errors)
        {
            // check json size
            if (registerData.jToken.ToString(Formatting.None).Count() > MaxRegisterContentLength)
            {
                errors.Add("Request size is too large");
                return false;
            }

            //Baes64AttachFile
            if (registerData.Base64AttachFiles.Any())
            {
                if (this.IsEnableAttachFile == null || !this.IsEnableAttachFile.Value)
                {
                    //AttachFileを有効にしていない状態で$Base64を指定した場合はエラー
                    errors.Add($"{Base64EncordString} cannot be used unless baldness is attachfile enabled");
                    return false;
                }
                if (!this.AttachFileBlobRepositoryInfo.CanSave)
                {
                    //有効リポジトリがない場合はエラー
                    errors.Add($"attachfile repository is full.");
                    return false;
                }
                foreach (var v in registerData.Base64AttachFiles)
                {
                    if (!TryBase64(v.Value))
                    {
                        errors.Add("can not parse base64");
                        return false;
                    }
                }
            }

            if (registerData.Base64AttachFiles.Sum(x => x.Value.Length) > MaxBase64AttachFileContentLength)
            {
                //Base64Stringが規定値を超えた場合はエラー
                errors.Add("Request base64 size is too large");
                return false;
            }
            return true;
        }
    }
}
