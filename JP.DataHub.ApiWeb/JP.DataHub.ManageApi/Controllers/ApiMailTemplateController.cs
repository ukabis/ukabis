using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.ApiWeb.Models.ApiMailTemplate;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.ApiMailTemplate;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("FEC321A7-0027-4C1C-BC51-A967585A4DCC")]
    [UserRoleCheckController("DI_091")]
    public class ApiMailTemplateController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApiMailTemplateModel, ApiMailTemplateViewModel>().ReverseMap();
                cfg.CreateMap<ApiMailTemplateModel, ApiMailTemplateViewModel>().ReverseMap();
                cfg.CreateMap<ApiMailTemplateModel, RegisterApiMailTemplateViewModel>().ReverseMap();
            }).CreateMapper();
        });
        private static IMapper Mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IMailTemplateApplicationService> _lazyMailTemplateApplicationService = new(() => UnityCore.Resolve<IMailTemplateApplicationService>());

        private IMailTemplateApplicationService ApiMailTemplateApplicationService { get => _lazyMailTemplateApplicationService.Value; }

        /// <summary>
        /// APIメールテンプレート設定の登録
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_091", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("0F66FD15-B35F-4492-A671-4895266045BD")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<ApiMailTemplateViewModel> RegisterApiMailTemplate(RegisterApiMailTemplateViewModel model)
        {
            var result = ApiMailTemplateApplicationService.RegisterApiMailTemplate(Mapper.Map<ApiMailTemplateModel>(model));
            return Created(string.Empty, Mapper.Map<ApiMailTemplateViewModel>(result));
        }
        /// <summary>
        /// APIメールテンプレート設定の更新
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_091", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("76FA1CB6-5A43-4BAA-BADF-09DC821BB7AB")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<ApiMailTemplateViewModel> UpdateApiMailTemplate(ApiMailTemplateViewModel model)
        {
            var result = ApiMailTemplateApplicationService.UpdateApiMailTemplate(Mapper.Map<ApiMailTemplateModel>(model));
            return Created(string.Empty, Mapper.Map<ApiMailTemplateViewModel>(result));
        }

        /// <summary>
        /// APIメールテンプレート設定の削除
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_091", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("A14C51A1-D6BA-46CD-B1BE-609FF5E9E8C3")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteApiMailTemplate([RequiredGuid] string apiMailTemplateId)
        {
            ApiMailTemplateApplicationService.DeleteApiMailTemplate(apiMailTemplateId);
            return NoContent();
        }

        /// <summary>
        /// APIメールテンプレート設定の取得
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_091", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("C5D09148-DD87-473C-B23C-B39B337D5CD2")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult GetApiMailTemplate([RequiredGuid] string apiMailTemplateId)
        {
            var result = ApiMailTemplateApplicationService.GetApiMailTemplate(apiMailTemplateId);
            return Ok(Mapper.Map<ApiMailTemplateViewModel>(result));
        }

        /// <summary>
        /// APIメールテンプレート設定一覧の取得
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_091", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("BDD76A73-70FD-45EA-874C-9EDBF3974C4D")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult GetApiMailTemplateList([RequiredGuid]string apiId, [ValidateGuid]string vendorId = null)
        {
            var result = ApiMailTemplateApplicationService.GetApiMailTemplateList(apiId, vendorId);
            return Ok(Mapper.Map<List<ApiMailTemplateViewModel>>(result));
        }
    }
}