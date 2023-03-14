using AutoMapper;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Models.ApiWebhook;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.ManageApi.Attributes;

namespace JP.DataHub.ManageApi.Controllers
{
    /// <summary>
    /// WebhookのAPIを提供します。
    /// </summary>
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("F7841696-D11D-4176-828A-26043C33E7C7")]
    public class ApiWebhookController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApiWebhookViewModel, ApiWebhookModel>().ReverseMap();
                cfg.CreateMap<ApiWebhookModel, ApiWebhookRegisterViewModel>().ReverseMap();
                cfg.CreateMap<ApiWebhookModel, ApiWebhookUpdateViewModel>().ReverseMap();
                cfg.CreateMap<HttpHeaderModel, HttpHeaderViewModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });

        private static IMapper Mapper { get => s_lazyMapper.Value; }

        private Lazy<IApiWebhookService> _lazyApiWebhookService = new(() => UnityCore.Resolve<IApiWebhookService>());
        private IApiWebhookService ApiWebhookService { get => _lazyApiWebhookService.Value; }

        /// <summary>
        /// Webhookを登録します。
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [UserRole("DI_100", UserRoleAccessType.Write)]
        [ManageAction("94B4419E-07BE-4F74-A6AC-EF9DF12E567B")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult RegisterApiWebhook(ApiWebhookRegisterViewModel model)
        {
            var result = ApiWebhookService.Register(Mapper.Map<ApiWebhookModel>(model));
            return Created(string.Empty, new { result.ApiWebhookId });
        }

        /// <summary>
        /// Webhookを更新します。
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [UserRole("DI_100", UserRoleAccessType.Write)]
        [ManageAction("AEE7B260-A834-48DB-A308-31B11A8B4208")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult UpdateApiWebhook(ApiWebhookUpdateViewModel model)
        {
            var result = ApiWebhookService.Update(Mapper.Map<ApiWebhookModel>(model));
            return Created(string.Empty, new { result.ApiWebhookId });
        }

        /// <summary>
        /// Webhookを取得します。
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [UserRole("DI_100", UserRoleAccessType.Read)]
        [ManageAction("1285849A-E91F-4E4D-930B-210729AC1399")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ApiWebhookViewModel> GetApiWebhook([RequiredGuid] string apiWebhookId)
            => Ok(Mapper.Map<ApiWebhookViewModel>(ApiWebhookService.Get(apiWebhookId)));

        /// <summary>
        /// Webhookの一覧を取得します。
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [UserRole("DI_100", UserRoleAccessType.Read)]
        [ManageAction("CF38B2A6-1C49-4CC0-9E35-02ABD492261B")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public ActionResult<IList<ApiWebhookViewModel>> GetApiWebhookList([RequiredGuid] string vendorId)
            => Ok(Mapper.Map<IList<ApiWebhookViewModel>>(ApiWebhookService.GetList(vendorId)));

        /// <summary>
        /// Webhookを削除します。
        /// </summary>        
        /// <returns></returns>
        [HttpDelete]
        [UserRole("DI_100", UserRoleAccessType.Write)]
        [ManageAction("D9BC0944-BAE2-48BA-98B5-9891D4F6BE43")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteApiWebhook([RequiredGuid] string apiWebhookId)
        {
            ApiWebhookService.Delete(apiWebhookId);
            return NoContent();
        }

    }
}