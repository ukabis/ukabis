using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Core.DataContainer;
using JP.DataHub.ManageApi.Models.DynamicApi;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.MVC.Extensions;
using JP.DataHub.MVC.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;

namespace JP.DataHub.ManageApi.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("938098c8-c239-42fa-a2b0-f3da1bb0e665")]
    [UserRoleCheckController("DI_021")]
    public class DynamicApiController : AbstractController
    {
        private IDynamicApiService DynamicApiService => _lazyDynamicApiService.Value;
        private Lazy<IDynamicApiService> _lazyDynamicApiService = new(() => UnityCore.Resolve<IDynamicApiService>());

        #region Mapper
        /// <summary>
        /// AutoMapperの設定
        /// </summary>
        private static IMapper Mapper => s_lazyMapper.Value;
        private static Lazy<IMapper> s_lazyMapper = new(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CategoryQueryModel, DynamicApiCategoryViewModel>();
                cfg.CreateMap<FieldQueryModel, DynamicApiFieldViewModel>();
                cfg.CreateMap<TagQueryModel, DynamicApiTagViewModel>();
                cfg.CreateMap<RepositoryGroupModel, DynamicApiRepositoryGroupSimpleViewModel>();
                cfg.CreateMap<RepositoryGroupModel, DynamicApiAttachFileStorageViewModel>();
                cfg.CreateMap<ActionTypeModel, ActionTypeViewModel>();
                cfg.CreateMap<HttpMethodTypeModel, HttpMethodTypeViewModel>();
                cfg.CreateMap<SchemaModel, SchemaViewModel>();
                cfg.CreateMap<ControllerCommonIpFilterGroupModel, DynamicApiControllerCommonIpFilterGroupViewModel>();
                cfg.CreateMap<OpenIdCaModel, DynamicApiOpenIdCaViewModel>();
                cfg.CreateMap<LanguageModel, LanguageViewModel>();
                cfg.CreateMap<ScriptTypeModel, ScriptTypeViewModel>();
                cfg.CreateMap<QueryTypeModel, QueryTypeViewModel>();
                cfg.CreateMap<RegisterSchemaRequestViewModel, DataSchemaInformationModel>()
                    .ForMember(dst => dst.DataSchemaId, ops => ops.MapFrom(src => src.SchemaId))
                    .ForMember(dst => dst.DataSchema, ops => ops.MapFrom(src => src.JsonSchema))
                    .ForMember(dst => dst.DataSchemaDescription, ops => ops.MapFrom(src => src.Description));
                cfg.CreateMap<DataSchemaInformationModel, RegisterSchemaResponseViewModel>()
                    .ForMember(dst => dst.SchemaId, ops => ops.MapFrom(src => src.DataSchemaId));
                cfg.CreateMap<RegisterApiRequestViewModel, ControllerInformationModel>()
                    .ForMember(dst => dst.ControllerId, ops => ops.MapFrom(src => src.ApiId))
                    .ForMember(dst => dst.ControllerDescription, ops => ops.MapFrom(src => src.ApiDescriptiveText))
                    .ForMember(dst => dst.ControllerIpFilterList, ops => ops.MapFrom(src => src.ApiIpFilterList.Any() ? src.ApiIpFilterList : new List<RegisterApiIpFilterViewModel>()))
                    .ForMember(dst => dst.ControllerSchemaId, ops => ops.MapFrom(src => src.ModelId))
                    .ForMember(dst => dst.CategoryList, ops => ops.MapFrom(src => src.CategoryList.Any() ? src.CategoryList : new List<RegisterApiCategoryViewModel>()))
                    .ForMember(dst => dst.ControllerName, ops => ops.MapFrom(src => src.ApiName))
                    .ForMember(dst => dst.ControllerTagInfoList, ops => ops.MapFrom(src => src.ApiTagInfoList))
                    .ForMember(dst => dst.ControllerFieldInfoList, ops => ops.MapFrom(src => src.ApiFieldInfoList))
                    .ForMember(dst => dst.ControllerCommonIpFilterGroupList, ops => ops.MapFrom(src => src.ApiCommonIpFilterGroupList))
                    .ForMember(dst => dst.ResourceCreateDate, ops => ops.MapFrom(src => src.ResourceCreateDate == "" ? null : DateTime.Parse(src.ResourceCreateDate).ToString("yyyy/MM/dd")))
                    .ForMember(dst => dst.ResourceLatestDate, ops => ops.MapFrom(src => src.ResourceLatestDate == "" ? null : DateTime.Parse(src.ResourceLatestDate).ToString("yyyy/MM/dd")));
                cfg.CreateMap<DynamicApiAttachFileSettingsViewModel, AttachFileSettingsModel>();
                cfg.CreateMap<DocumentHistorySettingsViewModel, DocumentHistorySettingsModel>();
                cfg.CreateMap<RegisterApiIpFilterViewModel, ControllerIpFilterModel>();
                cfg.CreateMap<RegisterApiCategoryViewModel, ControllerCategoryInfomationModel>()
                    .ForMember(dst => dst.IsActive, ops => ops.MapFrom(src => src.CategoryId.Any()));
                cfg.CreateMap<RegisterApiCommonIpFilterGroupViewModel, ControllerCommonIpFilterGroupModel>();
                cfg.CreateMap<RegisterApiTagInfoViewModel, ControllerTagInfoModel>()
                    .ForMember(dst => dst.IsActive, ops => ops.MapFrom(src => src.TagId.Any()));
                cfg.CreateMap<RegisterApiFieldInfoViewModel, ControllerFieldInfoModel>()
                    .ForMember(dst => dst.IsActive, ops => ops.MapFrom(src => src.FieldId.Any()));
                cfg.CreateMap<RegisterMethodViewModel, ApiInformationModel>()
                    .ForMember(dst => dst.ControllerId, ops => ops.MapFrom(src => src.ApiId))
                    .ForMember(dst => dst.ApiId, ops => ops.MapFrom(src => src.MethodId))
                    .ForMember(dst => dst.ApiDescription, ops => ops.MapFrom(src => src.MethodDescriptiveText))
                    .ForMember(dst => dst.MethodType, ops => ops.MapFrom(src => src.HttpMethodTypeCd))
                    .ForMember(dst => dst.ActionType, ops => ops.MapFrom(src => src.ActionTypeCd))
                    .ForMember(dst => dst.UrlSchemaId, ops => ops.MapFrom(src => src.UrlModelId))
                    .ForMember(dst => dst.PostDataType, ops => ops.MapFrom(src => src.IsPostDataTypeArray ? "array" : string.Empty))
                    .ForMember(dst => dst.RequestSchemaId, ops => ops.MapFrom(src => src.RequestModelId))
                    .ForMember(dst => dst.ResponseSchemaId, ops => ops.MapFrom(src => src.ResponseModelId))
                    .ForMember(dst => dst.CacheMinute, ops => ops.MapFrom(src => string.IsNullOrEmpty(src.CacheMinute) ? null : (int?)int.Parse(src.CacheMinute)))
                    .ForMember(dst => dst.ApiLinkList, ops => ops.MapFrom(src => src.ApiLinkList))
                    .ForMember(dst => dst.ReturnMessage, ops => ops.Ignore());
                cfg.CreateMap<RegisterApiLinkViewModel, ApiLinkModel>()
                    .ForMember(dst => dst.IsActive, ops => ops.MapFrom(src => true))
                    .ForMember(dst => dst.IsVisible, ops => ops.MapFrom(src => true));
                cfg.CreateMap<ControllerQueryModel, ApiViewModel>()
                    .ForMember(dst => dst.ApiId, ops => ops.MapFrom(src => src.ControllerId))
                    .ForMember(dst => dst.ApiDescription, ops => ops.MapFrom(src => src.ControllerDescription))
                    .ForMember(dst => dst.ApiName, ops => ops.MapFrom(src => src.ControllerName))
                    .ForMember(dst => dst.ApiSchemaId, ops => ops.MapFrom(src => src.ControllerSchemaId))
                    .ForMember(dst => dst.ApiSchemaName, ops => ops.MapFrom(src => src.ControllerSchemaName))
                    .ForMember(dst => dst.ApiDescription, ops => ops.MapFrom(src => src.ControllerDescription))
                    .ForMember(dst => dst.TagList, ops => ops.MapFrom(src => src.ControllerTagList))
                    .ForMember(dst => dst.CategoryList, ops => ops.MapFrom(src => src.ControllerCategoryList))
                    .ForMember(dst => dst.FieldList, ops => ops.MapFrom(src => src.ControllerFieldList))
                    .ForMember(dst => dst.CommonIpFilterGroupList, ops => ops.MapFrom(src => src.ControllerCommonIpFilterGroupList))
                    .ForMember(dst => dst.IpFilterList, ops => ops.MapFrom(src => src.ControllerIpFilterList))
                    .ForMember(dst => dst.OpenIdCAList, ops => ops.MapFrom(src => src.ControllerOpenIdCAList))
                    .ForMember(dst => dst.MethodList, ops => ops.MapFrom(src => src.ApiList));
                cfg.CreateMap<ControllerLightQueryModel, ApiLightViewModel>()
                    .ForMember(dst => dst.ApiId, ops => ops.MapFrom(src => src.ControllerId))
                    .ForMember(dst => dst.ApiSchemaId, ops => ops.MapFrom(src => src.ControllerSchemaId));
                cfg.CreateMap<ControllerTagInfoModel, ApiTagViewModel>();
                cfg.CreateMap<ControllerCategoryInfomationModel, ApiCategoryViewModel>();
                cfg.CreateMap<ControllerFieldInfoModel, ApiFieldViewModel>();
                cfg.CreateMap<ControllerCommonIpFilterGroupModel, DynamicApiCommonIpFilterGroupViewModel>();
                cfg.CreateMap<ControllerIpFilterModel, ApiIpFilterViewModel>();
                cfg.CreateMap<OpenIdCaModel, ApiOpenIdCAViewModel>()
                   .ForMember(dst => dst.ApiOpenIdCaId, ops => ops.MapFrom(src => src.Id));
                cfg.CreateMap<ApiQueryModel, MethodViewModel>()
                    .ForMember(dst => dst.MethodId, ops => ops.MapFrom(src => src.ApiId))
                    .ForMember(dst => dst.ApiId, ops => ops.MapFrom(src => src.ControllerId))
                    .ForMember(dst => dst.MethodDescription, ops => ops.MapFrom(src => src.ApiDescription))
                    .ForMember(dst => dst.MethodUrl, ops => ops.MapFrom(src => src.ApiUrl))
                    .ForMember(dst => dst.MethodLinkList, ops => ops.MapFrom(src => src.ApiLinkList))
                    .ForMember(dst => dst.AccessVendorList, ops => ops.MapFrom(src => src.ApiAccessVendorList))
                    .ForMember(dst => dst.MethodOpenIdCAList, ops => ops.MapFrom(src => src.ApiOpenIdCAList));
                cfg.CreateMap<SecondaryRepositoryMapQueryModel, SecondaryRepositoryViewModel>();
                cfg.CreateMap<SampleCodeModel, DynamicApiSampleCodeViewModel>();
                cfg.CreateMap<ApiLinkModel, DynamicApiMethodLinkViewModel>()
                    .ForMember(dst => dst.MethodLinkId, ops => ops.MapFrom(src => src.ApiLinkId))
                    .ForMember(dst => dst.Title, ops => ops.MapFrom(src => src.LinkTitle))
                    .ForMember(dst => dst.Detail, ops => ops.MapFrom(src => src.LinkDetail))
                    .ForMember(dst => dst.Url, ops => ops.MapFrom(src => src.LinkUrl))
                    .ForMember(dst => dst.IsVisible, ops => ops.MapFrom(src => src.IsVisible))
                    ;
                cfg.CreateMap<ApiAccessVendorModel, AccessVendorViewModel>()
                    .ForMember(dst => dst.AccessVendorId, ops => ops.MapFrom(src => src.ApiAccessVendorId));
                cfg.CreateMap<OpenIdCaModel, MethodOpenIdCAViewModel>()
                    .ForMember(dst => dst.MethodOpenIdCaId, ops => ops.MapFrom(src => src.Id));
                cfg.CreateMap<ApiAccessOpenIdInfoModel, ApiAccessOpenIdInfoViewModel>().ReverseMap();
                cfg.CreateMap<OpenIdCaModel, RegisterResourceOpenIdCaViewModel>().ReverseMap();
                cfg.CreateMap<SampleCodeModel, RegisterSampleCodeViewModel>().ReverseMap();
                cfg.CreateMap<ApiAccessVendorModel, RegisterAccessVendorViewModel>().ReverseMap();
                cfg.CreateMap<VendorNameSystemNameModel, VendorNameSystemNameViewModel>();
                cfg.CreateMap<SystemNameSystemIdModel, SystemNameSystemIdViewModel>();
                cfg.CreateMap<DataSchemaInformationModel, DataSchemaInformationViewModel>();
                cfg.CreateMap<ControllerCategoryInfomationModel, ControllerCategoryInfomationViewModel>();
                cfg.CreateMap<ControllerCommonIpFilterGroupModel, ControllerCommonIpFilterGroupViewModel>();
                cfg.CreateMap<ControllerInformationModel, ControllerInformationViewModel>();
                cfg.CreateMap<TagInfoModel, TagInfoViewModel>();
                cfg.CreateMap<ControllerTagInfoModel, ControllerTagInfoViewModel>();
                cfg.CreateMap<FieldQueryModel, FieldQueryViewModel>();
                cfg.CreateMap<ControllerFieldInfoModel, ControllerFieldInfoViewModel>();
                cfg.CreateMap<OpenIdCaModel, ControllerOpenIdCaViewModel>();
                cfg.CreateMap<RepositoryGroupModel, DynamicApiRepositoryGroupFullViewModel>();
                cfg.CreateMap<AttachFileSettingsModel, AttachFileSettingsViewModel>();
                cfg.CreateMap<DocumentHistorySettingsModel, DocumentHistorySettingsViewModel>();
                cfg.CreateMap<ControllerIpFilterModel, ControllerIpFilterViewModel>();
                cfg.CreateMap<ControllerCategoryInfomationModel, ControllerCategoryInfomationViewModel>();
                cfg.CreateMap<ControllerCommonIpFilterGroupModel, ControllerCommonIpFilterGroupViewModel>();
                cfg.CreateMap<ControllerTagInfoModel, ControllerTagInfoViewModel>();
                cfg.CreateMap<ControllerFieldInfoModel, ControllerFieldInfoViewModel>();
                cfg.CreateMap<OpenIdCaModel, ControllerOpenIdCaViewModel>();
                cfg.CreateMap<ControllerMultiLanguageModel, ControllerMultiLanguageViewModel>();
                cfg.CreateMap<FieldQueryModel, FieldQueryViewModel>();
                cfg.CreateMap<ControllerHeaderModel, ApiHeaderViewModel>();
                cfg.CreateMap<ControllerSimpleQueryModel, ApiSimpleViewModel>()
                    .ForMember(dst => dst.ApiId, ops => ops.MapFrom(src => src.ControllerId))
                    .ForMember(dst => dst.MethodList, ops => ops.MapFrom(src => src.ApiList));
                cfg.CreateMap<ApiSimpleQueryModel, MethodSimpleViewModel>()
                    .ForMember(dst => dst.MethodId, ops => ops.MapFrom(src => src.ApiId))
                    .ForMember(dst => dst.ApiId, ops => ops.MapFrom(src => src.ControllerId))
                    .ForMember(dst => dst.MethodUrl, ops => ops.MapFrom(src => src.ApiUrl));
            });

            return config.CreateMapper();
        });

        #endregion

        #region マスター取得系

        /// <summary>
        /// DynamicAPIのカテゴリーの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのカテゴリーの一覧</returns>
        [HttpGet]
        [ManageAction("869a5bef-00b4-4c1e-a047-7975830ebf67")]
        public ActionResult<IEnumerable<DynamicApiCategoryViewModel>> GetCategories()
            => Ok(Mapper.Map<IEnumerable<DynamicApiCategoryViewModel>>(DynamicApiService.GetCategories()));

        /// <summary>
        /// DynamicAPIの分野の一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIの分野の一覧</returns>
        [HttpGet]
        [ManageAction("67da3f4f-b93a-4653-9f87-9b3071319504")]
        public ActionResult<IEnumerable<DynamicApiFieldViewModel>> GetFields()
            => Ok(Mapper.Map<IEnumerable<DynamicApiFieldViewModel>>(DynamicApiService.GetFields()));

        /// <summary>
        /// DynamicAPIのタグの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのタグの一覧</returns>
        [HttpGet]
        [ManageAction("b4fff2d3-9550-4ebb-801e-febe8ecc9645")]
        public ActionResult<IEnumerable<DynamicApiTagViewModel>> GetTags()
            => Ok(Mapper.Map<IEnumerable<DynamicApiTagViewModel>>(DynamicApiService.GetTags()));

        /// <summary>
        /// DynamicAPIのリポジトリの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのリポジトリの一覧</returns>
        [HttpGet]
        [ManageAction("42da383d-56bb-4591-baf7-b491e9a198de")]
        public ActionResult<IEnumerable<DynamicApiRepositoryGroupSimpleViewModel>> GetRepositoryGroups()
            => Ok(Mapper.Map<IEnumerable<DynamicApiRepositoryGroupSimpleViewModel>>(DynamicApiService.GetRepositoryGroups()));

        /// <summary>
        /// DynamicAPIの添付ファイルBlobストレージの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIの添付ファイルBlobストレージの一覧</returns>
        [HttpGet]
        [ManageAction("0A2DE3BB-2E5E-4EA9-A5D5-FDD50B5E30C5")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<DynamicApiAttachFileStorageViewModel>> GetAttachFileBlobStorage()
            => Ok(Mapper.Map<IEnumerable<DynamicApiAttachFileStorageViewModel>>(DynamicApiService.GetAttachFileStorage(new[] { "afb" })));

        /// <summary>
        /// DynamicAPIの添付ファイルメタストレージの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIの添付ファイルメタストレージの一覧</returns>
        [HttpGet]
        [ManageAction("128D63B9-09D8-4213-94E2-F026D3A7A03C")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<DynamicApiAttachFileStorageViewModel>> GetAttachFileMetaStorage()
            => Ok(Mapper.Map<IEnumerable<DynamicApiAttachFileStorageViewModel>>(DynamicApiService.GetAttachFileStorage(new[] { "afm", "afs" })));

        /// <summary>
        /// DynamicAPIのアクションタイプの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのアクションタイプの一覧</returns>
        [HttpGet]
        [ManageAction("c5f02230-7049-486f-b5c0-fd5c814bcc4c")]
        public ActionResult<IEnumerable<ActionTypeViewModel>> GetActionTypes()
            => Ok(Mapper.Map<IEnumerable<ActionTypeViewModel>>(DynamicApiService.GetActionTypes()));

        /// <summary>
        /// DynamicAPIのHTTPメソッドの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのHTTPメソッドの一覧</returns>
        [HttpGet]
        [ManageAction("884b2dd8-3582-4227-8d07-f06054c2bc51")]
        public ActionResult<IEnumerable<HttpMethodTypeViewModel>> GetHttpMethods()
             => Ok(Mapper.Map<IEnumerable<HttpMethodTypeViewModel>>(DynamicApiService.GetHttpMethodTypes()));

        /// <summary>
        /// DynamicAPIのContollerCommonIpFilterGroupの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのContollerCommonIpFilterGroupの一覧</returns>
        [HttpGet]
        [ManageAction("233FF79F-38BA-4E72-BB05-FCC582027EDF")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<DynamicApiControllerCommonIpFilterGroupViewModel>> GetControllerCommonIpFilterGroupList()
            => Ok(Mapper.Map<IEnumerable<DynamicApiControllerCommonIpFilterGroupViewModel>>(DynamicApiService.GetControllerCommonIpFilterGroup(null)));

        /// <summary>
        /// DynamicAPIのOpenId認証局の一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのOpenId認証局の一覧</returns>
        [HttpGet]
        [ManageAction("9B906D94-4C91-4169-99E8-DD7CE463A519")]
        public ActionResult<IEnumerable<DynamicApiOpenIdCaViewModel>> GetOpenIdCaList()
            => Ok(Mapper.Map<IEnumerable<DynamicApiOpenIdCaViewModel>>(DynamicApiService.GetControllerOpenIdCaList(null)));

        /// <summary>
        /// DynamicAPIの言語の一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIの言語の一覧</returns>
        [HttpGet]
        [ManageAction("949D6A45-87F2-4E6C-9EEC-D5024865C656")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<LanguageViewModel>> GetLanguageList()
            => Ok(Mapper.Map<IEnumerable<LanguageViewModel>>(DynamicApiService.GetLanguageList()));

        /// <summary>
        /// DynamicAPIのスクリプトタイプの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのスクリプトタイプの一覧</returns>
        [HttpGet]
        [ManageAction("BDA47F28-FE1D-4C2B-8FC9-189D24CE956B")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<ScriptTypeViewModel>> GetScriptTypeList()
            => Ok(Mapper.Map<IEnumerable<ScriptTypeViewModel>>(DynamicApiService.GetScriptTypeList()));

        /// <summary>
        /// DynamicAPIのクエリタイプの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのクエリタイプの一覧</returns>
        [HttpGet]
        [ManageAction("10F92AAF-7DDF-44A1-870A-29175F7DECAB")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<QueryTypeViewModel>> GetQueryTypeList()
            => Ok(Mapper.Map<IEnumerable<QueryTypeViewModel>>(DynamicApiService.GetQueryTypeList()));

        #endregion

        #region API
        /// <summary>
        /// DynamicAPIのリソースのリストを取得します。
        /// </summary>
        /// <param name="isAll">全てのベンダーのリソースを取得するか</param>
        /// <returns>リソースのリスト</returns>
        [HttpGet]
        [ValidateModel]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("fa9700d3-ca09-43a6-b013-0d1b22df1c1a")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult<IEnumerable<ApiViewModel>> GetApiResourceMethodList(bool isAll = false)
            => Ok(Mapper.Map<IEnumerable<ApiViewModel>>(DynamicApiService.GetControllerApiList(isAll)));

        /// <summary>
        /// DynamicAPIのリソースのリストを取得します。（内容はシンプルなもの、管理画面のツリーを表示するための項目のみ）
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>
        /// ベンダーに紐づくリソースのリスト。
        /// <paramref name="vendorId"/>がnullの場合、運用管理ベンダーであれば全ベンダーのリソースのリスト、それ以外のベンダーは自ベンダーのリソースのリスト。
        /// </returns>
        [HttpGet]
        [ValidateModel]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("fa9700d3-ca09-43a6-b013-0d1b22df1c1a")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult<IEnumerable<ApiSimpleViewModel>> GetApiResourceMethodSimpleList(string vendorId = null, bool isTransparent = true)
            => Ok(Mapper.Map<IEnumerable<ApiSimpleViewModel>>(DynamicApiService.GetControllerApiSimpleList(vendorId, isTransparent)));

        /// <summary>
        /// ApiIdに紐づくDynamicAPIのリソースを取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        [HttpGet]
        [ValidateModel]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("ec03a439-4e86-47ab-a1ce-062942460e52")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ApiViewModel> GetApiResourceFromApiId([RequiredGuid] string apiId, bool isTransparent = false)
            => Ok(Mapper.Map<ApiViewModel>(DynamicApiService.GetControllerResourceFromControllerId(apiId, isTransparent)));

        /// <summary>
        /// ApiIdに紐づくDynamicAPIのリソースを取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>リソース</returns>
        [HttpGet]
        [ValidateModel]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("59162858-495c-44d2-909c-be378ac2006b")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ApiLightViewModel> GetApiResourceLight([RequiredGuid] string apiId)
            => Ok(Mapper.Map<ApiLightViewModel>(DynamicApiService.GetControllerResourceLight(apiId)));

        /// <summary>
        /// ApiIdに紐づくDynamicAPIのリソースを取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        [HttpGet]
        [ValidateModel]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("ec03a439-4e86-47ab-a1ce-062942460e52")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ApiHeaderViewModel> GetApiResourceHeader([RequiredGuid] string vendorId, [RequiredGuid] string apiId)
        {
            var ret = DynamicApiService.GetControllerHeader(vendorId, apiId);
            return Ok(Mapper.Map<ApiHeaderViewModel>(ret));
        }

        /// <summary>
        /// Urlに紐づくDynamicAPIのリソースを取得します。
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        [HttpGet]
        [ValidateModel]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("65b28d59-3ced-4636-980c-47f1701cb684")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public ActionResult<ApiViewModel> GetApiResourceFromUrl([Required] string url, bool isTransparent = false)
            => Ok(Mapper.Map<ApiViewModel>(DynamicApiService.GetControllerResourceFromUrl(url, isTransparent)));

        /// <summary>
        /// DynamicAPIのAPIを登録または更新します。
        /// </summary>
        /// <param name="model">APIモデル</param>
        /// <returns>APIの登録・更新結果</returns>
        [HttpPost]
        [ValidateModel]
        [UserRole("DI_021", UserRoleAccessType.Write)]
        [ManageAction("936f09e7-0224-435f-a7f0-3146f96d5696")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, new Type[] { typeof(AlreadyExistsException), typeof(InvalidDataException), typeof(ArgumentException), typeof(QuerySyntaxErrorException), typeof(ForeignKeyException), typeof(ConflictException) }, HttpStatusCode.BadRequest)]
        public ActionResult RegisterApi(RegisterApiRequestViewModel model)
        {
            var result = DynamicApiService.RegistOrUpdateController(Mapper.Map<ControllerInformationModel>(model));
            return Created(string.Empty, new { ApiId = result.ControllerId });
        }

        /// <summary>
        ///  DynamicAPIのAPIを削除します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <returns>NoContent</returns>
        [HttpDelete]
        [ValidateModel]
        [ManageAction("5CC32699-97F3-4420-A126-04C8BDAE245D")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteApi([RequiredGuid] string apiId)
        {
            DynamicApiService.DeleteController(apiId);
            return NoContent();
        }

        /// <summary>
        /// DynamicAPIのAPIをUrlを元に削除します。
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>NoContent</returns>
        [HttpDelete]
        [UserRole("DI_021", UserRoleAccessType.Write)]
        [ManageAction("F7495D33-A9E8-432D-89EA-4B951DD4520E")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteApiFromUrl([Required] string url)
        {
            DynamicApiService.DeleteControllerFromUrl(url);
            return NoContent();
        }

        /// <summary>
        /// コントローラが重複しているか
        /// </summary>
        /// <param name="model">APIモデル</param>
        /// <returns>重複しているか</returns>
        [HttpPost]
        [ValidateModel]
        [UserRole("DI_021", UserRoleAccessType.Write)]
        [ManageAction("FD735E18-F516-4A53-969E-C1A4514FB72E")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest)]
        public ActionResult<bool> IsDuplicateController(RegisterApiRequestViewModel model)
            => Ok(DynamicApiService.IsDuplicateController(Mapper.Map<ControllerInformationModel>(model), out _));
        #endregion

        #region メソッド
        /// <summary>
        /// メソッドIDに紐づくDynamicAPIのメソッドを取得します。
        /// </summary>
        /// <param name="methodId">methodId</param>
        /// <returns>リソース</returns>
        [HttpGet]
        [ValidateModel]
        [UserRole("DI_022", UserRoleAccessType.Read)]
        [ManageAction("EF1ACF11-2A48-488C-BF8C-BE8ACE351E48")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<MethodViewModel> GetApiMethod([RequiredGuid] string methodId)
            => Ok(Mapper.Map<MethodViewModel>(DynamicApiService.GetApi(methodId)));

        /// <summary>
        /// Urlに紐づくDynamicAPIのメソッドを取得します。
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>リソース</returns>
        [HttpGet]
        [UserRole("DI_022", UserRoleAccessType.Read)]
        [ManageAction("fa0b17c6-60d3-43ba-afd6-42741e85f400")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public ActionResult<MethodViewModel> GetApiMethodFromUrl([Required] string url)
            => Ok(Mapper.Map<MethodViewModel>(DynamicApiService.GetApiFromUrl(url)));

        /// <summary>
        /// DynamicAPIのメソッドを登録または更新します。
        /// </summary>
        /// <param name="model">メソッドモデル</param>
        /// <returns>メソッドの登録・更新結果</returns>
        [HttpPost]
        [ValidateModel]
        [UserRole("DI_022", UserRoleAccessType.Write)]
        [ManageAction("63f1a156-adf3-449a-8876-3d1237af21c6")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, new Type[] { typeof(AlreadyExistsException), typeof(InvalidDataException), typeof(ArgumentException), typeof(QuerySyntaxErrorException), typeof(ConflictException) }, HttpStatusCode.BadRequest)]
        public ActionResult RegisterMethod(RegisterMethodViewModel model)
        {
            var result = DynamicApiService.RegistOrUpdateApi(Mapper.Map<ApiInformationModel>(model));
            return Created(string.Empty, new { MethodId = result.ApiId });
        }

        /// <summary>
        ///  DynamicAPIのメソッドを削除します。
        /// </summary>
        /// <param name="methodId">methodId</param>
        /// <returns>NoContent</returns>
        [HttpDelete]
        [UserRole("DI_022", UserRoleAccessType.Write)]
        [ManageAction("7B8434AD-5377-4F2D-8170-53FE80FC987B")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentException), HttpStatusCode.BadRequest, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteMethod([RequiredGuid] string methodId)
        {
            DynamicApiService.DeleteApi(methodId);
            return NoContent();
        }

        /// <summary>
        /// DynamicAPIのメソッドをUrlを元に削除します。
        /// </summary>
        /// <param name="url">Url</param>
        [HttpDelete]
        [UserRole("DI_022", UserRoleAccessType.Write)]
        [ManageAction("298418FF-40AE-46C4-B1F7-889D2147EB96")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteMethodFromUrl([Required] string url)
        {
            DynamicApiService.DeleteApiFromUrl(url);
            return NoContent();
        }

        /// <summary>
        /// 指定したメソッド情報が他のメソッドと重複しているかを判定する。
        /// メソッドIDを指定した場合は、当該メソッドとの重複は除外する。
        /// </summary>
        /// <param name="model">メソッド情報</param>
        /// <returns>重複している場合はtrue、それ以外はfalse。</returns>
        [HttpPost]
        [UserRole("DI_022", UserRoleAccessType.Read)]
        [ManageAction("926CC98E-425C-4365-BE6F-C55903167482")]
        public ActionResult<bool> IsDuplicateMethod(IsDuplicateMethodViewModel model)
            => Ok(DynamicApiService.IsDuplicateApi(model.ActionType, model.HttpMethodType, model.ApiId, model.MethodUrl, model.MethodId));

        /// <summary>
        /// アクションタイプ、メソッドタイプ、リポジトリグループの組み合わせでAPIが実行可能かを判定する。
        /// </summary>
        /// <param name="actionType">アクションタイプ</param>
        /// <param name="methodType">メソッドタイプ</param>
        /// <param name="repositoryGroupId">リポジトリグループID</param>
        /// <returns>APIが実行可能な場合はtrue、それ以外はfalse。</returns>
        [HttpGet]
        [UserRole("DI_022", UserRoleAccessType.Read)]
        [ManageAction("E9AD833E-1DA5-4649-B9A9-3C13E654527F")]
        public ActionResult<bool> IsExecutableApiMethod(string actionType, string methodType, string repositoryGroupId)
            => Ok(DynamicApiService.IsExcuseApiCombinationConstraints(actionType, methodType, repositoryGroupId));

        /// <summary>
        /// スクリプト構文チェックメッセージを取得します。
        /// </summary>
        /// <param name="model">モデル</param>
        /// <returns>メッセージ</returns>
        [HttpPost]
        [UserRole("DI_022", UserRoleAccessType.Write)]
        [ManageAction("75F32CAC-EBD9-4E68-B5BE-7B14D42D0047")]
        public ActionResult GetScriptSyntaxCheckMessage(ScriptViewModel model)
            => Ok(new { Message = DynamicApiService.GetScriptSyntaxCheckMessage(model.Script, model.ScriptType) });
        #endregion

        #region スキーマ
        /// <summary>
        /// 指定されたベンダーのDynamicAPIのスキーマの一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID。</param>
        /// <returns>
        /// DynamicAPIのスキーマの一覧。
        /// <paramref name="vendorId"/>がnullの場合、運用管理ベンダーであれば全ベンダーのスキーマ一覧、それ以外のベンダーであれば自ベンダーのスキーマ一覧。
        /// </returns>
        [HttpGet]
        [UserRole("DI_023", UserRoleAccessType.Read)]
        [ManageAction("3b3f4f63-451c-4829-a51d-3490670e4c4c")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult<IEnumerable<SchemaViewModel>> GetSchemas(string vendorId = null)
            => Ok(Mapper.Map<IEnumerable<SchemaViewModel>>(DynamicApiService.GetSchemas(vendorId)));

        /// <summary>
        /// スキーマIDからDynamicAPIのスキーマ一件を取得します。
        /// </summary>
        /// <returns>DynamicAPIのスキーマ情報</returns>
        [HttpGet]
        [ValidateModel]
        [UserRole("DI_023", UserRoleAccessType.Read)]
        [ManageAction("BB138CC3-4704-4BA0-88C9-D3C9AA93022F")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<SchemaViewModel> GetSchemaById([RequiredGuid] string schemaId)
            => Ok(Mapper.Map<SchemaViewModel>(DynamicApiService.GetSchemaById(schemaId)));

        /// <summary>
        ///  DynamicAPIのURL、URIまたはレスポンスに指定するスキーマを登録します。
        /// </summary>
        /// <param name="model">スキーマ登録情報</param>
        /// <returns>スキーマ登録結果</returns>
        [HttpPost]
        [ValidateModel]
        [UserRole("DI_023", UserRoleAccessType.Write)]
        [ManageAction("099638B0-9086-4379-B97F-B1102D175B7B")]
        [ExceptionFilter(typeof(AccessDeniedException), HttpStatusCode.Forbidden, new Type[] { typeof(InUseException), typeof(AlreadyExistsException), typeof(InvalidNotifyDataException), typeof(JsonException), typeof(ForeignKeyException), typeof(ConflictException) }, HttpStatusCode.BadRequest)]
        public ActionResult<RegisterSchemaResponseViewModel> RegisterUriOrResponseSchema(RegisterSchemaRequestViewModel model)
            => Created(string.Empty, Mapper.Map<RegisterSchemaResponseViewModel>(DynamicApiService.RegistOrUpdateSchema(Mapper.Map<DataSchemaInformationModel>(model), true)));

        /// <summary>
        ///  DynamicAPIのURL、リクエストに指定するスキーマを登録します。
        /// </summary>
        /// <param name="model">スキーマ登録情報</param>
        /// <returns>スキーマ登録結果</returns>
        [HttpPost]
        [ValidateModel]
        [UserRole("DI_023", UserRoleAccessType.Write)]
        [ManageAction("8a462e9c-037f-4f5f-a070-e1b0971a9a19")]
        [ExceptionFilter(typeof(AccessDeniedException), HttpStatusCode.Forbidden, new Type[] { typeof(InUseException), typeof(AlreadyExistsException), typeof(InvalidNotifyDataException), typeof(JsonException), typeof(ForeignKeyException), typeof(ConflictException) }, HttpStatusCode.BadRequest)]
        public ActionResult<RegisterSchemaResponseViewModel> RegisterSchema(RegisterSchemaRequestViewModel model)
            => Created(string.Empty, Mapper.Map<RegisterSchemaResponseViewModel>(DynamicApiService.RegistOrUpdateSchema(Mapper.Map<DataSchemaInformationModel>(model), false)));

        /// <summary>
        /// DynamicAPIのスキーマを削除します。
        /// </summary>
        /// <param name="schemaId">schemaId</param>
        /// <returns></returns>
        [HttpDelete]
        [ValidateModel]
        [UserRole("DI_023", UserRoleAccessType.Write)]
        [ManageAction("7EE70EE5-FDE0-4C0C-B03D-D2E59ED154F8")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(InUseException), HttpStatusCode.BadRequest, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteSchema([RequiredGuid] string schemaId)
        {
            DynamicApiService.DeleteDataSchema(schemaId);
            return NoContent();
        }

        /// <summary>
        /// DataSchema名重複チェック
        /// </summary>
        /// <param name="schemaName">スキーマ名</param>
        /// <returns>true:登録済/false:未登録</returns>
        [HttpGet]
        [UserRole("DI_023", UserRoleAccessType.Write)]
        [ManageAction("75F651BA-90AF-436F-A60C-7ACD476D238D")]
        public bool ExistsSameSchemaName(string schemaName)
            => DynamicApiService.ExistsSameSchemaName(schemaName);
        #endregion

        #region OpenIdACL
        /// <summary>
        /// DynamicAPIのメソッドにアクセスできるOpenIdを追加します。
        /// </summary>
        /// <param name="methodId">メソッドID</param>
        /// <param name="openId">OpenId</param>
        [HttpPost]
        [ValidateModel]
        [UserRole("DI_022", UserRoleAccessType.Write)]
        [ManageAction("092AA9D7-4542-4708-AB28-277559083940")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentException), HttpStatusCode.BadRequest)]
        public ActionResult AddOpenIdAuthorizeMethod([RequiredGuid] string methodId, string openId)
            => Ok(Mapper.Map<ApiAccessOpenIdInfoViewModel>(DynamicApiService.RegisterApiAccessOpenId(methodId, openId)));

        /// <summary>
        /// DynamicAPIのメソッドにアクセスできるOpenIdを削除します。
        /// </summary>
        /// <param name="methodId">メソッドID</param>
        /// <param name="openId">OpenId</param>
        [HttpDelete]
        [ValidateModel]
        [UserRole("DI_022", UserRoleAccessType.Write)]
        [ManageAction("56067509-5A61-411F-9455-36C812AD7AD8")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteOpenIdAuthorizeMethod([RequiredGuid] string methodId, string openId)
        {
            DynamicApiService.DeleteApiAccessOpenId(methodId, openId);
            return NoContent();
        }

        /// <summary>
        /// ApiのOpenIdアクセスコントロールの有効/無効を設定します。
        /// </summary>
        /// <param name="methodId">メソッドID</param>
        /// <param name="use">有効な場合true、無効な場合falseを指定する</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [UserRole("DI_022", UserRoleAccessType.Write)]
        [ManageAction("0984E57E-CECF-4D44-AFD0-99069A319D49")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentException), HttpStatusCode.BadRequest)]
        public ActionResult UseOpenIdAuthorizeMethod([RequiredGuid] string methodId, bool use)
        {
            DynamicApiService.UseApiAccessOpenId(methodId, use);
            return Ok();
        }
        #endregion
    }
}
