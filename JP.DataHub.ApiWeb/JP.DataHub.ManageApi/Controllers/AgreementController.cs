using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.Agreement;
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
    [ManageApi("54D623E2-8C5B-459A-8A8D-430F9E789BA5")]
    [UserRoleCheckController("DI_046")]
    public class AgreementController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AgreementViewModel, AgreementModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IAgreementService> _lazyAgreementService = new Lazy<IAgreementService>(() => UnityCore.Resolve<IAgreementService>());
        private IAgreementService _agreementService { get => _lazyAgreementService.Value; }

        [HttpGet]
        [ManageAction("F06F3C3B-183F-46CE-94F5-37E1C1021C51")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        [UserRole("DI_046", UserRoleAccessType.Read)]
        public ActionResult<AgreementViewModel> GetAgreement([RequiredGuid] string agreementId)
        {
            var hit = s_mapper.Map<AgreementViewModel>(_agreementService.GetAgreement(agreementId));
            return Ok(hit);
        }

        [HttpGet]
        [ManageAction("3D7EA23E-E947-4640-8A90-A13BE78823BD")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        [UserRole("DI_046", UserRoleAccessType.Read)]
        public ActionResult<List<AgreementViewModel>> GetAgreementList()
        {
            var result = _agreementService.GetAgreementList();
            return Ok(s_mapper.Map<List<AgreementViewModel>>(result));
        }

        [HttpGet]
        [ManageAction("C2796FC3-E3BC-46E0-9EAD-05FDAD43080C")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        [UserRole("DI_046", UserRoleAccessType.Read)]
        public ActionResult<List<AgreementViewModel>> GetAgreementListByVendorId([RequiredGuid] string vendorId)
        {
            var result = _agreementService.GetAgreementList(vendorId.ToString());
            return Ok(s_mapper.Map<List<AgreementViewModel>>(result));
        }

        [HttpPost]
        [ManageAction("2410448B-5FFF-40D1-A892-C5D26B29C93F")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest)]
        [UserRole("DI_046", UserRoleAccessType.Write)]
        public ActionResult RegisterAgreement(AgreementViewModel model)
        {
            var result = _agreementService.RegistAgreement(s_mapper.Map<AgreementModel>(model));
            return Created(string.Empty, new { result.AgreementId });
        }

        [HttpPost]
        [ManageAction("E3C394E2-E92D-4D46-878D-8BF8250E2842")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest)]
        [UserRole("DI_046", UserRoleAccessType.Write)]
        public ActionResult UpdateAgreement(AgreementViewModel model)
        {
            _agreementService.GetAgreement(model.AgreementId);
            var result = _agreementService.UpdateAgreement(s_mapper.Map<AgreementModel>(model));
            return Created(string.Empty, new { result.AgreementId });
        }

        [HttpDelete]
        [ManageAction("9FF19293-B030-41CC-AEF2-EE7035D541C0")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        [UserRole("DI_046", UserRoleAccessType.Write)]
        public ActionResult DeleteAgreement([RequiredGuid] string agreementId)
        {
            _agreementService.DeleteAgreement(agreementId);
            return NoContent();
        }
    }
}