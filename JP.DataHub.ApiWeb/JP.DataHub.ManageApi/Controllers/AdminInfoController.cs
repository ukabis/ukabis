using AutoMapper;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.AdminInfo;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace JP.DataHub.ManageApi.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("9579C2A0-998A-4186-9AFB-21340622A199")]
    [UserRoleCheckController("DI_041")]
    public class AdminInfoController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterAdminFuncInfomationViewModel, AdminFuncInfomationModel>().ReverseMap();
                cfg.CreateMap<AdminFuncInfomationViewModel, AdminFuncInfomationModel>().ReverseMap();
                cfg.CreateMap<UpdateAdminFuncInfomationViewModel, AdminFuncInfomationModel>().ReverseMap();
                cfg.CreateMap<AdminFuncRoleInfomationModel, AdminFuncRoleInfomationViewModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IAdminInfoService> _lazyAdminInfoService = new Lazy<IAdminInfoService>(() => UnityCore.Resolve<IAdminInfoService>());
        private IAdminInfoService _adminInfoService { get => _lazyAdminInfoService.Value; }

        [HttpPost]
        [UserRole("DI_041", UserRoleAccessType.Write, false)]
        [ManageAction("2CEBC922-C209-4F0F-84D6-8FAEB50E09AF")]
        [ExceptionFilter(typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<AdminFuncInfomationViewModel> RegisterAdminInfo(RegisterAdminFuncInfomationViewModel model)
            => Created(string.Empty, s_mapper.Map<AdminFuncInfomationViewModel>(_adminInfoService.RegistrationAdminFuncInfomation(s_mapper.Map<AdminFuncInfomationModel>(model))));

        [HttpGet]
        [UserRole("DI_041", UserRoleAccessType.Read, false)]
        [ManageAction("2D5567BE-7552-4A6A-A191-F91BD6CA0817")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<AdminFuncInfomationViewModel> GetAdminInfo(
            [RequiredGuid] string adminFuncRoleId)
            => Ok(s_mapper.Map<AdminFuncInfomationViewModel>(_adminInfoService.GetAdminInfo(adminFuncRoleId)));

        [HttpGet]
        [UserRole("DI_041", UserRoleAccessType.Read, false)]
        [ManageAction("B4920556-E3D3-41A2-8B49-D55EFDB4C998")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<AdminFuncRoleInfomationViewModel>> GetAdminInfoList()
        {
            return Ok(s_mapper.Map<List<AdminFuncRoleInfomationViewModel>>(_adminInfoService.GetAdminFuncRoleInfomationList()));
        }

        [HttpPost]
        [UserRole("DI_041", UserRoleAccessType.Write, false)]
        [ManageAction("276B5C3E-63AC-4841-B213-9318FD108708")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest)]
        public ActionResult<UpdateAdminFuncInfomationViewModel> UpdateAdminInfo(UpdateAdminFuncInfomationViewModel model)
        {
            _adminInfoService.UpdateAdminFuncInfo(s_mapper.Map<AdminFuncInfomationModel>(model));
            return Created(string.Empty, model);
        }

        [HttpDelete]
        [UserRole("DI_041", UserRoleAccessType.Write, false)]
        [ManageAction("1EBED423-7725-4227-AEFF-56DA66DF6919")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteAdminInfo([RequiredGuid] string adminFuncRoleId)
        {
            _adminInfoService.DeleteAdminInfo(adminFuncRoleId);
            return new NoContentResult();
        }
    }
}
