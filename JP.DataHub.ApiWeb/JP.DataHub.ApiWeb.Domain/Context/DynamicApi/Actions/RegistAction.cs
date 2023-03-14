using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using AutoMapper;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.Extensions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class RegistAction : AbstractDynamicApiAction, IEntity
    {
        private static readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
        }).CreateMapper();

        [DataContract]
        private class RegistReturnId
        {
            [DataMember]
            public string id { get; set; }

            public bool IsError { get; set; } = false;
            public string Error { get; set; }
        }

        public override HttpResponseMessage ExecuteAction()
        {
            if ((this.MethodType.IsPost != true) && (this.MethodType.IsPut != true))
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }

            if (string.IsNullOrEmpty(this.RepositoryKey.Value))
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }

            // jsonデータ
            JToken json;
            try
            {
                json = ToJson(Contents.ReadToString());
            }
            catch (Exception ex)
            {
                var log = new JPDataHubLogger(this.GetType());
                log.Error(ex);
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10403, this.RelativeUri?.Value);
                msg.Detail = ex.Message.Replace("\r", "").Replace("\n", "");
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, msg.ToJson().ToString()));
            }

            if (json == null || (json.Type != JTokenType.Array && !json.Any()))
            {
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10404, this.RelativeUri?.Value);
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, msg.ToJson().ToString()));
            }

            bool isArray = json.Type == JTokenType.Array && PostDataType?.Value == "array";

            // 通常のエラー
            List<string> errors = new List<string>();
            //JsonValidationエラー
            List<RFC7807ProblemDetailExtendErrors> jsonValidationErros = new List<RFC7807ProblemDetailExtendErrors>();

            // 登録データを作成
            var dicHistoryOriginalData = Enumerable.Range(0, 0).Select(x => new { id = "", index = -1, funcresult = "{'__dummy__':null}".ToJson().RemoveField("dummy") }).ToList();
            var refSourceHeaders = new List<ResponseHeader>();
            var header = new Dictionary<string, string>();
            var resolvedReferenecDatas = new List<JToken>();
            var notify = new List<JsonPropertyFormatProtect>();

            var registerDatas = CreateRegisterDataList(json, isArray, errors, dicHistoryOriginalData, refSourceHeaders, ref jsonValidationErros, ref resolvedReferenecDatas, out notify);
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
                    var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10403, this.RelativeUri?.Value);
                    msg.Detail = string.Join("\r\n", errors);
                    msgJson.Add(msg.ToJson());
                }

                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(msgJson.Count == 1 ? msgJson[0] : msgJson), header));
            }

            // primary repository
            List<RegistReturnId> regids = new List<RegistReturnId>();

            // メールテンプレートまたはWebhookがある場合、イベントハブ通知用のリポジトリを取得
            var enableWebHook = UnityCore.Resolve<bool>("EnableWebHookAndMailTemplate");
            IResourceChangeEventHubStoreRepository eventHubRepository = null;
            if (enableWebHook == true && (HasMailTemplate?.Value == true || HasWebhook?.Value == true))
                eventHubRepository = UnityCore.Resolve<IResourceChangeEventHubStoreRepository>();

            for (int i = 0; i < registerDatas.Count; i++)
            {
                var registerData = registerDatas[i];
                try
                {
                    regids.Add(RegistJson(registerData.jToken, DynamicApiDataStoreRepository[0]));
                    // メールテンプレートまたはWebhookがある場合、イベントハブに通知する
                    if (enableWebHook == true && (HasMailTemplate?.Value == true || HasWebhook?.Value == true))
                        eventHubRepository.Register(this, registerData.jToken);

                    // 既存Base64AttachFileの削除
                    DeleteBase64AttachFiles(registerData.id);
                    // Base64AttachFileのUpload
                    registerData.Base64AttachFiles.ToList().ForEach(y => UploadBase64AttachFile(y.Key, y.Value));
                }
                catch (ConflictException)
                {
                    if (EnableJsonDocumentReference == false)
                    {
                        regids.Add(new RegistReturnId() { IsError = true, Error = "data conflict. please set the latest _etag." });
                        refSourceHeaders.Clear();
                    }
                    else
                    {
                        // 履歴が登録されないように、失敗したデータは、履歴対象から除外する
                        var target = dicHistoryOriginalData.Where(x => x.index == i).First();
                        dicHistoryOriginalData.Remove(target);

                        regids.Add(new RegistReturnId() { IsError = true, Error = "data conflict. please set the latest _etag." });
                        refSourceHeaders.Clear();
                        if (PerRequestDataContainer.XRegisterConflictStop)
                        {
                            // ロールバックする(配列の途中の場合は、途中～最後までロールバック)
                            for (int j = i; j < registerDatas.Count; j++)
                            {
                                OtherResourceRollback(notify[j], ref errors, ref refSourceHeaders);
                            }

                            // XRegisterConflictStopが設定されているときはコンフリクトが発生時点で処理を停止する。
                            break;
                        }
                        else
                        {
                            // ロールバックする
                            OtherResourceRollback(notify[i], ref errors, ref refSourceHeaders);
                        }
                    }
                }
            }

            // ドキュメント履歴への対応
            // すべてのデータ更新が終了したら、Registerによる更新の前のデータをBlob(など)に保存し、履歴情報を作成する
            header = MakeHistory(dicHistoryOriginalData.Select(x => x.id).ToArray(), dicHistoryOriginalData.Select(x => x.funcresult).ToArray(), false);

            // スナップショットを追加
            AddSnapshotToHistory(header, resolvedReferenecDatas);

            // 自身の履歴OFFでもReference先の履歴がONの場合は、自身に空の履歴ヘッダを作る
            // ただし自身の履歴OFFで、Reference先も履歴OFFの場合は、履歴ヘッダ無くて良いので、refSourceHeadersをクリアする
            (header, refSourceHeaders) = MakeEmptyHistoryHeaderIfNoHistory(header, refSourceHeaders);

            // Notify先からのレスポンスヘッダがあればセットする
            header = MergeResponseHeader(header, ToDictionary(MergeHistoryRefSourceHeader(this.ControllerRelativeUrl.Value, refSourceHeaders)));

            if (this.IsEnableBlockchain?.Value == true)
            {
                // 正しく登録できたデータのみSelectしてBlockchainに登録
                var registeredData = dicHistoryOriginalData.Join(registerDatas, x => x.id, y => y.id, (x, y) => new KeyValuePair<string, JToken>(y.id, y.jToken)).ToList();
                var historyHeader = header?.SingleOrDefault(y => y.Key == DOCUMENTHISTORY_HEADERNAME).Value;
                if (historyHeader != null && EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true)
                {
                    var historyInfo = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(historyHeader).SingleOrDefault(x => x.isSelfHistory);
                    registeredData.ForEach(x => RegisterBlockchainEvent(x.Value, DynamicApiDataStoreRepository[0].RepositoryInfo.Type, RepositoryInfo[0], historyInfo.documents.SingleOrDefault(y => y.documentKey == x.Key)?.versionKey));
                }
                else
                {
                    registeredData.ForEach(x => RegisterBlockchainEvent(x.Value, DynamicApiDataStoreRepository[0].RepositoryInfo.Type, RepositoryInfo[0]));
                }
            }

            // 自身のリソースのキャッシュを削除する
            // RegisterActionでキャッシュを消す理由は、このAPIによるQueryのキャッシュが存在している可能性がある。
            // Queryでキャッシュしているものは、Expire時間がくるまで有効になってしまうため、RegisterActionした場合は
            // そのデータを書き換えている可能性があるため、キャッシュをクリアする
            // またキャッシュサイズが大きい（多い）とタイムアウトになる可能性があるため、この処理は非同期とする
            RefreshApiResourceCache(CreateResourceCacheKey());

            // secondary repository
            // ここで発生したエラーは無視する
            this.DynamicApiDataStoreRepository.Skip(1)
                .AsParallel()
                .ToList().ForEach(repository =>
                {
                    var threadPerRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                    mapper.Map<IPerRequestDataContainer, IPerRequestDataContainer>(PerRequestDataContainer, threadPerRequestDataContainer);
                    try
                    {
                        registerDatas.ForEach(x => RegistJson(x.jToken.DeepClone(), repository));
                    }
                    catch (NotImplementedException)
                    {
                        return;
                    }
                    return;
                });

            if (regids.Any(x => x.IsError))
            {
                string result = GetErrorMassage(regids, isArray);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(HttpStatusCode.Conflict, result, header));
            }
            else
            {
                string result = isArray == false && regids.Any() ? JToken.FromObject(regids[0]).ToString() : JToken.FromObject(regids).ToString();
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(this.MethodType.IsPost ? HttpStatusCode.Created : HttpStatusCode.OK, result, header));
            }
        }

        /// <summary>
        /// 登録データリストの生成
        /// </summary>
        private List<RegisterData> CreateRegisterDataList(JToken registerJson, bool isArray, List<string> errors, dynamic dicHistoryOriginalData, List<ResponseHeader> refSourceHeaders, ref List<RFC7807ProblemDetailExtendErrors> jsonValidationErrors, ref List<JToken> resolvedReferenecDatas, out List<JsonPropertyFormatProtect> notify)
        {
            var tmpDicHistoryOriginalData = Enumerable.Range(0, 0).Select(x => new { id = "", index = -1, funcresult = "{'__dummy__':null}".ToJson().RemoveField("dummy") }).ToList();
            tmpDicHistoryOriginalData = dicHistoryOriginalData;

            notify = new List<JsonPropertyFormatProtect>();

            var registerDatas = EnumerableJsonArray(isArray, registerJson)
                .Where(x => x.Any())
                .Select((x, i) => new RegisterData(x, i)).ToList();

            var schema = GetRequestSchema()?.ToJSchema();

            if (IsSkipJsonSchemaValidation == null || IsSkipJsonSchemaValidation.Value == false)
            {
                if (registerDatas.Any())
                {
                    // Jsonのチェック
                    if (ThresholdJsonSchemaValidaitonParallelize <= registerDatas.Count)
                    {
                        // 並列
                        var concurrent = new ConcurrentBag<List<RFC7807ProblemDetailExtendErrors>>();
                        ForeignKeyHttpContext = UnityCore.Resolve<IHttpContextAccessor>().HttpContext;
                        ForeignKeyPerRequestDataContainer = PerRequestDataContainer;

                        var isInternalCall = PerRequestDataContainer.IsInternalCall;
                        ForeignKeyPerRequestDataContainer.IsInternalCall = true;
                        ParallelExtension.ForEachWithContextAndPartitiner(registerDatas, i =>
                        {
                            var paraErrors = new List<RFC7807ProblemDetailExtendErrors>();
                            var data = registerDatas[i].jToken;
                            // AdditionalPropertiesがfalseの場合"id"プロパティが存在するとValidationエラーとなる。
                            // "id"はキーの値を意味するものなので、AdditionalPropertiesがfalseでも"id"は許可にしたい。
                            // よってValidationするときは"id"を削除してからチェックするようにした。
                            if (schema?.AllowAdditionalProperties == false)
                            {
                                data = data.DeepClone();
                                data.RemoveField("id");
                            }
                            ValidateJson(schema, data, isArray ? registerDatas[i].Index : (int?)null, ref paraErrors);
                            concurrent.Add(paraErrors);
                        }, ForeignKeyHttpContext, ForeignKeyPerRequestDataContainer);
                        ForeignKeyPerRequestDataContainer.IsInternalCall = isInternalCall;

                        jsonValidationErrors = concurrent.SelectMany(x => x).ToList();
                    }
                    else
                    {
                        // 直列
                        var isInternalCall = PerRequestDataContainer.IsInternalCall;
                        PerRequestDataContainer.IsInternalCall = true;
                        for (var i = 0; i < registerDatas.Count; i++)
                        {
                            var data = registerDatas[i].jToken;
                            // AdditionalPropertiesがfalseの場合"id"プロパティが存在するとValidationエラーとなる。
                            // "id"はキーの値を意味するものなので、AdditionalPropertiesがfalseでも"id"は許可にしたい。
                            // よってValidationするときは"id"を削除してからチェックするようにした。
                            if (schema?.AllowAdditionalProperties == false)
                            {
                                data = data.DeepClone();
                                data.RemoveField("id");
                            }
                            ValidateJson(schema, data, isArray ? i : (int?)null, ref jsonValidationErrors);
                        }
                        PerRequestDataContainer.IsInternalCall = isInternalCall;
                    }
                }

                if (jsonValidationErrors.Any())
                {
                    return registerDatas;
                }
            }

            // 配列且つ、ID自動割り振りしないAPI場合は、リクエストデータ内にID or リポジトリキーが同じ（同一ドキュメント）更新が無いかチェック
            if ((isArray && registerJson.Any()) && this.IsAutomaticId?.Value == false)
            {
                // IDの値でグループ化し、件数が２件（同じID値のリクエストが重複）無いかチェック
                var idlist = registerDatas.GroupBy(x => x.jToken.GetPropertyValue("id")).ToList();
                var idcheck = idlist.Exists(x => x.Key == null ? false : x.Count() > 1);
                if (idcheck)
                {
                    errors.Add("Array Request does not include same id or repositoryKey in the array");
                    return registerDatas;
                }

                //リポジトリキー
                if (this.RepositoryKey.IsExsitsLogicalKey)
                {
                    var keys = this.RepositoryKey.LogicalKeys.ToList();
                    var repGrp = registerDatas.GroupBy(x =>
                    {
                        string value = string.Empty;
                        foreach (var key in keys)
                        {
                            //リポジトリキーが欠落の場合は、カウント対象外
                            if (x.jToken[key] == null) return null;
                            value += x.jToken[key] + ",";
                        }
                        return value;
                    }).ToList();
                    //リポジトリキーの重複チェック
                    var repCnt = repGrp.Exists(x => x.Key == null ? false : x.Count() > 1);
                    if (repCnt)
                    {
                        errors.Add("Array Request does not include same id or repositoryKey in the array");
                        return registerDatas;
                    }
                }
            }

            for (int i = 0; i < registerDatas.Count; i++)
            {
                // json data
                var x = registerDatas[i];

                // 管理項目の削除し、管理項目を計算する
                Dictionary<string, JToken> removed;
                x.jToken.RemoveAdminFields(out removed);
                IDictionary<string, object> admins = CalcAdminFields(this.DynamicApiDataStoreRepository[0], x.jToken, removed[ID]);
                string _id = admins.ContainsKey(ID) == false ? null : admins[ID] as string;

                JToken resolvedReferenecData = null;
                Func<JToken, bool, JToken> func = (JToken token, bool onlyDataResolve) =>
                {
                    if (onlyDataResolve == false && (_id != null && tmpDicHistoryOriginalData.Count != 0))
                    {
                        var data = tmpDicHistoryOriginalData.Where(xx => xx.id == _id && xx.index == i).FirstOrDefault();
                        if (data != null)
                        {
                            return data.funcresult;
                        }
                    }
                    // idを計算しているため、それを使って元のデータを取得する
                    var dic = new Dictionary<UrlParameterKey, UrlParameterValue>();
                    (token as JObject)?.Properties().ToList().ForEach(p => dic.Add(new UrlParameterKey(p.Name), new UrlParameterValue(token[p.Name]?.ToString())));
                    var urlParam = new UrlParameter(dic);
                    JsonDocument result;    // <= ここに登録するデータの以前のデータ（変更前というべきか）がセットされる
                    using (new Pushd<bool>(() => PerRequestDataContainer.XgetInternalAllField, () => PerRequestDataContainer.XgetInternalAllField = true, (before) => PerRequestDataContainer.XgetInternalAllField = before))
                    {
                        result = _id == null ? null : DynamicApiDataStoreRepository[0].QueryOnce(ValueObjectUtil.Create<QueryParam>(this, new Identification(_id)));
                    }

                    // データがある場合は必ず配列なので最初の要素を元データとする
                    var funcresult = (result == null) ? null : result.Value.Type == JTokenType.Array ? result.Value.FirstOrDefault() : result.Value;
                    if (onlyDataResolve)
                    {
                        // Reference属性の解決
                        resolvedReferenecData = RecoveryReferenceAttribute(funcresult.ToString());
                        return resolvedReferenecData;
                    }

                    // ドキュメント履歴用（オリジナルデータを保存しておく）
                    if (_id != null)
                    {
                        tmpDicHistoryOriginalData.Add(new { id = _id, index = i, funcresult = funcresult });
                    }

                    return funcresult;
                };

                // Protect属性の適用（変更されていたら戻す）
                if (admins.Any(y => y.Key == ID) && UnityCore.Resolve<IPerRequestDataContainer>().IsSkipJsonFormatProtect == false)
                {
                    TmpHttpContext = UnityCore.Resolve<IHttpContextAccessor>().HttpContext; // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
                    TmpPerRequestDataContainer = PerRequestDataContainer?.DeepCopy(); // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用

                    var json = new JsonPropertyFormatProtect(true) { Schema = schema, Update = x.jToken, GetOriginalData = func, GetOtherResource = GetOtherResource };
                    try
                    {
                        json.Recovery();
                    }
                    catch (InvalidNotifyDataException nex)
                    {
                        if (i != 0)
                        {
                            refSourceHeaders.Clear();
                            OtherResourceRollback(notify, ref errors, ref refSourceHeaders);
                        }

                        errors.Add(nex.Message);
                        break;
                    }
                    var jsonForNotify = json;
                    notify.Add(jsonForNotify);

                    var isDataNotifed = false;

                    // 他のリソースを更新
                    if (OtherResourceUpdate(i, json.UpdateOtherResource, ref errors, ref refSourceHeaders, ref jsonValidationErrors, ref isDataNotifed) == false)
                    {
                        refSourceHeaders.Clear();
                        OtherResourceRollback(notify, ref errors, ref refSourceHeaders);
                        break;
                    }

                    //このデータがNotifyをした場合は、更新済みデータを登録する。
                    if (isDataNotifed)
                    {
                        //もう１回Get
                        resolvedReferenecData = func(x.jToken, true);
                        resolvedReferenecDatas.Add(resolvedReferenecData);
                    }
                }

                // ドキュメント履歴が有効な場合、履歴のための処理を行う
                // UpdDate,RegDateを適切に処理するために（SetupAdminFieldsにて行う）、更新前のデータを取得する
                if ((EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value != false) || this.EnableJsonDocumentKeepRegDate == true || PerRequestDataContainer.IsSkipJsonFormatProtect == false)
                {
                    func(x.jToken, false);
                }

                // 管理項目を再設定
                SetupAdminFields(_id == null || tmpDicHistoryOriginalData.Any() == false ? null : tmpDicHistoryOriginalData.Where(xxx => xxx.id == _id && xxx.index == i).First()?.funcresult, x.jToken, admins, EnableJsonDocumentKeepRegDate);
            }
            if (errors.Any())
            {
                return registerDatas;
            }

            // Base64AttachFileのデータが存在する場合はBase64StringをURIに置き換える
            foreach (var x in registerDatas)
            {
                var id = x.jToken.Value<string>(ID);
                if (id != null)
                {
                    x.id = id;
                    x.Base64AttachFiles = ReplaceBase64AttachFileJson(x.jToken, id);
                }
            }

            // 成型後のJsonチェック
            for (int i = 0; i < registerDatas.Count; i++)
            {
                ValidateRegisterData(isArray ? i : (int?)null, registerDatas[i], ref errors);
            }

            dicHistoryOriginalData = tmpDicHistoryOriginalData;

            return registerDatas;
        }

        private void SetupAdminFields(JToken original, JToken json, IDictionary<string, object> admins, bool isKeepRegDate)
        {
            var first = json.First;
            admins.ToList().ForEach(field =>
            {
                json.RemoveField(field.Key);
                first = json.First;
                if (field.Key != REGDATE && field.Key != REGUSERID)
                {
                    first.AddAfterSelf(new JProperty(field.Key, field.Value));
                }
            });

            var time = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            json.RemoveFields(new string[] { REGDATE, REGUSERID, UPDDATE, UPDUSERID });
            first = json.First;
            if (original == null || isKeepRegDate == false)
            {
                first.AddAfterSelf(new JProperty(REGDATE, time));
                first.AddAfterSelf(new JProperty(REGUSERID, OpenId?.Value));
            }
            else
            {
                first.AddAfterSelf(new JProperty(REGDATE, original[REGDATE]));
                first.AddAfterSelf(new JProperty(REGUSERID, original[REGUSERID]));
            }
            first.AddAfterSelf(new JProperty(UPDDATE, time));
            first.AddAfterSelf(new JProperty(UPDUSERID, OpenId?.Value));
        }

        private IDictionary<string, object> CalcAdminFields(INewDynamicApiDataStoreRepository repository, JToken json, JToken keyToken)
        {
            var registerParam = ValueObjectUtil.Create<RegisterParam>(this, json, new XResourceSharingPerson(PerRequestDataContainer.XResourceSharingPerson), new XResourceSharingWith(PerRequestDataContainer.XResourceSharingWith));
            var result = new Dictionary<string, object>();
            DocumentDataId id;
            if (repository.KeyManagement.IsIdValid(keyToken, registerParam, repository.ResourceVersionRepository, out id) == true)
            {
                // もともとidが送られてきていて、それが正しい形式の場合、もとに戻す
                result.Add(ID, id.Id);
            }
            else
            {
                // idを新規に割り振る
                id = repository.KeyManagement.GetId(registerParam, repository.ResourceVersionRepository, PerRequestDataContainer);
                if (id != null)
                {
                    var tmpid = id.Id;
                    if (id.Id.EndsWith(" "))
                    {
                        tmpid = id.Id.Remove(id.Id.Length - 1, 1) + SPACE_HEX;
                    }
                    result.Add(ID, tmpid);
                }
            }

            // 自動採番で、リポジトリーキーのプロパティ要素が１つの場合は、IDで割り振った値（GUIDの部分）をプロパティにもセットする
            if (IsAutomaticId.Value == true && RepositoryKey.IsLogicalKeyOnce == true && RepositoryKey.LogicalKeyFirst != ID)
            {
                result.Add(RepositoryKey.LogicalKeyFirst, id.LogicalId);
            }

            if (IsVendor.Value)
            {
                result.Add(VENDORID, VendorId.Value);
                result.Add(SYSTEMID, SystemId.Value);
            }

            result.Add(REGDATE, PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime());
            result.Add(REGUSERID, OpenId?.Value);
            if ((IsOpenIdAuthentication.Value == true) || (IsOpenIdAuthentication.Value == false && string.IsNullOrEmpty(OpenId?.Value) == false))
            {
                json.RemoveField(OWNERID);
                // 個人共有が許可されているか
                var isResourceSharingPerson = !string.IsNullOrEmpty(PerRequestDataContainer.XResourceSharingPerson) && this.ResourceSharingPersonRules.Any();
                result.Add(OWNERID, isResourceSharingPerson ? PerRequestDataContainer.XResourceSharingPerson : OpenId?.Value);
            }
            return result;
        }

        private bool ValidateJson(JSchema schema, JToken json, int? array, ref List<RFC7807ProblemDetailExtendErrors> jsonValidateErros)
        {
            if (schema != null)
            {
                IList<ValidationError> jsonErrors;
                if (json.IsValid(schema, out jsonErrors) == false)
                {
                    EditJsonSchemaErrorMessage(jsonErrors, ref jsonValidateErros);
                    return false;
                }
            }

            if (!this.IsAutomaticId.Value)
            {
                // 自動採番でない場合はリポジトリキーのチェック
                foreach (var logicalKey in this.RepositoryKey.LogicalKeys)
                {
                    if (json[logicalKey] == null)
                    {
                        var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10403, this.RelativeUri?.Value);
                        msg.Detail = (array == null ? "" : $"array {array} ") + $"{logicalKey} key not found";
                        jsonValidateErros.Add(msg);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ValidateRegisterData(int? array, RegisterData registerData, ref List<string> errors)
        {

            if (IsPerson?.Value == true)
            {
                // validation part2(check owner-info)
                if (registerData.jToken.SelectToken(OWNERID) == null)
                {
                    errors.Add((array == null ? "" : $"array {array} ") + "_Owner_Id property or openId authorization Required");
                }
            }

            // check json size
            if (registerData.jToken.ToString(Formatting.None).Count() > MaxRegisterContentLength)
            {
                errors.Add((array == null ? "" : $"array {array} ") + "Request size is too large");
                return false;
            }

            // Baes64AttachFile
            if (registerData.Base64AttachFiles.Any())
            {
                if (this.IsEnableAttachFile == null || !this.IsEnableAttachFile.Value)
                {
                    // AttachFileを有効にしていない状態で$Base64を指定した場合はエラー
                    errors.Add($"{Base64EncordString} cannot be used unless baldness is attachfile enabled");
                    return false;
                }
                if (!this.AttachFileBlobRepositoryInfo.CanSave)
                {
                    // 有効リポジトリがない場合はエラー
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
                // Base64Stringが規定値を超えた場合はエラー
                errors.Add((array == null ? "" : $"array {array} ") + "Request base64 size is too large");
                return false;
            }

            return true;
        }

        private RegistReturnId RegistJson(JToken json, INewDynamicApiDataStoreRepository repository)
        {
            string registDocReturn = repository.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(json, this))?.Value;
            return new RegistReturnId { id = registDocReturn };
        }

        private void EditJsonSchemaErrorMessage(IList<ValidationError> errors, ref List<RFC7807ProblemDetailExtendErrors> validationErros)
        {
            CreateOrUpdateJsonErrorDictionary(errors, ref validationErros);
        }

        private string EditJsonSchemaErrorMessage(IList<ValidationError> errors)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in errors)
            {
                sb.Append($" Lineno={error.LineNumber} Position={error.LinePosition} Path={error.Path} ErrorMessage={error.Message} ");

            }
            return sb.ToString();
        }
        private string GetErrorMassage(List<RegistReturnId> regids, bool isArray)
        {
            StringBuilder result = new StringBuilder();
            if (!isArray)
            {
                result.Append($"{{\"{nameof(RegistReturnId.Error)}\":\"{regids[0].Error}\"}}");
            }
            else
            {
                result.Append("[");
                bool isFarst = true;
                foreach (var data in regids)
                {
                    if (!isFarst)
                        result.Append(",");
                    if (data.IsError)
                        result.Append($"{{\"{nameof(RegistReturnId.Error)}\":\"{data.Error}\"}}");
                    else
                        result.Append(JToken.FromObject(data).ToString());
                    isFarst = false;
                }
                result.Append("]");
            }
            return result.ToString();
        }
    }
}
