using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ActionInjector;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Scripting;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class DynamicApiActionFactory
    {
        private static Lazy<IMapper> _Mapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                //                    cfg.CreateMap<IMethod, IDynamicApiAction>().ForMember(d => d.KeyValue, o => o.MapFrom(s => new UrlParameter(s.KeyValue.ToDictionary(k => k.Key, v => v.Value))))
                //                    .ForMember(d=>d.Query,o=>o.MapFrom(s=>new QueryString(s.Query.ToDictionary(k=>k.Key,v=>v.Value))));
                cfg.CreateMap<IMethod, IDynamicApiAction>()
                    .ForMember(x => x.KeyValue, opt => opt.Ignore())
                    .ForMember(x => x.Query, opt => opt.Ignore())
                    .AfterMap((s, d) =>
                    {
                        if (s.Query != null)
                        {
                            d.Query = new QueryStringVO(s.Query.Dic.ToDictionary(key => key.Key, value => value.Value), s.Query.OriginalQueryString);
                        }
                        if (s.KeyValue != null)
                        {
                            d.KeyValue = new UrlParameter(s.KeyValue.Dic.ToDictionary(key => key.Key, value => value.Value));
                        }
                    });
                cfg.CreateMap<IMethod, IResourceVersionRepository>();
            });
            return mappingConfig.CreateMapper();
        });


        private static IMapper Mapper
        {
            get
            {
                return _Mapper.Value;

            }
        }

        public static IDynamicApiAction CreateDynamicApiAction(IMethod method, MediaType mediaType,
            Contents contents, Accept accept, IPerRequestDataContainer perRequestDataContainer, ActionId actionId, DataSchema requestSchema, ContentRange contentRange = null)
        {
            IDynamicApiAction ret;
            switch (method.ActionType.Value)
            {
                case ActionType.Gateway:
                    var gatewayAction = UnityCore.Resolve<IGatewayAction>();
                    gatewayAction.GatewayInfo = method.GatewayInfo;
                    ret = gatewayAction;
                    break;
                case ActionType.AdaptResourceSchema:
                    var adaptAction = UnityCore.Resolve<IAdaptResourceSchemaAction>();
                    adaptAction.RepositoryInfoList = method.RepositoryInfo;
                    ret = adaptAction;
                    break;
                default:
                    ret = UnityCore.Resolve<IDynamicApiAction>(method.ActionType.Value.GetRegistorName(method.ActionTypeVersion.Value));
                    // ContentsをConvert
                    var convertResult = contents.ConvertContents(mediaType, requestSchema, method.PostDataType.ToIsArray);
                    contents = convertResult.Item2;
                    break;
            }

            // Methodのプロパティをコピー
            Mapper.Map(method, ret);
            // ContentsをActionに設定
            ret.Contents = contents;
            // PerRequestDataContainerの値をActionに設定
            SetPerRequestDataContainerValues(ret, perRequestDataContainer);
            var dynamicApiDataStoreRepositoryFactory = UnityCore.Resolve<IDynamicApiDataStoreRepositoryFactory>();
            ret.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(method.RepositoryInfo
                .Where(repositoryInfo => repositoryInfo.Type != RepositoryType.Unknown && repositoryInfo.Type != RepositoryType.DocumentHistoryStorage && repositoryInfo.Type != RepositoryType.AttachFileBlob)
                .Select(repositoryInfo =>
                {
                    var r = dynamicApiDataStoreRepositoryFactory.NewDataStoreRestore(repositoryInfo.Type, repositoryInfo, method.IsEnableResourceVersion);
                    Mapper.Map(method, r.ResourceVersionRepository);
                    return r;
                })
                .ToList());
            var dhs = method.RepositoryInfo.Where(x => x.Type == RepositoryType.DocumentHistoryStorage).FirstOrDefault();
            ret.HistoryEvacuationDataStoreRepository = dynamicApiDataStoreRepositoryFactory.NewDataStoreRestore(RepositoryType.DocumentHistoryStorage, dhs, method.IsEnableResourceVersion);
            ret.MediaType = mediaType;
            if (accept != null) ret.Accept = accept;
            if (contentRange != null) ret.ContentRange = contentRange;
            ret.ProviderVendorId = method.VendorId;
            ret.ProviderSystemId = method.SystemId;
            if (ret.AttachFileBlobRepositoryInfo != null)
            {
                var attachfileRepository = UnityCore.Resolve<IDynamicApiDataStoreRepositoryFactory>().NewDataStoreRestore(ret.AttachFileBlobRepositoryInfo.Type, ret.AttachFileBlobRepositoryInfo, method.IsEnableResourceVersion);
                ret.AttachFileDynamicApiDataStoreRepository = (IDynamicApiAttachFileRepository)attachfileRepository;
            }

            // 添付ファイル用履歴機能が無効な場合はここで履歴機能を無効にする
            if (!UnityCore.Resolve<bool>("UseApiAttachFileDocumentHistory"))
            {
                if (ret.ActionType.IsAttachFileAction || (ret.ActionInjectorHandler != null && ret.ActionInjectorHandler.Value.BaseType == typeof(AbstractAttachFileActionInjector)))
                {
                    ret.IsDocumentHistory = new IsDocumentHistory(false);
                }
            }

            // Scriptが設定されていない場合は生成したアクションを返却
            if (string.IsNullOrEmpty(method.Script.Value)) return ret;

            // Scriptが設定されている場合はActionTypeをスクリプトにして再度呼び出す、本来すべきアクションはUnityContainerに登録する。
            var dataContainer = UnityCore.Resolve<IDynamicApiDataContainer>();
            dataContainer.baseApiAction = ret;
            // ScriptActionを生成
            var scriptAction = UnityCore.Resolve<IScriptAction>();
            // Methodのプロパティをコピー
            Mapper.Map(method, scriptAction);
            // ContentsをActionに設定
            scriptAction.Contents = contents;

            scriptAction.ProviderVendorId = new VendorId(method.VendorId.Value);
            // PerRequestDataContainerの値をActionに設定
            SetPerRequestDataContainerValues(scriptAction, perRequestDataContainer);
            scriptAction.MediaType = mediaType;
            scriptAction.DynamicApiDataStoreRepository = dataContainer.baseApiAction.DynamicApiDataStoreRepository;
            // アクションにスクリプトを設定
            scriptAction.Script = method.Script;
            scriptAction.ScriptType = method.ScriptType;
            return scriptAction;
        }

        /// <summary>
        /// PerRequestDataContainerの値をActionに設定します。
        /// </summary>
        /// <param name="action">IDynamicApiAction</param>
        /// <param name="container">IPerRequestDataContainer</param>
        private static void SetPerRequestDataContainerValues(IDynamicApiAction action, IPerRequestDataContainer container)
        {
            action.VendorId = container.VendorId.ToVendorId();
            action.SystemId = container.SystemId.ToSystemId();
            action.OpenId = container.OpenId.ToOpenId();
            action.Xadmin = container.Xadmin.ToXAdmin();
            action.Xversion = container.Xversion.ToXVersion();
            action.XGetInnerAllField = container.XgetInternalAllField.ToXGetInnerField();
            action.XRequestContinuation = container.XRequestContinuation.ToXRequestContinuation();
            action.XNoOptimistic = container.XNoOptimistic.ToXNoOptimistic();
            action.XResourceSharingPerson = container.XResourceSharingPerson.ToXResourceSharingPerson();
            action.XUserResourceSharing = container.XUserResourceSharing.ToXUserResourceSharing();

        }
    }
}
