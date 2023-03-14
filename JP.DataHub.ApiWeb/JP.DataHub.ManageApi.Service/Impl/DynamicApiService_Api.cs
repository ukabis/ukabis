using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Consts;
using JP.DataHub.ManageApi.Service.DymamicApi;
using JP.DataHub.ManageApi.Service.DymamicApi.Scripting;
using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Impl
{
    /// <summary>
    /// APIに関連するDynamicApiService
    /// </summary>
    internal partial class DynamicApiService
    {
        #region API
        /// <summary>
        /// apiIdに紐づくAPIを取得します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        /// <returns>API</returns>
        public ApiQueryModel GetApi(string apiId)
            => DynamicApiRepository.GetApi(apiId);

        /// <summary>
        /// URLに紐づくAPIを取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>API</returns>
        public ApiQueryModel GetApiFromUrl(string url)
        {
            // 引数が取得できない場合はBadRequest
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            // UrlをAPIとメソッドに分解する
            var (controllerUrl, apiUrl) = GetSplitResoureUrl(url);
            return DynamicApiRepository.GetApiFromUrl(controllerUrl, apiUrl);
        }

        /// <summary>
        /// APIを登録または更新します。
        /// </summary>
        /// <param name="model">APIモデル</param>
        /// <returns>APIモデル</returns>
        public ApiInformationModel RegistOrUpdateApi(ApiInformationModel model, bool IsTransparentApi = false)
        {
            var controller = DynamicApiRepository.GetController(model.ControllerId);
            var methodType = new HttpMethodType(model.MethodType);
            var actionType = ActionType.Parse(model.ActionType);

            // 透過APIでない場合
            if (!IsTransparentApi)
            {
                model.SecondaryRepositoryGroupIds?.ForEach(x => model.SecondaryRepositoryMapList.Add(new SecondaryRepositoryMapModel() { RepositoryGroupId = x, IsActive = true }));

                // 同一URLのAPIが存在するか
                var hasSameUrlApi = IsDuplicateApi(actionType, methodType, controller.Url, model.ControllerId, model.Url, model.ApiId, out string sameUrlApiId);
                var hasSameIdApi = DynamicApiRepository.ExistsEnableApi(model.ApiId);
                if (hasSameUrlApi && hasSameIdApi)
                {
                    if (sameUrlApiId.ToUpper() == model.ApiId.ToUpper())
                    {
                        // 既存APIをID指定で更新(URL変更なし)
                        model.ApiId = sameUrlApiId;
                    }
                    else
                    {
                        // 既存APIをID指定で更新(URL変更あり、コンフリクトあり)
                        throw new ConflictException("An api with the same url already exists.");
                    }
                }
                else if (hasSameIdApi)
                {
                    // 既存APIをID指定で更新(URL変更あり、コンフリクトなし)
                    // 何もしない
                }
                else if (hasSameUrlApi)
                {
                    if (string.IsNullOrWhiteSpace(model.ApiId))
                    {
                        // 既存APIをURL指定で更新
                        model.ApiId = sameUrlApiId;
                    }
                    else
                    {
                        // 新規APIの登録(コンフリクトあり)
                        // ※管理画面から既存のAPIと同じURLで新規作成する場合
                        throw new ConflictException("An api with the same url already exists.");
                    }
                }
                else
                {
                    // 新規APIの登録(コンフリクトなし)
                    model.ApiId ??= Guid.NewGuid().ToString();
                }
            }

            ValidateApi(model, controller, methodType, actionType);
            var result = DynamicApiRepository.UpsertApi(model);

            // ApiAccessVendor登録
            if (result.ApiAccessVendorList.Any())
            {
                result.ApiAccessVendorList = DynamicApiRepository.UpsertApiAccessVendor(result.ApiAccessVendorList, result.ApiId);
            }

            // ApiLink登録
            if (result.ApiLinkList.Any())
            {
                result.ApiLinkList = DynamicApiRepository.UpsertApiLink(result.ApiLinkList, result.ApiId);
            }

            // サンプルコード登録
            if (result.SampleCodeList.Any())
            {
                result.SampleCodeList = DynamicApiRepository.UpsertSampleCode(result.SampleCodeList, result.ApiId);
            }

            // OpenId認証局登録
            if (result.OpenIdCaList.Any())
            {
                result.OpenIdCaList.ToList().ForEach(x => DynamicApiRepository.UpsertApiOpenIdCa(x, result.ApiId));
            }

            // SecondaryRepositoryMap登録
            if (result.SecondaryRepositoryMapList.Any() && !DynamicApiRepository.IsDuplicateSecondaryRepositoryMap(result.SecondaryRepositoryMapList))
            {
                DynamicApiRepository.DeleteSecondaryRepositoryMap(result.ApiId);
                result.SecondaryRepositoryMapList = DynamicApiRepository.UpsertSecondaryRepositoryMapList(result.SecondaryRepositoryMapList.Where(p => p.IsActive == true), result.ApiId).ToList();
            }

            // マルチランゲージ登録
            // ※登録画面及びManageAPI未実装のため暫定でStaticAPIのみ対象
            if (result.ApiMultiLanguageList.Any() && result.IsStaticApi)
            {
                DynamicApiRepository.UpsertApiMultiLanguage(result.ApiMultiLanguageList, result.ApiId);
            }

            return result;
        }

        /// <summary>
        /// APIを削除します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        public void DeleteApi(string apiId)
        {
            // 引数が取得できない・Guidでない場合はBadRequest
            if (string.IsNullOrEmpty(apiId) || !Guid.TryParse(apiId, out _))
            {
                throw new ArgumentException("Can not get MethodId.");
            }

            var model = DynamicApiRepository.GetApi(apiId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }

            DynamicApiRepository.DeleteApi(apiId);
        }

        /// <summary>
        /// URLに紐づくAPIを削除します。
        /// </summary>
        /// <param name="url">URL</param>
        public void DeleteApiFromUrl(string url)
        {
            // 引数が取得できない場合はBadRequest
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            // UrlをAPIとメソッドに分解する
            var (controllerUrl, apiUrl) = GetSplitResoureUrl(url);

            var model = DynamicApiRepository.GetApiFromUrl(controllerUrl, apiUrl);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }

            DynamicApiRepository.DeleteApiFromUrl(controllerUrl, apiUrl, model.ApiId);
        }

        /// <summary>
        /// 引数のURLをApiUrlとMethodUrlに分解して取得します。
        /// </summary>
        /// <param name="originalUrl">URL</param>
        /// <returns>コントローラURLとAPIURL</returns>
        private (string ControllerUrl, string ApiUrl) GetSplitResoureUrl(string originalUrl)
        {
            var url = originalUrl.Split("/".ToCharArray());
            string controllerUrl = string.Empty;
            string apiUrl = string.Empty;

            for (int i = 0; i < url.Length; i++)
            {
                var item = url[i];
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                if (i != url.Length - 1)
                {
                    controllerUrl += "/" + item;
                }
                else
                {
                    apiUrl = item;
                }
            }
            return (controllerUrl, apiUrl);
        }

        /// <summary>
        /// 指定したAPIが重複しているかを判定する。
        /// </summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="httpMethodType">メソッドタイプ</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="apiUrl">API(メソッド)のURL</param>
        /// <param name="apiId">API(メソッド)のID。指定した場合は指定API以外との重複があるかどうかを判定する。</param>
        /// <returns>重複している場合はtrue、それ以外はfalse。</returns>
        public bool IsDuplicateApi(string actionType, string httpMethodType, string controllerId, string apiUrl, string apiId = null)
        {
            var controllerUrl = DynamicApiRepository.GetControllerUrl(controllerId);
            try
            {
                return IsDuplicateApi(ActionType.Parse(actionType), new HttpMethodType(httpMethodType), controllerUrl, controllerId, apiUrl, apiId, out var _);
            }
            catch (AlreadyExistsException)
            {
                // 異なるAPIのメソッドとURLが重複している場合(AlreadyExistsException)も重複扱い
                return true;
            }
        }

        /// <summary>
        /// APIが重複しているか
        /// </summary>
        /// <param name="sourceActionType">比較元アクションタイプ</param>
        /// <param name="sourceHttpMethodType">比較元メソッドタイプ</param>
        /// <param name="sourceControllerUrl">比較元コントローラーURL</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="sourceApiUrl">比較元API URL</param>
        /// <param name="sourceApiId">比較元対象APIID(新規ならNULL)</param>
        /// <param name="apiId">APIID</param>
        /// <returns>true:重複している/false:重複していない</returns>
        private bool IsDuplicateApi(ActionType sourceActionType, HttpMethodType sourceHttpMethodType, string sourceControllerUrl, string controllerId, string sourceApiUrl, string sourceApiId, out string apiId)
        {
            apiId = null;
            var targets = DynamicApiRepository.GetApiUrlIdentifier(sourceHttpMethodType, sourceControllerUrl);

            // 存在なし
            if (targets == null || !targets.Any())
            {
                return false;
            }

            var sourceUrl = $"{sourceControllerUrl}/{sourceApiUrl}";
            // 重複チェック
            foreach (var target in targets)
            {
                var targetUrl = $"{target.ControllerUrl}/{target.ApiUrl}";
                var targetActionType = ActionType.Parse(target.ActionType);

                // メソッドタイプとURLの一致で比較
                // どちらかがGatewayの場合はURLパラメータの差異は無視する
                // (Gatewayは受け取ったパラメータをそのまま転送するため)
                var ignoreParam = (sourceActionType.Value == ActionTypes.GateWay || targetActionType.Value == ActionTypes.GateWay);
                if (target.ApiId.ToUpper() != sourceApiId?.ToUpper() && CompareUrl(sourceUrl, targetUrl, ignoreParam))
                {
                    // 異なるAPIのメソッドとURLが重複している場合は受け付けない
                    // ex) /API/XXX/YYY + Get 
                    //     /API/XXX + YYY/Get
                    if (controllerId.ToLower() != target.ControllerId.ToLower())
                    {
                        throw new AlreadyExistsException($"指定したURLは既に使用されています。URL={sourceUrl}");
                    }

                    // コントローラーが一致する場合はAPIの更新となるためAPIIDを返却する
                    apiId = target.ApiId;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// URLを比較します。
        /// </summary>
        /// <param name="source">比較元URL</param>
        /// <param name="target">ターゲットURL</param>
        /// <param name="ignoreParam">パラメータを無視するか</param>
        /// <returns></returns>
        private bool CompareUrl(string source, string target, bool ignoreParam)
        {
            // パラメータ値を無視して比較する
            const string variablePattern = "{.*?}";

            var sourceUrlAndParam = Regex.Replace(source, variablePattern, "");
            var sourceUrl = UrlUtil.GetUrlNonQuery(sourceUrlAndParam);
            var sourceParam = UrlUtil.GetParameterKeyList(sourceUrlAndParam);

            var targetUrlAndPram = Regex.Replace(target, variablePattern, "");
            var targetUrl = UrlUtil.GetUrlNonQuery(targetUrlAndPram);
            var targetParam = UrlUtil.GetParameterKeyList(targetUrlAndPram);

            // URLセグメントの比較
            // 実行時には大文字・小文字が区別されるが重複チェック時は区別しない
            if (sourceUrl.ToLower() != targetUrl.ToLower())
            {
                return false;
            }

            // パラメータ無視の場合はURLセグメントの比較のみ
            if (ignoreParam)
            {
                return true;
            }

            // パラメータは順番無視して比較
            // 実行時には大文字・小文字が区別されるが重複チェック時は区別しない
            if (sourceParam != null && targetParam != null)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(
                    sourceParam.Select(x => x.ToLower()).Distinct().OrderBy(x => x).ToArray(),
                    targetParam.Select(x => x.ToLower()).Distinct().OrderBy(x => x).ToArray());
            }
            else
            {
                return (sourceParam == null && targetParam == null);
            }
        }

        /// <summary>
        /// Api Register・Updateの共通バリデーション
        /// </summary>
        /// <returns></returns>
        private void ValidateApi(ApiInformationModel model, ControllerInformationModel controller, HttpMethodType methodType, ActionType actionType)
        {
            if (!string.IsNullOrEmpty(model.Query) && !IsQueryFormat(model.Query))
            {
                throw new QuerySyntaxErrorException(model.Query);
            }

            if (model.IsOtherResourceSqlAccess && !IsRdbRepository(model.RepositoryGroupId))
            {
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E60402);
                throw new InvalidDataException(msg.Title + Environment.NewLine + msg.Detail);
            }

            if (model.IsHeaderAuthentication && model.IsClientCertAuthentication)
            {
                throw new InvalidDataException("IsHeaderAuthentication and IsClientCertAuthentication cannot be enabled at the same time.");
            }

            // リクエストモデルの定義済みのキーワードをチェック
            if (!string.IsNullOrEmpty(model.RequestSchemaId))
            {
                var dataSchema = DataSchemaRepository.GetDataSchema(model.RequestSchemaId);
                if (HasDefinedKeyword(dataSchema, out var errorKeywords))
                {
                    throw new ArgumentException($"Has Defined Keywords. Keyword ={string.Join(",", errorKeywords)}");
                }
            }

            if (!model.IsTransparentApi && IsReserveAttachFileUrl(model.Url) && IsReserveBlockchainUrl(model.Url) && IsReserveAttachfileWithBlockchainUrl(model.Url))
            {
                throw new ArgumentException($"{model.Url}のURLはシステム予約されているため指定できません。", "Url");
            }

            if (actionType.Value != ActionTypes.GateWay && !model.IsTransparentApi && controller.IsContainerDynamicSeparation)
            {
                var isValidPrimary = DynamicApiRepository.HasContainerDynamicSeparationSecondaryRepositoryMap(model.ApiId, model.VendorId, model.RepositoryGroupId);

                // コンテナ分離対応できないリポジトリが選択されていないか
                var isValidSecondary = !model.SecondaryRepositoryMapList.Any(m => m.IsActive && !m.IsContainerDynamicSeparation);

                if (!isValidPrimary || !isValidSecondary)
                {
                    throw new NotFoundException("ContainerDynamicSeparationRepository NotFound");
                }
            }

            var isContainTransparentApi = ApiListConst.AllTransparentApiList.Any(m => m.MethodType == methodType.Value && m.MethodName == model.Url);
            if ((model.IsTransparentApi && !isContainTransparentApi) || (!model.IsTransparentApi && isContainTransparentApi))
            {
                throw new InvalidDataException(DynamicApiMessages.IsTransparentApiIncorrect);
            }

            if (model.IsTransparentApi && model.ApiId != null)
            {
                // QueryTypeとScriptTypeが変更されたらエラー
                var apiMethod = DynamicApiRepository.GetApiInformation(model.ApiId);
                if (apiMethod != null)
                {
                    if (model.QueryType != apiMethod.QueryType || model.ScriptType != apiMethod.ScriptType)
                    {
                        throw new InvalidDataException(DynamicApiMessages.IsTransparentApiIncorrect);
                    }
                }
            }

            if (!controller.IsStaticApi && !model.IsTransparentApi)
            {
                // DataLakeStoreのリポジトリでモデルが存在していない場合エラー
                if (HasDataLakeStore(model.RepositoryGroupId, model.SecondaryRepositoryMapList)
                    && DataSchemaRepository.GetDataSchemaIdByControllerId(model.ControllerId) == null)
                {
                    throw new AlreadyExistsException();
                }

                // APIが実行可能か
                if (!DynamicApiRepository.IsExcuseApiCombinationConstraints(actionType, methodType, model.RepositoryGroupId))
                {
                    throw new InvalidDataException();
                }

                // Gateway以外の場合、リポジトリキーが登録されている必要あり
                if (actionType.Value != ActionTypes.GateWay && string.IsNullOrEmpty(controller.RepositoryKey))
                {
                    throw new ArgumentNullException(nameof(controller.RepositoryKey));
                }
            }

            // スキーマがある場合のクエリのバリデーションチェック
            if (!string.IsNullOrEmpty(model.UrlSchemaId))
            {
                if (!ValidateUrlParam(model.Url, model.UrlSchemaId, out string paramName) ||
                    !ValidateQueryParam(model.Query, model.Url, model.UrlSchemaId, out paramName))
                {
                    throw new NotFoundException(paramName);
                }
            }

            // スクリプトチェック
            CheckScriptSyntax(model.Script, model.ScriptType);
        }

        /// <summary>
        /// アクションタイプ、メソッドタイプ、リポジトリグループの組み合わせでAPIが実行可能かを判定する。
        /// </summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="methodType">メソッドタイプ</param>
        /// <param name="repositoryGroupId">リポジトリグループID</param>
        /// <returns>APIが実行可能な場合はtrue、それ以外はfalse。</returns>
        public bool IsExcuseApiCombinationConstraints(string actionType, string methodType, string repositoryGroupId)
            => DynamicApiRepository.IsExcuseApiCombinationConstraints(ActionType.Parse(actionType), new HttpMethodType(methodType), repositoryGroupId);

        /// <summary>
        /// クエリ形式か
        /// </summary>
        /// <param name="query">クエリ</param>
        /// <returns>true:クエリ形式/false:クエリ形式ではない</returns>
        private bool IsQueryFormat(string query) => !Regex.IsMatch(query, "^(\r\n)+$");

        /// <summary>
        /// リポジトリがRBDかどうか
        /// </summary>
        /// <returns>true:RDB/false:RDBではない</returns>
        private bool IsRdbRepository(string repositoryGroupId)
        {
            var rdbTypeCdList = new string[]{
                RepositoryInfo.RepositoryType.ss2.ToString(),
                RepositoryInfo.RepositoryType.ssd.ToString(),
                RepositoryInfo.RepositoryType.ora.ToString()
            };

            return RepositoryGroupRepository.MatchRepositoryType(repositoryGroupId, rdbTypeCdList);
        }

        /// <summary>
        /// Attachfileで利用するURLのリスト
        /// </summary>
        private static readonly string[] AttachfileApiUrlList = new string[] { "CreateAttachFile", "UploadAttachFile", "GetAttachFile", "DeleteAttachFile", "GetAttachFileMeta", "GetAttachFileMetaList" };

        /// <summary>
        /// Blockchainで利用するURLのリスト
        /// </summary>
        private static readonly string[] BlockchainApiUrlList = new string[] { "ValidateWithBlockchain" };

        /// <summary>
        /// Blockchain(Attachfile併用)で利用するURLのリスト
        /// </summary>
        private static readonly string[] AttachfileWithBlockchainApiUrlList = new string[] { "ValidateAttachFileWithBlockchain" };

        /// <summary>
        /// AttachFileで予約されているURLか
        /// </summary>
        public static bool IsReserveAttachFileUrl(string url)
        {
            if (url == null)
            {
                return false;
            }
            var queryStringSplit = url.Split('?');
            var urlSplit = queryStringSplit[0].Split('/');
            return AttachfileApiUrlList.Contains(urlSplit[0]);
        }

        /// <summary>
        /// Blockchainで予約されているURLか
        /// </summary>
        public static bool IsReserveBlockchainUrl(string url)
        {
            if (url == null)
            {
                return false;
            }
            var queryStringSplit = url.Split('?');
            var urlSplit = queryStringSplit[0].Split('/');
            return BlockchainApiUrlList.Contains(urlSplit[0]);
        }

        /// <summary>
        /// Blockchain(AttachFile併用)で予約されているURLか
        /// </summary>
        public static bool IsReserveAttachfileWithBlockchainUrl(string url)
        {
            if (url == null)
            {
                return false;
            }
            var queryStringSplit = url.Split('?');
            var urlSplit = queryStringSplit[0].Split('/');
            return AttachfileWithBlockchainApiUrlList.Contains(urlSplit[0]);
        }

        /// <summary>
        /// リポジトリグループがDataLakeStoreのリポジトリが含まれているか
        /// </summary>
        /// <param name="repositoryGroupId">repositoryGroupId</param>
        /// <param name="secondaryRepositoryMapList">secondaryRepositoryMapList</param>
        /// <returns></returns>
        private bool HasDataLakeStore(string repositoryGroupId, IList<SecondaryRepositoryMapModel> secondaryRepositoryMapList)
        {
            // プライマリのリポジトリグループIDを追加
            List<string> repositoryGroupIdList = new() { };
            if(!string.IsNullOrEmpty(repositoryGroupId))
            {
                repositoryGroupIdList.Add(repositoryGroupId);
            }
            // セカンダリのリポジトリグループIDを追加
            if (secondaryRepositoryMapList != null)
            {
                foreach (var secondaryRepositoryMap in secondaryRepositoryMapList)
                {
                    if (secondaryRepositoryMap.IsActive)
                    {
                        repositoryGroupIdList.Add(secondaryRepositoryMap.RepositoryGroupId);
                    }
                }
            }
            if (!repositoryGroupIdList.Any())
            {
                return false;
            }
            return DynamicApiRepository.MatchDataLakeStoreId(repositoryGroupIdList);
        }

        /// <summary>
        /// クエリ内のパラメータの検証
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="apiUrl">The API URL.</param>
        /// <param name="requestSchemaId">The request schema identifier.</param>
        /// <param name="errorParamName">Name of the error parameter.</param>
        /// <returns></returns>
        private bool ValidateQueryParam(string query, string apiUrl, string requestSchemaId, out string errorParamName)
        {
            errorParamName = string.Empty;

            // クエリが存在しない場合は検証しない
            if (string.IsNullOrEmpty(query)) return true;

            var parameters = GetParameters(query, true).Distinct();

            // パラメータが存在しない場合は検証しない
            if (!parameters.Any()) return true;

            // QueryStringから変数名(左辺)を取得
            List<string> keysQueryString = new();
            if (!string.IsNullOrEmpty(apiUrl))
            {
                int pos = apiUrl.IndexOf('?');
                if (pos != -1)
                {
                    string queryString = apiUrl[(pos + 1)..];
                    keysQueryString.AddRange(System.Web.HttpUtility.ParseQueryString(queryString).AllKeys);
                }
            }

            var schema = DataSchemaRepository.GetDataSchema(requestSchemaId);
            List<string> validErrorResult = new();
            foreach (var parameter in parameters)
            {
                // このチェックはQueryStringの右辺を見ているんだろうけど、厳密ではない
                // 右辺のものだけを拾ってチェックするべき
                // これだと次の例がダメ
                // Get?x={id}{123}&hoge={hoge}
                // など。構文上このような指定ができないけどね（厳密性という意味）
                if (apiUrl.Contains($"{{{parameter}}}"))
                {
                    continue;
                }

                // parameter名がQueryStringの左辺にあればそれはOK（DynamicAPIで正しく処理しているため、バリデーションルールを追加
                if (keysQueryString.Contains(parameter))
                {
                    continue;
                }

                if (schema != null)
                {
                    if (schema.GetJSchema().Properties.Any(x => x.Key == parameter))
                    {
                        continue;
                    }
                }

                validErrorResult.Add(parameter);
            }

            if (!validErrorResult.Any()) return true;

            errorParamName = string.Join("、", validErrorResult.Select(x => $"「{x.Replace("{", string.Empty).Replace("}", string.Empty)}」").ToArray());
            return false;

        }

        /// <summary>
        /// APIのURL内のパラメータがURLスキーマのプロパティに定義されているかを検証します。
        /// </summary>
        /// <param name="url">検証対象API URL</param>
        /// <param name="urlSchemaId">URLスキーマId</param>
        /// <param name="errorParamName">検証エラーが起きたパラメータ名</param>
        /// <returns>true:検証OK/false:検証NG</returns>
        private bool ValidateUrlParam(string url, string urlSchemaId, out string errorParamName)
        {
            errorParamName = string.Empty;
            var parameters = GetParameters(url, false).Distinct();

            // パラメータが存在しない場合は検証しない
            if (!parameters.Any()) return true;

            var schema = DataSchemaRepository.GetDataSchema(urlSchemaId);
            List<string> validErrorResult = new();
            foreach (var parameter in parameters)
            {
                if (schema.GetJSchema() != null && schema.GetJSchema().Properties?.Any(x => x.Key == parameter) == false)
                {
                    validErrorResult.Add(parameter);
                }
            }

            if (!validErrorResult.Any()) return true;

            errorParamName = string.Join("、", validErrorResult.Select(x => $"「{x.Replace("{", string.Empty).Replace("}", string.Empty)}」").ToArray());
            return false;
        }

        /// <summary>
        /// パラメータを取得します。
        /// </summary>
        /// <param name="targetString">対象文字列</param>
        /// <param name="isQuery">クエリか</param>
        /// <returns>パラメータリスト</returns>
        private IEnumerable<string> GetParameters(string targetString, bool isQuery)
        {
            // 事前にRDBMSのテーブル結合用URL内部の"{xxx}"を除外
            var resourceNamePattern = @"Resource\((?<url>/API/[^?#]+?)(?<querystring>\?([^#])*?)*?\)";
            var resourceParameters = new Regex(resourceNamePattern).Matches(targetString).Cast<Match>().Distinct();
            foreach (var resourceParameter in resourceParameters)
            {
                targetString = targetString.Replace(resourceParameter.Value, "");
            }

            // パラメータを抽出
            var pattern = @"{[-_0-9a-zA-Z]+}";
            var parameters = new Regex(pattern)
                .Matches(targetString)
                .Cast<Match>().Select(x => x.Value.Replace("{", string.Empty).Replace("}", string.Empty));

            if (isQuery)
            {
                return DynamicApiRepository.GetParameters(parameters);
            }

            return parameters;
        }

        /// <summary>
        /// スクリプトの構文チェックを行います。
        /// </summary>
        /// <param name="script">スクリプト</param>
        /// <param name="scriptType">スクリプトタイプ</param>
        private void CheckScriptSyntax(string script, string scriptType)
        {
            var errorMessage = GetScriptSyntaxCheckMessage(script, scriptType);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new InvalidDataException(errorMessage);
            }
        }

        /// <summary>
        /// スクリプト構文チェックメッセージを取得します。
        /// </summary>
        /// <param name="script">スクリプト</param>
        /// <param name="scriptType">スクリプトタイプ</param>
        /// <returns>メッセージ</returns>
        public string GetScriptSyntaxCheckMessage(string script, string scriptType)
        {
            string message = string.Empty;
            // ScruptTypeとScriptが入力されている場合、Scriptをコンパイルする。
            if (!string.IsNullOrEmpty(script) && !string.IsNullOrEmpty(scriptType))
            {
                
                if (scriptType == ScriptType.RoslynScript.ToCode())
                {
                    // RoslynScript
                    var scriptCompiler = UnityCore.Resolve<IScriptingCompiler>(scriptType);
                    message = scriptCompiler.Compile<HttpResponseMessage>(script, new ScriptArgumentParameters());
                }
                else if (scriptType == ScriptType.EtlJsonDefinition.ToCode())
                {
                    // EtlJsonDefinitionは.NET6版でスコープアウトになったので何もしない
                }
            }

            return message;
        }

        #endregion
    }
}
