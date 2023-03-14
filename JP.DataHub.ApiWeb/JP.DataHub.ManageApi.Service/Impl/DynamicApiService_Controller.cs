using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Consts;
using JP.DataHub.ManageApi.Service.DymamicApi;
using JP.DataHub.ManageApi.Service.DymamicApi.Factory;
using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Impl
{
    /// <summary>
    /// コントローラに関連するDynamicApiService
    /// </summary>
    internal partial class DynamicApiService
    {
        #region Controller
        /// <summary>
        /// ベンダーに紐づくのコントローラとAPI情報のリストを取得します。
        /// </summary>
        /// <param name="isAll">全てのベンダーのリソースを取得するか</param>
        /// <returns>リソースのリスト</returns>
        public IEnumerable<ControllerQueryModel> GetControllerApiList(bool isAll)
        {
            // 全取得は運用管理ベンダーのみ
            if (isAll && !PerRequestDataContainer.IsOperatingVendorUser)
            {
                throw new AccessDeniedException();
            }

            var result = DynamicApiRepository.GetControllerApiList(PerRequestDataContainer.VendorId, isAll);
            if (result == null)
            {
                throw new NotFoundException();
            }

            return result;
        }

        /// <summary>
        /// ベンダーに紐づくのコントローラとAPI情報のリストを取得します。（内容はシンプルなもの、管理画面のツリーを表示するための項目のみ）
        /// </summary>
        /// <param name="vendorId">ベンダーID。</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>
        /// ベンダーに紐づくリソースのリスト。
        /// <paramref name="vendorId"/>がnullの場合、運用管理ベンダーであれば全ベンダーのリソースのリスト、それ以外のベンダーは自ベンダーのリソースのリスト。
        /// </returns>
        public IEnumerable<ControllerSimpleQueryModel> GetControllerApiSimpleList(string vendorId = null, bool isTransparent = true)
        {
            if (!PerRequestDataContainer.IsOperatingVendorUser)
            {
                if (vendorId == null)
                {
                    // 運用管理ベンダー以外でベンダーIDがnullの場合は自ベンダーのリソースを返す
                    vendorId = PerRequestDataContainer.VendorId;
                }
                else if (vendorId != PerRequestDataContainer.VendorId)
                {
                    // 運用管理ベンダー以外で他ベンダーのIDを指定された場合はエラー
                    throw new AccessDeniedException();
                }
            }
            
            return DynamicApiRepository.GetControllerApiSimpleList(vendorId, vendorId == null, isTransparent);
        }

        /// <summary>
        /// ControllerIdに紐づくリソースを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        public ControllerQueryModel GetControllerResourceFromControllerId(string controllerId, bool isTransparent)
            => DynamicApiRepository.GetControllerResourceFromControllerId(controllerId, isTransparent);

        /// <summary>
        /// ControllerIdに紐づくコントローラ情報を取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>リソース</returns>
        public ControllerLightQueryModel GetControllerResourceLight(string controllerId)
            => DynamicApiRepository.GetControllerResourceLight(controllerId);

        /// <summary>
        /// URLに紐づくコントローラ情報を取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        public ControllerQueryModel GetControllerResourceFromUrl(string url, bool isTransparent)
        {
            // 引数が取得できない場合はBadRequest
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            return DynamicApiRepository.GetControllerResourceFromUrl(url, isTransparent);
        }

        /// <summary>
        /// コントローラを登録または更新します。
        /// </summary>
        /// <param name="model">コントローラモデル</param>
        /// <returns>コントローラモデル</returns>
        public ControllerInformationModel RegistOrUpdateController(ControllerInformationModel model)
        {
            var isNew = false;

            // 同一URLのコントローラが存在するか
            var hasSameUrlController = IsDuplicateController(model, out string sameUrlControllerId);
            var hasSameIdController = DynamicApiRepository.IsExists(model.ControllerId);
            if (hasSameUrlController && hasSameIdController)
            {
                if (sameUrlControllerId.ToUpper() == model.ControllerId.ToUpper())
                {
                    // 既存コントローラをID指定で更新(URL変更なし)
                    model.ControllerId = sameUrlControllerId;
                }
                else
                {
                    // 既存コントローラをID指定で更新(URL変更あり、コンフリクトあり)
                    throw new ConflictException("An api with the same url already exists.");
                }
            }
            else if (hasSameIdController)
            {
                // 既存コントローラをID指定で更新(URL変更あり、コンフリクトなし)
                // 何もしない
            }
            else if (hasSameUrlController)
            {
                if (string.IsNullOrWhiteSpace(model.ControllerId))
                {
                    // 既存APIをURL指定で更新
                    model.ControllerId = sameUrlControllerId;
                }
                else
                {
                    // 新規コントローラの登録(コンフリクトあり)
                    // ※管理画面から既存のコントローラと同じURLで新規作成する場合
                    throw new ConflictException("An api with the same url already exists.");
                }
            }
            else
            {
                // 新規APIの登録(コンフリクトなし)
                model.ControllerId ??= Guid.NewGuid().ToString();
                isNew = true;
            }

            ValidateController(model);
            var result = isNew ? RegistController(model) : UpdateController(model);

            return result;
        }

        /// <summary>
        /// コントローラを削除します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        public void DeleteController(string controllerId)
        {
            var model = DynamicApiRepository.GetControllerResourceFromControllerId(controllerId, false);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }

            DynamicApiRepository.DeleteController(controllerId);
        }

        /// <summary>
        /// URLに紐づくコントローラを削除します。
        /// </summary>
        /// <param name="url">URL</param>
        public void DeleteControllerFromUrl(string url)
        {
            var model = DynamicApiRepository.GetControllerResourceFromUrl(url, false);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }

            DynamicApiRepository.DeleteControllerFromUrl(url, model.ControllerId);
        }


        /// <summary>
        /// コントローラを登録します。
        /// </summary>
        /// <param name="model">コントローラモデル</param>
        /// <returns>コントローラモデル</returns>
        private ControllerInformationModel RegistController(ControllerInformationModel model)
        {
            // コントローラ登録
            var result = DynamicApiRepository.UpsertController(model);

            // ControllerCategory登録
            if (result.CategoryList.Any())
            {
                result.CategoryList = DynamicApiRepository.UpsertControllerCategoryList(result.ControllerId, result.CategoryList);
            }

            // ControllerCommonIpFilterGroup登録
            if (result.ControllerCommonIpFilterGroupList.Any())
            {
                result.ControllerCommonIpFilterGroupList = DynamicApiRepository.UpsertControllerCommonIpFilterGroupList(result.ControllerCommonIpFilterGroupList, result.ControllerId);
            }

            // ControllerIpFilter登録
            if (result.ControllerIpFilterList.Any())
            {
                result.ControllerIpFilterList = DynamicApiRepository.UpsertControllerIpFilter(result.ControllerIpFilterList, result.ControllerId);
            }

            // ControllerTag登録
            if (result.ControllerTagInfoList.Any())
            {
                result.ControllerTagInfoList = DynamicApiRepository.UpsertControllerTagInfoList(result.ControllerId, result.ControllerTagInfoList);
            }

            // ControllerField登録
            if (result.ControllerFieldInfoList.Any())
            {
                result.ControllerFieldInfoList = DynamicApiRepository.UpsertControllerFieldInfoList(result.ControllerId, result.ControllerFieldInfoList);
            }

            // OpenId認証局登録
            if (result.OpenIdCaList.Any())
            {
                result.OpenIdCaList = result.OpenIdCaList.Select(x => DynamicApiRepository.UpsertControllerOpenIdCa(x, result.ControllerId)).ToList();
            }

            // マルチランゲージ登録
            // ※登録画面及びManageAPI未実装のため暫定でStaticAPIのみ対象
            if (result.IsStaticApi && result.ControllerMultiLanguageList.Any())
            {
                result.ControllerMultiLanguageList = DynamicApiRepository.RegisterControllerMultiLanguage(result.ControllerId, result.ControllerMultiLanguageList);
            }

            // 透過API作成
            if (!result.IsStaticApi)
            {
                foreach (var transparentApi in ApiListConst.BaseTransparentApiList)
                {
                    var api = CreateTransparentApi(transparentApi, model.VendorId, model.Url, model.ControllerId);
                    var methodType = new HttpMethodType(api.MethodType);
                    var actionType = ActionType.Parse(api.ActionType);
                    ValidateApi(api, result, methodType, actionType);
                    DynamicApiRepository.UpsertApi(api);
                }


                // 履歴用の透過APIを作成
                if (model.DocumentHistorySettings?.IsEnable ?? false)
                {
                    var transparentApis = DocumentHistoryTransparentApiFactory.CreateDocumentHistoryTransparentApiInformation(model, OpenIdConnectMustBeUsed);
                    if (UnityCore.Resolve<bool>("UseApiAttachFileDocumentHistory") && (model.AttachFileSettings?.IsEnable ?? false))
                    {
                        transparentApis.AddRange(AttachFileDocumentHistoryTransparentApiFactory.CreateDocumentHistoryTransparentApiInformation(model, OpenIdConnectMustBeUsed));
                    }

                    foreach (var api in transparentApis)
                    {
                        SecondaryRepositoryMapModel secondaryRepositoryMap = new()
                        {
                            SecondaryRepositoryMapId = Guid.NewGuid().ToString(),
                            RepositoryGroupId = model.DocumentHistorySettings.HistoryRepositoryId,
                            ApiId = api.ApiId,
                            IsActive = true
                        };

                        api.SecondaryRepositoryMapList.Add(secondaryRepositoryMap);
                        RegistOrUpdateApi(api, true);
                    }
                };

                // AttachFile用の透過APIを作成
                if (model.AttachFileSettings?.IsEnable ?? false)
                {
                    foreach (var api in AttachfileTransparentApiFactory.CreateAttachfileTransparentApiInformation(model, OpenIdConnectMustBeUsed))
                    {
                        RegistOrUpdateApi(api, true);
                    }
                }

                // Blockchain用の透過APIを作成
                if (model.IsEnableBlockchain)
                {
                    foreach (var api in BlockchainTransparentApiFactory.CreateBlockchainTransparentApiInformation(model, OpenIdConnectMustBeUsed))
                    {
                        RegistOrUpdateApi(api, true);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// コントローラを更新します。
        /// </summary>
        /// <param name="model">コントローラモデル</param>
        /// <returns>コントローラモデル</returns>
        private ControllerInformationModel UpdateController(ControllerInformationModel model)
        {
            // 運用管理ベンダーかつ設定で許可された場合を除き、ベンダー依存と個人依存は変更できないようにする
            if (!PerRequestDataContainer.IsOperatingVendorUser || !AllowResourceDependencyChangeByManageApi)
            {
                var controllerHeader = GetControllerHeader(model.VendorId, model.ControllerId);

                // 運用管理ベンダーが依存設定を変更する場合はエラーとする
                if (PerRequestDataContainer.IsOperatingVendorUser &&
                    (model.IsVendor != controllerHeader.Controller.IsVendor ||
                     model.IsPerson != controllerHeader.Controller.IsPerson))
                {
                    throw new AccessDeniedException($"Resource dependency can not be changed. (IsVendor:{controllerHeader.Controller.IsVendor}, IsPerson:{controllerHeader.Controller.IsPerson}) => (IsVendor:{model.IsVendor}, IsPerson:{model.IsPerson})");
                }
                else
                {
                    model.IsVendor = controllerHeader.Controller.IsVendor;
                    model.IsPerson = controllerHeader.Controller.IsPerson;
                }
            }

            if (!model.IsStaticApi)
            {
                // DataLakeStoreチェック
                if (!DynamicApiRepository.CheckDataLakeStoreMethod(model.ControllerId, model.ControllerSchemaId))
                    throw new AlreadyExistsException();

                // Gateway以外のAPIが存在する場合、RepositoryKeyは必須
                var apiList = DynamicApiRepository.GetControllerApi(model.ControllerId);
                if (apiList.Where(x => x.action_type_cd != ActionTypesEx.GetCode(ActionTypes.GateWay) && (!x.is_transparent_api)).Any() && string.IsNullOrEmpty(model.RepositoryKey))
                    throw new AlreadyExistsException();
            }

            // コントローラ更新
            var result = DynamicApiRepository.UpsertController(model);

            // カテゴリ更新
            if (result.CategoryList.Any())
            {
                result.CategoryList = DynamicApiRepository.UpsertControllerCategoryList(result.ControllerId, result.CategoryList);
            }

            // ControllerCommonIpFilterGroup登録
            if (result.ControllerCommonIpFilterGroupList.Any())
            {
                result.ControllerCommonIpFilterGroupList = DynamicApiRepository.UpsertControllerCommonIpFilterGroupList(result.ControllerCommonIpFilterGroupList, result.ControllerId);
            }

            // ControllerIpFilter登録
            if (result.ControllerIpFilterList.Any())
            {
                result.ControllerIpFilterList = DynamicApiRepository.UpsertControllerIpFilter(result.ControllerIpFilterList, result.ControllerId);
                result.ControllerIpFilterList.RemoveAll(x => !x.IsActive);
            }

            // ControllerTag登録
            if (result.ControllerTagInfoList.Any())
            {
                result.ControllerTagInfoList = DynamicApiRepository.UpsertControllerTagInfoList(result.ControllerId, result.ControllerTagInfoList);
            }

            // ControllerField登録
            if (result.ControllerFieldInfoList.Any())
            {
                result.ControllerFieldInfoList = DynamicApiRepository.UpsertControllerFieldInfoList(result.ControllerId, result.ControllerFieldInfoList);
            }

            // OpenId認証局登録
            if (result.OpenIdCaList.Any())
            {
                result.OpenIdCaList = result.OpenIdCaList.Select(x => DynamicApiRepository.UpsertControllerOpenIdCa(x, result.ControllerId)).ToList();
            }

            // マルチランゲージ登録
            // ※登録画面及びManageAPI未実装のため暫定でStaticAPIのみ対象
            if (result.IsStaticApi && result.ControllerMultiLanguageList.Any())
            {
                DynamicApiRepository.RegisterControllerMultiLanguage(result.ControllerId, result.ControllerMultiLanguageList);
            }

            if (!result.IsStaticApi)
            {
                //AttachFile用の透過APIが存在しない場合は作成する
                if (result.AttachFileSettings?.IsEnable ?? false)
                {
                    foreach (var api in AttachfileTransparentApiFactory.CreateAttachfileTransparentApiInformation(result, OpenIdConnectMustBeUsed))
                    {
                        if (DynamicApiRepository.IsDuplicateUrl(out string registedApiId, result.ControllerId, api.Url))
                        {
                            var registedApi = DynamicApiRepository.GetAllFieldApiInformation(registedApiId);
                            registedApi.RepositoryGroupId = api.RepositoryGroupId;
                            //一旦フラグを全部折る、必要な項目を埋める
                            registedApi.SecondaryRepositoryMapList.ToList().ForEach(x => {
                                x.IsActive = false;
                                x.ApiId = registedApi.ApiId;
                            });
                            //AttachFileBlobに指定されているRepositoryIdのIsActiveを立てる
                            registedApi.SecondaryRepositoryMapList.Where(x => x.RepositoryGroupId.ToLower() == result.AttachFileSettings.BlobRepositoryId.ToLower()).First().IsActive = true;
                            RegistOrUpdateApi(registedApi, true);
                        }
                        else
                        {
                            RegistOrUpdateApi(api, true);
                        }

                    }
                }

                //Blockchain用の透過APIが存在しない場合は作成する
                if (result.IsEnableBlockchain == true)
                {
                    foreach (var api in BlockchainTransparentApiFactory.CreateBlockchainTransparentApiInformation(result, OpenIdConnectMustBeUsed))
                    {
                        if (DynamicApiRepository.IsDuplicateUrl(out string registedApiId, result.ControllerId, api.Url))
                        {
                            var registedApi = DynamicApiRepository.GetAllFieldApiInformation(registedApiId);
                            registedApi.RepositoryGroupId = api.RepositoryGroupId;
                            RegistOrUpdateApi(registedApi, true);
                        }
                        else
                        {
                            RegistOrUpdateApi(api, true);
                        }
                    }
                }

                //履歴用の透過APIが存在しない場合は作成する
                if (result.DocumentHistorySettings?.IsEnable == true)
                {
                    var transparentApis = DocumentHistoryTransparentApiFactory.CreateDocumentHistoryTransparentApiInformation(result, OpenIdConnectMustBeUsed);
                    if (UnityCore.Resolve<bool>("UseApiAttachFileDocumentHistory") && result.AttachFileSettings?.IsEnable == true)
                    {
                        transparentApis.AddRange(AttachFileDocumentHistoryTransparentApiFactory.CreateDocumentHistoryTransparentApiInformation(result, OpenIdConnectMustBeUsed));
                    }
                    foreach (var api in transparentApis)
                    {
                        if (DynamicApiRepository.IsDuplicateUrl(out string registedApiId, result.ControllerId, api.Url))
                        {
                            var registedApi = DynamicApiRepository.GetAllFieldApiInformation(registedApiId);
                            //一旦フラグを全部折る、必要な項目を埋める
                            registedApi.SecondaryRepositoryMapList.ToList().ForEach(x => {
                                x.IsActive = false;
                                x.ApiId = registedApi.ApiId;
                            });
                            //DocumentHistoryに指定されているRepositoryIdのIsActiveを立てる
                            registedApi.SecondaryRepositoryMapList.Where(x => x.RepositoryGroupId.ToLower() == result.DocumentHistorySettings.HistoryRepositoryId.ToLower()).First().IsActive = true;
                            RegistOrUpdateApi(registedApi, true);
                        }
                        else
                        {
                            RegistOrUpdateApi(api, true);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 登録・更新共通のコントローラ検証を行います。
        /// </summary>
        /// <param name="model">モデル</param>
        /// <remarks>API化のため、画面側と似た内容の検証を行う</remarks>
        private void ValidateController(ControllerInformationModel model)
        {
            if (!DynamicApiRepository.CheckVendorSystemCombination(model.VendorId, model.SystemId))
            {
                throw new ArgumentException("指定されたVendorIdに所属するSystemIdを指定してください。", "SystemId");
            }

            var url = DynamicApiRepository.GetControllerUrl(model.ControllerId);

            if (!string.IsNullOrEmpty(model.Url))
            {
                if (!PerRequestDataContainer.IsOperatingVendorUser && !model.Url.Equals(url ?? string.Empty))
                {
                    var vendorApiPrefix = UnityCore.Resolve<string>("VendorApiPrefix");
                    if (!model.Url.StartsWith(vendorApiPrefix ?? string.Empty))
                    {
                        throw new ArgumentException($"URLは「{vendorApiPrefix}」から始めてください。", "Url");
                    }
                }
            }

            // 動的Container分離はベンダー依存・個人依存(片方or両方)のみ許可
            if (model.IsContainerDynamicSeparation && (!model.IsVendor && !model.IsPerson))
            {
                throw new InvalidDataException(DynamicApiMessages.ContainerDynamicSeparationRequiredDependency);
            }

            // 同意ありの場合は利用規約必須
            if (model.IsVisibleAgreement && string.IsNullOrEmpty(model.AgreeDescription))
            {
                throw new ArgumentException(DynamicApiMessages.InvalidResourceAgreement_Detail, "AgreeDescription");
            }

            // リポジトリキーまたはパーティションキーに、モデルの論理キーが含まれているか
            if (!model.IsStaticApi && !string.IsNullOrEmpty(model.ControllerSchemaId))
            {
                ContainsLogicalKey(model.ControllerSchemaId, EditLogicalKeys(model.RepositoryKey));
                ContainsLogicalKey(model.ControllerSchemaId, EditLogicalKeys(model.PartitionKey));
            };
        }

        /// <summary>
        /// 透過APIモデルを作成します。
        /// </summary>
        /// <param name="transparentApi">透過API</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="url">URL</param>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>APIモデル</returns>
        private ApiInformationModel CreateTransparentApi(TransparentApiType transparentApi, string vendorId, string url, string controllerId)
        {
            return new ApiInformationModel()
            {
                VendorId = vendorId,
                ControllerUrl = url,
                ApiId = Guid.NewGuid().ToString(),
                ApiDescription = transparentApi.Description,
                ControllerId = controllerId,
                MethodType = transparentApi.MethodType.ToString(),
                Url = transparentApi.MethodName,
                IsEnable = true,
                IsHeaderAuthentication = true,
                ActionType = new ActionType(ActionTypes.Query).Value.GetCode(),
                IsHidden = true,
                IsActive = true,
                ApiAccessVendorList = new(),
                SecondaryRepositoryMapList = new List<SecondaryRepositoryMapModel>(),
                ApiLinkList = new(),
                SampleCodeList = new(),
                IsTransparentApi = true
            };
        }

        /// <summary>
        /// 対象の論理キーが指定したモデルの論理キーに含まれているか
        /// </summary>
        /// <param name="schemaId">スキーマID</param>
        /// <param name="logicalKeys">論理キー</param>
        /// <param name="errorParamName">エラーパラメータ名</param>
        /// <returns>含まれている場合true、含まれていない場合はfalseを返す</returns>
        private void ContainsLogicalKey(string schemaId, IEnumerable<string> logicalKeys)
        {
            if (logicalKeys == null) return;

            // スキーマ情報取得
            var schema = DataSchemaRepository.GetDataSchema(schemaId);

            List<string> validResult = new();
            foreach (var key in logicalKeys.Where(x => string.IsNullOrEmpty(x) == false))
            {
                if (schema != null)
                {
                    if (schema.GetJSchema().Properties.Any(x => x.Key == key))
                    {
                        continue;
                    }
                }

                // 論理キーに含まれないキーを抽出
                validResult.Add(key);
            }

            if (validResult.Any())
            {
                var errorParamName = string.Join("、", validResult.Select(x => $"「{x.Replace("{", string.Empty).Replace("}", string.Empty)}」").ToArray());
                throw new NotFoundException(errorParamName);
            }
        }

        /// <summary>
        /// ロジカルキーを編集します。
        /// </summary>
        /// <returns>ロジカルキーリスト</returns>
        private IEnumerable<string> EditLogicalKeys(string Key)
        {
            if (string.IsNullOrEmpty(Key)) return null;

            List<string> logicalKeys = new();
            foreach (var key in Key.Split("/".ToCharArray()).ToList().Where(x => !string.IsNullOrEmpty(x)))
            {
                if (key.StartsWith("{") && key.EndsWith("}"))
                {
                    logicalKeys.Add(key[1..^1]);
                }
            }

            return logicalKeys.Distinct();
        }

        /// <summary>
        /// コントローラが重複しているか
        /// </summary>
        /// <param name="controller">コントローラ</param>
        /// <returns>true:存在する/false:存在しない</returns>
        public bool IsDuplicateController(ControllerInformationModel controller, out string controllerId)
        {

            // コントローラーのURLが重複しているか
            if (DynamicApiRepository.IsDuplicateController(controller.Url, out controllerId))
            {
                return true;
            }

            // コントローラーURLとAPIURLの組み合わせが重複しているか
            foreach (var api in controller.ApiList)
            {
                var methodType = new HttpMethodType(api.MethodType);
                var actionType = ActionType.Parse(api.ActionType);

                if (IsDuplicateApi(actionType, methodType, controller.Url, controllerId, api.Url, api.ApiId, out _))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// APIヘッダーを取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="controllerId">コントローラID</param>
        /// <returns></returns>
        public ControllerHeaderModel GetControllerHeader(string vendorId, string controllerId)
        {
            return new ControllerHeaderModel()
            {
                VendorNameSystemNameList = DynamicApiRepository.GetVendorList(),
                DataSchemaList = DataSchemaRepository.GetDataSchemaImformationList(vendorId),
                ControllerCategoryInfomationList = DynamicApiRepository.GetContollerCategoryListContainControllerIdNull(controllerId),
                ControllerCommonIpFilterGroupList = DynamicApiRepository.GetControllerCommonIpFilterGroupListContainControllerIdNull(controllerId),
                Controller = DynamicApiRepository.GetController(controllerId),
                TagInfoList = GetTagTree(),
                ControllerTagInfoList = DynamicApiRepository.GetControllerTagList(controllerId),
                FieldInfoList = new FieldTree(DynamicApiRepository.GetFields()).TreeList,
                ControllerFieldInfoList = DynamicApiRepository.GetControllerFieldList(controllerId),
                OpenIdCaList = DynamicApiRepository.GetControllerOpenIdCaList(controllerId),
                RepositoryGroupList = RepositoryGroupRepository.GetRepositoryGroupList(vendorId),
                AttachFileSettings = DynamicApiRepository.GetAttachFileSettings(controllerId),
                DocumentHistorySettings = DynamicApiRepository.GetDocumentHistorySettings(controllerId),
            };
        }

        /// <summary>
        /// タグ情報をTree構造で取得する
        /// </summary>
        /// <returns></returns>
        private List<TagInfoModel> GetTagTree()
        {
            var ret = new List<TagInfoModel>();

            // 有効なタグを全て取得
            var sourceList = DynamicApiRepository.GetTagInfoList();

            foreach (var source in sourceList)
            {
                // 親を探す、親が未登録の場合は親を先に登録して子要素として登録する
                var parent = FindTagData(sourceList, source, ret);

                // 親がいない かつ 未登録の場合、登録する
                if (!parent && !ret.Any(x => x.TagId == source.TagId))
                {
                    ret.Add(source);
                }
            }

            return ret;
        }

        public bool FindTagData(IList<TagInfoModel> sourceList, TagInfoModel tag, IList<TagInfoModel> result = null)
        {
            // 検索結果から対象を検索
            foreach (var source in sourceList)
            {
                // 対象と一致する
                if (source.TagId == tag.ParentTagId)
                {
                    if (!FindTagData(sourceList, source, result))
                    {
                        // 結果に登録済み？
                        if (!result.Where(x => x.TagId == source.TagId).Any())
                        {
                            // 未登録のものに親がいないまたは親を登録後、結果に登録
                            result.Add(source);
                        }
                    }

                    // 親の子要素に未登録の場合、登録
                    if (!source.Children.Any(x => x.TagId == tag.TagId))
                    {
                        source.Children.Add(tag);
                    }

                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
