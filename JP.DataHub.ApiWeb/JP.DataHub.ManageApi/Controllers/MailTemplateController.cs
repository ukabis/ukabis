using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.ApiWeb.Models.MailTemplate;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.MailTemplate;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("66DE25A5-A4F6-4C7A-B92F-7DBE8FEEDCE5")]
    [UserRoleCheckController("DI_090")]
    public class MailTemplateController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MailTemplateModel, MailTemplateViewModel>()
                    .ForMember(d => d.From, o => o.MapFrom(s => s.FromMailAddress))
                    .ForMember(d => d.To, o => o.MapFrom(s => s.ToMailAddress))
                    .ForMember(d => d.Cc, o => o.MapFrom(s => s.CcMailAddress))
                    .ForMember(d => d.Bcc, o => o.MapFrom(s => s.BccMailAddress));

                cfg.CreateMap<MailTemplateViewModel, MailTemplateModel>()
                    .ForMember(d => d.FromMailAddress, o => o.MapFrom(s => s.From))
                    .ForMember(d => d.ToMailAddress, o => o.MapFrom(s => s.To))
                    .ForMember(d => d.CcMailAddress, o => o.MapFrom(s => s.Cc))
                    .ForMember(d => d.BccMailAddress, o => o.MapFrom(s => s.Bcc));

                cfg.CreateMap<MailTemplateModel, RegisterMailTemplateViewModel>()
                    .ForMember(d => d.From, o => o.MapFrom(s => s.FromMailAddress))
                    .ForMember(d => d.To, o => o.MapFrom(s => s.ToMailAddress))
                    .ForMember(d => d.Cc, o => o.MapFrom(s => s.CcMailAddress))
                    .ForMember(d => d.Bcc, o => o.MapFrom(s => s.BccMailAddress));

                cfg.CreateMap<RegisterMailTemplateViewModel, MailTemplateModel>()
                    .ForMember(d => d.FromMailAddress, o => o.MapFrom(s => s.From))
                    .ForMember(d => d.ToMailAddress, o => o.MapFrom(s => s.To))
                    .ForMember(d => d.CcMailAddress, o => o.MapFrom(s => s.Cc))
                    .ForMember(d => d.BccMailAddress, o => o.MapFrom(s => s.Bcc));
            }).CreateMapper();
        });

        private static IMapper Mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IMailTemplateApplicationService> _lazyMailTemplateApplicationService = new(() => UnityCore.Resolve<IMailTemplateApplicationService>());

        private IMailTemplateApplicationService MailTemplateApplicationService { get => _lazyMailTemplateApplicationService.Value; }

        /// <summary>
        /// メールテンプレートを登録します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_090", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("7D851EB4-FEF0-4932-94E2-DD2AB9802D9C")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<MailTemplateViewModel> RegisterMailTemplate(RegisterMailTemplateViewModel model)
        {
            var result = MailTemplateApplicationService.Register(Mapper.Map<MailTemplateModel>(model));
            return Created(string.Empty, Mapper.Map<MailTemplateViewModel>(result));
        }

        /// <summary>
        /// メールテンプレートを更新します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_090", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("60640555-5BE9-4192-9F3B-ABA09B948462")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<MailTemplateViewModel> UpdateMailTemplate(MailTemplateViewModel model)
        {
            var result = MailTemplateApplicationService.Register(Mapper.Map<MailTemplateModel>(model));
            return Created(string.Empty, Mapper.Map<MailTemplateViewModel>(result));
        }

        /// <summary>
        /// メールテンプレートを削除します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_090", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("9267F90E-E1A1-47D3-8062-36EEC360DB25")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteMailTemplate([RequiredGuid] string mailTemplateId)
        {
            MailTemplateApplicationService.Delete(mailTemplateId);
            return NoContent();
        }

        /// <summary>
        /// メールテンプレートを取得します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_090", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("AD6C4DE7-EE12-495D-823C-79E771E31A61")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<MailTemplateViewModel> GetMailTemplate([RequiredGuid] string mailTemplateId)
        {
            var result = MailTemplateApplicationService.Get(mailTemplateId);
            return Ok(Mapper.Map<MailTemplateViewModel>(result));
        }

        /// <summary>
        /// メールテンプレートリストを取得します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_090", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("2CDBFE0F-7E16-4350-9170-9BCF2A14212F")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<List<MailTemplateViewModel>> GetMailTemplateList([RequiredGuid] string vendorId)
        {
            var result = MailTemplateApplicationService.GetList(vendorId);
            return Ok(Mapper.Map<List<MailTemplateViewModel>>(result));
        }
    }
}