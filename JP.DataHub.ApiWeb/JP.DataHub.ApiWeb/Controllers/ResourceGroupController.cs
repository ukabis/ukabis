using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Validations.Annotations.Attributes;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.ApiWeb.Models.ResourceGroup;
using JP.DataHub.ApiWeb.Domain.Service;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Service.Impl;

namespace JP.DataHub.ApiWeb.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("C9AD813C-11E9-4B36-BBCC-4A2797683F20")]
    public class ResourceGroupController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ResourceGroupViewModel, ResourceGroupModel>().ReverseMap();
                cfg.CreateMap < ResourceGroupInResourceViewMode, ResourceGroupInResourceModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IResourceGroupService> _lazyResourceGroupService = new Lazy<IResourceGroupService>(() => UnityCore.Resolve<IResourceGroupService>());
        private IResourceGroupService _resourceGroupService { get => _lazyResourceGroupService.Value; }

        /// <summary>
        /// すべてのリソースグループを取得する
        /// </summary>
        [HttpGet]
        [ManageAction("7BB4759C-A3C7-4E64-9302-55B41F2B261E")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60429)]
        public ActionResult<IList<ResourceGroupViewModel>> GetList()
        {
            var result = _resourceGroupService.GetList();
            return Ok(s_mapper.Map<List<ResourceGroupViewModel>>(result));
        }

        /// <summary>
        /// リソースグループを登録する
        /// </summary>
        [Admin]
        [HttpPost]
        [ManageAction("0AFD57A0-67AF-4838-8476-ED9467F7167B")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60430)]
        public ActionResult Register(ResourceGroupViewModel model)
        {
            var result = _resourceGroupService.Register(s_mapper.Map<ResourceGroupModel>(model));
            return Created(string.Empty, new { ResourceGroupId = result });
        }

        /// <summary>
        /// リソースグループを削除する
        /// </summary>
        [Admin]
        [HttpDelete]
        [ManageAction("17C40E09-84AD-4E40-9E17-9DE630A479E4")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60431)]
        public ActionResult Delete([RequiredGuid] string resource_group_id)
        {
            _resourceGroupService.Delete(resource_group_id);
            return NoContent();
        }
    }
}
