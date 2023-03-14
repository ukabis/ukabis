using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Web.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Json.Schema.Generation;
using Unity;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.MVC.Extensions;
using JP.DataHub.MVC.Filters;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Models.Test;
using JP.DataHub.ManageApi.Models.ApiDescription;
using JP.DataHub.ManageApi.Core.DataContainer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using JP.DataHub.ManageApi.Attributes;

namespace JP.DataHub.ManageApi.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("4987FA7D-F709-4A48-A04E-3BC0230ED887")]
    [UserRoleCheckController("DI_021")]
    public class ApiController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VendorLinkModel, ApiVendorLinkViewModel>();
                cfg.CreateMap<SystemLinkModel, ApiDescriptionSystemLinkViewModel>();
                cfg.CreateMap<ApiDescriptionModel, ApiDescriptionViewModel>();
                cfg.CreateMap<CategoryModel, CategoryViewModel>();
                cfg.CreateMap<FieldModel, FieldViewModel>();
                cfg.CreateMap<TagModel, TagViewModel>();
                cfg.CreateMap<MethodDescriptionModel, MethodDescriptionViewModel>();
                cfg.CreateMap<MethodLinkModel, MethodLinkViewModel>();
                cfg.CreateMap<SampleCode, SampleCodeViewModel>();
                cfg.CreateMap<SchemaDescriptionModel, SchemaDescriptionViewModel>();
                cfg.CreateMap<RegisterStaticApiRequestViewModel, RegisterStaticApiRequestModel>();
                cfg.CreateMap<RegisterStaticApiResponseModel, RegisterStaticApiResponseViewModel>();
            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private IPerRequestDataContainer _perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();

        private Lazy<IApiDescriptionService> _lazyApiDescriptionService = new Lazy<IApiDescriptionService>(() => UnityCore.Resolve<IApiDescriptionService>());
        private IApiDescriptionService _apiDescriptionService { get => _lazyApiDescriptionService.Value; }

        private Lazy<IDynamicApiService> _lazyDynamicApiService = new(() => UnityCore.Resolve<IDynamicApiService>());
        private IDynamicApiService DynamicApiService => _lazyDynamicApiService.Value;

        private readonly IApiDescriptionGroupCollectionProvider ApiExplorer;

        private readonly IUnityContainer UnityContainer;

        public ApiController(IApiDescriptionGroupCollectionProvider apiExplorer, IUnityContainer container)
        {
            ApiExplorer = apiExplorer;
            UnityContainer = container;
        }

        [Admin]
        [HttpGet]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("fe685fb5-9b55-45cc-89d7-9399fc7c266f")]
        public ActionResult<List<ApiVendorLinkViewModel>> GetVendorLink()
        {
            var links = _apiDescriptionService.GetVendorLink();
            return s_mapper.Map<List<ApiVendorLinkViewModel>>(links);
        }

        [Admin]
        [HttpGet]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("4855272d-96db-459b-be62-3fbe14eb4e7a")]
        public ActionResult<IList<ApiDescriptionSystemLinkViewModel>> GetSystemLink()
        {
            var links = _apiDescriptionService.GetSystemLink();
            return s_mapper.Map<List<ApiDescriptionSystemLinkViewModel>>(links);
        }

        [Admin]
        [HttpGet]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("c2a02d2b-d3bd-46a4-b3d2-0fbcbf73db75")]
        public ActionResult<IList<ApiDescriptionViewModel>> GetApiDescription(bool noChildren = false)
        {
            var culture = _perRequestDataContainer.CultureInfo.Name;
            var apis = _apiDescriptionService.GetApiDescription(noChildren, culture);
            return s_mapper.Map<List<ApiDescriptionViewModel>>(apis);
        }

        [Admin]
        [HttpGet]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("70553086-2888-4571-8089-f0bb440c11ed")]
        public ActionResult<IList<SchemaDescriptionViewModel>> GetSchemaDescription()
        {
            var culture = _perRequestDataContainer.CultureInfo.Name;
            var schemas = _apiDescriptionService.GetSchemaDescription(culture);
            return s_mapper.Map<List<SchemaDescriptionViewModel>>(schemas);
        }

        [Admin]
        [HttpGet]
        [UserRole("DI_021", UserRoleAccessType.Read)]
        [ManageAction("9E3481FE-8EDD-45CC-ABBF-421245D80515")]
        public ActionResult<IList<CategoryViewModel>> GetCategory()
        {
            var culture = _perRequestDataContainer.CultureInfo.Name;
            var categories = _apiDescriptionService.GetCategoryList(culture);
            return s_mapper.Map<List<CategoryViewModel>>(categories);
        }

        [Admin]
        [HttpPost]
        [UserRole("DI_021", UserRoleAccessType.Write)]
        [ManageAction("4f2d4141-7f45-4ede-ad2b-f68625ef4c8a")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest)]
        public ActionResult<RegisterStaticApiResponseViewModel> RegisterStaticApi(RegisterStaticApiRequestViewModel requestModel)
        {
            var model = CreateStaticApiModel(requestModel.ApiId.ToString());
            var result = _apiDescriptionService.RegisterStaticApi(model);
            return Created(string.Empty, s_mapper.Map<RegisterStaticApiResponseViewModel>(result));
        }

        [Admin]
        [HttpPost]
        [UserRole("DI_021", UserRoleAccessType.Write)]
        [ManageAction("F36B2ADA-4A9E-44D5-A1CD-06B4BAA87977")]
        public ActionResult<IEnumerable<RegisterStaticApiResponseViewModel>> RegisterAllStaticApi()
        {
            var resourceIds = ApiExplorer.ApiDescriptionGroups.Items[0].Items.GroupBy(x => GetResourceId(new ApiDescriptionWrapper(x))).Select(x => x.Key).Where(x => !string.IsNullOrEmpty(x)).ToList();
            var models = new List<StaticApiModel>();

            foreach (var resourceId in resourceIds)
            {
                models.Add(CreateStaticApiModel(resourceId));
            }
            var result = _apiDescriptionService.RegisterStaticApi(models);

            return Created(string.Empty, s_mapper.Map<IEnumerable<RegisterStaticApiResponseViewModel>>(result));
        }

        /// <summary>
        /// StaticApiのモデルを生成します。
        /// </summary>
        private StaticApiModel CreateStaticApiModel(string controllerId)
        {
            var apiDescriptions = ApiExplorer.ApiDescriptionGroups.Items[0].Items.Select(x => new ApiDescriptionWrapper(x))
                .GroupBy(x => GetResourceId(x)).FirstOrDefault(x => x.Key?.ToUpper() == controllerId?.ToString().ToUpper());
            
            if (apiDescriptions == null)
            {
                return new StaticApiModel()
                {
                    Controller = new ControllerInformationModel() { ControllerId = controllerId },
                    DetectedInSourceCode = false
                };
            }
            else
            {
                var apiSample = apiDescriptions.First();
                var lastPathIdx = apiSample.RelativePath.LastIndexOf('/');
                var resourceUrl = "/" + (lastPathIdx < 0 ? apiSample.RelativePath : apiSample.RelativePath.Substring(0, lastPathIdx));
                var resouceModel = new ControllerInformationModel()
                {
                    ControllerId = controllerId,
                    VendorId = _perRequestDataContainer.VendorId ?? UnityContainer.Resolve<string>("VendorSystemAuthenticationDefaultVendorId"),
                    SystemId = _perRequestDataContainer.SystemId ?? UnityContainer.Resolve<string>("VendorSystemAuthenticationDefaultSystemId"),
                    Url = resourceUrl,
                    IsStaticApi = true,
                    IsActive = true,
                    IsEnable = true
                };

                var apiList = new List<ApiInformationModel>();
                foreach (var apiDescription in apiDescriptions)
                {

                    // スキーマ情報を作成
                    var schemas = CreateSchemaInfo(apiDescription);

                    var httpMethod = new HttpMethod(apiDescription.HttpMethod);
                    var apiModel = new ApiInformationModel()
                    {
                        ControllerId = controllerId,
                        ApiId = GetApiId(apiDescription),
                        Url = lastPathIdx < 0 ? apiDescription.RelativePath : apiDescription.RelativePath.Substring(lastPathIdx + 1),
                        MethodType = httpMethod.Method.ToUpper(),
                        ActionType = GetActionType(httpMethod),
                        IsAdminAuthentication = IsXAdminRequired(apiDescription),
                        IsHeaderAuthentication = true,
                        IsOpenIdAuthentication = true,
                        IsEnable = true,
                        IsStaticApi = true,
                        IsActive = true,
                        IsHidden = true,
                        UrlSchemaId = schemas.urlSchema?.DataSchemaId
                    };
                    if (schemas.urlSchema != null ) 
                        DynamicApiService.RegistOrUpdateSchema(schemas.urlSchema, true);

                    apiList.Add(apiModel);
                }

                return new StaticApiModel()
                {
                    Controller = resouceModel,
                    ApiList = apiList
                };
            }
        }

        /// <summary>
        /// APIの定義からスキーマ情報を生成します。
        /// </summary>
        /// <param name="api">APIの定義</param>
        /// <returns>スキーマ情報</returns>
        private (DataSchemaInformationModel urlSchema, DataSchemaInformationModel bodySchema, DataSchemaInformationModel responseSchema) CreateSchemaInfo(ApiDescriptionWrapper api)
        {
            string defaultVendorId = _perRequestDataContainer.VendorId ?? UnityContainer.Resolve<string>("VendorSystemAuthenticationDefaultVendorId");

            var generator = new JSchemaGenerator() { DefaultRequired = Required.Default };

            DataSchemaInformationModel urlSchema = null;
            DataSchemaInformationModel bodySchema = null;
            DataSchemaInformationModel responseSchema = null;

            JSchema urlJSchema = null;
            var urlSchemaLocalization = new Dictionary<string, string>();

            // スキーマ情報を作成するローカルメソッド
            DataSchemaInformationModel createSchemaInfo(string schemaId, string schemaName, JSchema jsonSchema, string description, Dictionary<string, string> multiLanguageList)
            {
                return new DataSchemaInformationModel
                {
                    DataSchemaId = schemaId,
                    VendorId = defaultVendorId,
                    SchemaName = schemaName?.Length > 100 ? schemaName.Substring(0, 100) : schemaName,
                    DataSchema = jsonSchema.ToString(),
                    DataSchemaDescription = api.HttpMethod + " " + api.RelativePath + " " + description,
                    IsActive = true
                };
            }

            // 要素の型を取得するローカルメソッド
            Type getElementType(Type t)
            {
                if (t.IsArray) return t.GetElementType();
                if (t.GetInterface("IEnumerable") != null)
                    return t.GetGenericArguments().FirstOrDefault();
                return t;
            }


            // リクエストスキーマ情報作成
            foreach (var p in api.ParameterDescriptions)
            {
                 if (p.Source == BindingSource.Query)
                {
                    if (urlJSchema == null)
                    {
                        urlJSchema = new JSchema { Description = $"StaticAPI: {api.HttpMethod} {api.RelativePath}" };
                    }

                    var cSchema = generator.Generate(p.ParameterDescriptor.ParameterType);
                    urlJSchema.Properties.Add(p.Name, (JSchema)cSchema);
                }
            }

            // URLパラメータをまとめてスキーマ情報を作成
            if (urlJSchema != null)
            {
                var controllerInfo = api.ActionDescriptor as ControllerActionDescriptor;
                var methodIdAttr = controllerInfo.MethodInfo.GetCustomAttributes<ManageActionAttribute>().FirstOrDefault();
                urlSchema = createSchemaInfo(methodIdAttr.Id, urlJSchema.Description, urlJSchema, "Url", urlSchemaLocalization);
            }

            return (urlSchema, bodySchema, responseSchema);
        }

        /// <summary>
        /// リソースIDを取得します。
        /// </summary>
        private string GetResourceId(ApiDescriptionWrapper apiDescription)
        {
            if (apiDescription.ActionDescriptor is ControllerActionDescriptor cad)
            {
                return cad.ControllerTypeInfo.GetCustomAttribute<ManageApiAttribute>(false)?.Id;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// API IDを取得します。
        /// </summary>
        private string GetApiId(ApiDescriptionWrapper apiDescription)
        {
            if (apiDescription.ActionDescriptor is ControllerActionDescriptor cad)
            {
                return cad.MethodInfo.GetCustomAttribute<ManageActionAttribute>(false)?.Id;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 管理者認証の有無を取得します。
        /// </summary>
        private bool IsXAdminRequired(ApiDescriptionWrapper apiDescription)
        {
            if (apiDescription.ActionDescriptor is ControllerActionDescriptor cad)
            {
                return cad.MethodInfo.GetCustomAttribute<IgnoreXadminAttribute>(false) == null;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// HttpMethodからActionTypeを取得します。
        /// </summary>
        private string GetActionType(HttpMethod httpMethod)
        {
            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put) return "reg";
            if (httpMethod.Method.ToUpper() == "PATCH") return "upd";
            if (httpMethod == HttpMethod.Delete) return "del";

            return "quy";
        }
    }
}
