using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Models.Role;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace JP.DataHub.ManageApi.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("854C084C-BC3E-4B28-A6AE-20A449018EA4")]
    [UserRoleCheckController("DI_041")]
    public class RoleController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterRoleViewModel, RoleModel>().ReverseMap();
                cfg.CreateMap<RoleViewModel, RoleModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IRoleService> _lazyRoleService = new Lazy<IRoleService>(() => UnityCore.Resolve<IRoleService>());
        private IRoleService _roleService { get => _lazyRoleService.Value; }

        [HttpGet]
        [UserRole("DI_041", UserRoleAccessType.Read, false)]
        [ManageAction("DFBD12AE-F7E4-4FC5-B8F5-8D27ADB0D813")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<RoleViewModel> GetRole(Guid roleId)
        {
            var hit = s_mapper.Map<RoleViewModel>(_roleService.GetRole(roleId));
            return hit == null ? NotFound() : Ok(hit);
        }

        [HttpGet]
        [UserRole("DI_041", UserRoleAccessType.Read, false)]
        [ManageAction("61C8C587-79FE-433A-BB32-4805BE748E56")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<List<RoleViewModel>> GetRoleList()
        {
            var hit = s_mapper.Map<List<RoleViewModel>>(_roleService.GetRoleList());
            return hit == null ? NotFound() : Ok(hit);
        }

        [HttpPost]
        [UserRole("DI_041", UserRoleAccessType.Write, false)]
        [ManageAction("FC4AAB68-DFFD-4F5A-8327-791E470637B5")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest)]
        public ActionResult<RoleViewModel> RegisterRole(RegisterRoleViewModel model)
        {
            var result = _roleService.RegistrationRole(s_mapper.Map<RoleModel>(model));
            return Created(string.Empty, s_mapper.Map<RoleViewModel>(result));
        }

        [HttpPost]
        [UserRole("DI_041", UserRoleAccessType.Write, false)]
        [ManageAction("4936C827-9376-45BD-A9DC-1E0B33BB3DA5")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(NotFoundException), HttpStatusCode.BadRequest)]
        public ActionResult<RoleViewModel> UpdateRole(RoleViewModel model)
        {
            _roleService.UpdateRole(s_mapper.Map<RoleModel>(model));
            return Created(string.Empty, model);
        }

        [HttpDelete]
        [UserRole("DI_041", UserRoleAccessType.Write, false)]
        [ManageAction("2BC6CD9C-4135-4BC9-8328-B6D88E5BD53D")]
        [ExceptionFilter(typeof(InUseException), HttpStatusCode.BadRequest, typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteRole(Guid RoleId)
        {
            _roleService.DeleteRole(RoleId);
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}