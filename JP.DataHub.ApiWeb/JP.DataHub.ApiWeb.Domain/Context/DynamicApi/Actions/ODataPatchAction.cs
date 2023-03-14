using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.OData.Interface.Exceptions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    /// <summary>
    /// ODataPatchAction
    /// RDBMS    : 入力されたfilter条件に基づきUPDATE文を使用して一括で更新
    /// RDBMS以外: 入力されたfilter条件に基づく検索結果を1件ずつ更新(未実装)
    /// </summary>
    internal class ODataPatchAction : ODataAction, IEntity
    {
        private const string AdditionalConditionPropertyName = "_Where";
        private static readonly int AdditionalConditionMaxInClauseItems = UnityCore.Resolve<IConfiguration>().GetValue<int>("AppConfig:ODataPatchMaxInClauseItems", 1000);
        private static readonly string[] ManagementColumns = new string[] { ID, VERSION_COLNAME, VENDORID, SYSTEMID, OWNERID, REGUSERID, REGDATE, UPDUSERID, UPDDATE };
        private static readonly string[] InvalidODataColumns = UnityCore.Resolve<string[]>("InvalidODataColums");

        /// <summary>
        /// 無視するODataクエリリスト
        /// </summary>
        private readonly List<string> OdataIgnoreList = new List<string>() { "$top", "$select", "$count", "$orderby" };


        /// <summary>
        /// メールテンプレート及びWebHookの有効無効
        /// </summary>
        private bool EnableWebHookAndMailTemplate { get; set; } = UnityCore.Resolve<bool>("EnableWebHookAndMailTemplate");


        /// <summary>
        /// 初期化を行う、初期化はAbstractDynamicApiActionBaseクラスを継承したクラスそれぞれに実装をする。
        /// </summary>
        public override void Initialize()
        {
        }

        /// <summary>
        /// ODataの検索結果データを対象に更新を行う。
        /// </summary>
        public override HttpResponseMessage ExecuteAction()
        {
            // API設定のバリデーション
            if (this.MethodType.IsPatch != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10419, RelativeUri?.Value);
            }
            if (string.IsNullOrEmpty(RepositoryKey?.Value))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10420, RelativeUri?.Value);
            }

            var targetRepositories = DynamicApiDataStoreRepository.Where(x => x.ODataPatchSupport != ODataPatchSupport.None);
            if (targetRepositories.Count() < 1)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10435, RelativeUri?.Value);
            }

            // 楽観排他ONの場合はカスタムヘッダによる無視指定がある場合のみ実行可
            if (IsOptimisticConcurrency?.Value == true && XNoOptimistic?.Value != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10436, RelativeUri?.Value);
            }

            // 更新内容のバリデーション
            var errors = ValidatePatchData(out JToken patchData);
            if (errors != null)
            {
                MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(errors);
            }

            // 一括更新時のバリデーション
            // 一括更新の場合は更新対象ドキュメント単位の処理が行えない等の制約があるため
            // 一括更新のリポジトリを含む場合は追加でバリデーションを実施
            if (targetRepositories.Any(x => x.ODataPatchSupport == ODataPatchSupport.BulkUpdate))
            {
                errors = ValidateUnsupportedFeatures(patchData) ??
                         ValidatePatchDataForBulkUpdate(patchData);
                if (errors != null)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(errors);
                }
            }


            // 更新日時・ユーザーを更新対象に追加
            patchData.RemoveFields(new string[] { UPDDATE, UPDUSERID });
            var first = patchData.First;
            first.AddAfterSelf(new JProperty(UPDDATE, PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime()));
            first.AddAfterSelf(new JProperty(UPDUSERID, OpenId?.Value));


            // Patch実行
            // 便宜的に対象リポジトリの先頭をプライマリとする
            var primaryRepository = targetRepositories.First();
            var primaryAffectedCount = 0;
            Exception primaryException = null;
            Parallel.ForEach(targetRepositories, repository =>
            {
                var isPrimary = (primaryRepository == repository);
                try
                {
                    // マルチスレッド処理のためPerRequestDataContainerをスレッド毎にクローン
                    var threadPerRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                    PerRequestDataContainer.DeepCopy(threadPerRequestDataContainer);

                    // RDBMSかそれ以外かに応じて異なる方式でPatchを実行
                    var affectedCount = (repository.ODataPatchSupport == ODataPatchSupport.BulkUpdate)
                        ? PatchByUpdateSql(patchData, repository, isPrimary)
                        : PatchSequentially(patchData, repository, isPrimary);

                    if (isPrimary)
                    {
                        primaryAffectedCount = affectedCount;
                    }

                    Logger.Info($"ODataPatch updated {affectedCount} documents on {repository.RepositoryInfo.PhysicalRepositoryId}");
                }
                catch (Exception ex) when (isPrimary)
                {
                    primaryException = ex;
                }
            });

            // 自身のリソースのキャッシュを削除する
            try
            {
                var keyCache = CreateResourceCacheKey();
                if (keyCache != null)
                {
                    Cache.RemoveFirstMatch(keyCache);
                }
            }
            catch (NotImplementedException)
            {
                // chche none....
            }

            if (primaryException != null)
            {
                throw primaryException;
            }

            // プライマリの更新結果に基づいてレスポンス返却
            return primaryAffectedCount > 0
                ? TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NoContent, null))
                : ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.I10402, RelativeUri?.Value);
        }


        /// <summary>
        /// SQLによる一括更新
        /// </summary>
        private int PatchByUpdateSql(JToken patchData, INewDynamicApiDataStoreRepository repository, bool isPrimary)
        {
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ODataQuery(Query?.GetQueryString(OdataIgnoreList, true)), new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()), this);
            var affectedCount = 0;
            try
            {
                affectedCount = repository.ODataPatch(queryParam, patchData);
            }
            // プライマリ以外のエラーはログ出力して無視
            catch (ODataException ex) when (!isPrimary)
            {
                Logger.Info($"{ex.GetType().Name} occured on ODataPatch secondary repository. PhysicalRepositoryId={repository.RepositoryInfo.PhysicalRepositoryId}", ex);
            }
            catch (DomainException ex) when (!isPrimary && (ex is ODataInvalidFilterColumnException || ex is ODataNotConvertibleToQueryException))
            {
                Logger.Info($"{ex.GetType().Name} occured on ODataPatch secondary repository. PhysicalRepositoryId={repository.RepositoryInfo.PhysicalRepositoryId}", ex);
            }
            catch (Exception ex) when (!isPrimary)
            {
                Logger.Fatal($"Unexpected {ex.GetType().Name} occured on ODataPatch secondary repository. PhysicalRepositoryId={repository.RepositoryInfo.PhysicalRepositoryId}", ex);
            }

            return affectedCount;
        }

        /// <summary>
        /// 検索結果を1件ずつ更新
        /// </summary>
        private int PatchSequentially(JToken patchData, INewDynamicApiDataStoreRepository repository, bool isPrimary)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新内容のバリデーション
        /// </summary>
        private RFC7807ProblemDetailExtendErrors ValidatePatchData(out JToken patchData)
        {
            patchData = null;

            // JSON形式チェック
            try
            {
                var tempobj = JsonConvert.DeserializeObject(Contents.ReadToString(), new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal });
                patchData = JToken.FromObject(tempobj);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10438, RelativeUri?.Value);
                msg.Detail = ex.Message.Replace("\r", "").Replace("\n", "");
                return msg;
            }
            if (patchData.Type == JTokenType.Array)
            {
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10438, RelativeUri?.Value);
                msg.Detail = "Expected Object but got Array.";
                return msg;
            }
            if (!patchData.Children().Any(x => x.Type == JTokenType.Property && ((JProperty)x).Name != AdditionalConditionPropertyName))
            {
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10438, RelativeUri?.Value);
                msg.Detail = "Properties to update is not specified.";
                return msg;
            }

            // 更新対象項目チェック
            var errors = new Dictionary<string, dynamic>();
            patchData.Children().Where(x => x.Type == JTokenType.Property).Select(x => (JProperty)x).ToList().ForEach(x =>
            {
                // 管理項目及びidの更新は不可
                if ((x.Name.StartsWith("_") && x.Name != AdditionalConditionPropertyName) || x.Name == ID)
                {
                    errors.Add(x.Name, new List<string> { "Updating this property is not allowed." });
                }
            });

            if (errors.Count > 0)
            {
                var rfc7807 = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10439, RelativeUri?.Value);
                rfc7807.Errors = errors;
                return rfc7807;
            }

            return null;
        }

        /// <summary>
        /// 更新内容のバリデーション(一括更新)
        /// </summary>
        private RFC7807ProblemDetailExtendErrors ValidatePatchDataForBulkUpdate(JToken patchData)
        {
            var propertyErrors = new Dictionary<string, dynamic>();
            var jsonValidationErrors = new Dictionary<string, dynamic>();

            // 更新対象項目チェック
            var schema = ControllerSchema?.ToJSchema();
            patchData.Children().Where(x => x.Type == JTokenType.Property && ((JProperty)x).Name != AdditionalConditionPropertyName).Select(x => (JProperty)x).ToList().ForEach(x =>
            {
                var propertySchema = schema?.Properties.FirstOrDefault(y => y.Key == x.Name).Value;
                if (propertySchema == null && !(schema?.AllowAdditionalProperties ?? true))
                {
                    propertyErrors.Add(x.Name, new List<string> { "This Property has not been defined and the schema does not allow additional properties." });
                }
                if (propertySchema != null)
                {
                    // 【制約事項】
                    // 更新対象の既存データを取得せずに更新をかけるため、オブジェクトはトップレベルでの洗い替えとなる。
                    // オブジェクトの子要素の項目のみの更新は出来ず、PatchDataがオブジェクトの制約を満たさない場合はここでバリデーションエラー。
                    if (!x.Value.IsValid(propertySchema, out IList<string> schemaErrors))
                    {
                        jsonValidationErrors.Add(x.Name, schemaErrors);
                    }
                }
            });

            if (propertyErrors.Count > 0)
            {
                var rfc7807 = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10439, RelativeUri?.Value);
                if (ReturnJsonValidatorErrorDetail || PerRequestDataContainer.ReturnNeedsJsonValidatorErrorDetail)
                {
                    rfc7807.Errors = propertyErrors;
                }
                return rfc7807;
            }
            if (jsonValidationErrors.Count > 0)
            {
                var rfc7807 = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10402, RelativeUri?.Value);
                if (ReturnJsonValidatorErrorDetail || PerRequestDataContainer.ReturnNeedsJsonValidatorErrorDetail)
                {
                    rfc7807.Errors = jsonValidationErrors;
                }
                return rfc7807;
            }


            // 追加条件チェック
            // Ocha専用の隠し機能としてリクエストBodyの"_Where"で追加条件を指定可能
            // ODataのfilterでIN句が使用できないためその代替手段(現状IN句の形式のみ対応)
            if (!patchData.IsExistProperty(AdditionalConditionPropertyName))
            {
                return null;
            }

            ODataPatchAdditionalCondition condition = null;
            try
            {
                condition = JsonConvert.DeserializeObject<ODataPatchAdditionalCondition>(patchData[AdditionalConditionPropertyName].ToString());
            }
            catch (Exception ex)
            {
                propertyErrors.Add(AdditionalConditionPropertyName, new List<string> { ex.Message.Replace("\r", "").Replace("\n", "") });
            }

            if (condition != null)
            {
                if ((!ManagementColumns.Contains(condition.ColumnName) && schema != null && !schema.Properties.ContainsKey(condition.ColumnName) && !schema.AllowAdditionalProperties) ||
                    (InvalidODataColumns?.Contains(condition.ColumnName) ?? false))
                {
                    propertyErrors.Add($"{AdditionalConditionPropertyName}/ColumnName", new List<string> { $"'{condition.ColumnName}' is not available for query." });
                }
                if (condition.Operator?.ToUpper() != "IN")
                {
                    propertyErrors.Add($"{AdditionalConditionPropertyName}/Operator", new List<string> { $"Undefined Operator '{condition.Operator}'." });
                }
                if (condition.Object?.Count <= 0)
                {
                    propertyErrors.Add($"{AdditionalConditionPropertyName}/Object", new List<string> { "Object cannot be empty." });
                }
                if (condition.Object?.Count > AdditionalConditionMaxInClauseItems)
                {
                    propertyErrors.Add($"{AdditionalConditionPropertyName}/Object", new List<string> { "Object items too mutch." });
                }
            }

            if (propertyErrors.Count > 0)
            {
                var rfc7807 = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10440, RelativeUri?.Value);
                if (ReturnJsonValidatorErrorDetail || PerRequestDataContainer.ReturnNeedsJsonValidatorErrorDetail)
                {
                    rfc7807.Errors = propertyErrors;
                }
                return rfc7807;
            }

            return null;
        }

        /// <summary>
        /// 未対応機能のバリデーション
        /// </summary>
        /// <remarks>
        /// 履歴、メールテンプレート、WebHook、ブロックチェーン、Base64は未対応
        /// </remarks>
        private RFC7807ProblemDetailExtendErrors ValidateUnsupportedFeatures(JToken patchData)
        {
            var errors = new List<string>();

            if (EnableJsonDocumentHistory && IsDocumentHistory?.Value == true)
            {
                errors.Add("DocumentHistory is not supported ODataPatch.");
            }
            if (EnableWebHookAndMailTemplate && HasMailTemplate?.Value == true)
            {
                errors.Add("MailTemplate is not supported ODataPatch.");
            }
            if (EnableWebHookAndMailTemplate && HasWebhook?.Value == true)
            {
                errors.Add("Webhook is not supported ODataPatch.");
            }
            if (IsEnableBlockchain?.Value == true)
            {
                errors.Add("Blockchain is not supported ODataPatch.");
            }
            if (ContainsBase64(patchData))
            {
                errors.Add("Base64 is not supported ODataPatch.");
            }

            if (errors.Count > 0)
            {
                var rfc7807 = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10437);
                rfc7807.Errors = new Dictionary<string, dynamic>() { { "", errors } };
                return rfc7807;
            }

            return null;
        }
    }
}
