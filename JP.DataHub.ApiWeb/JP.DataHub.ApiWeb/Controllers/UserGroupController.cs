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
using JP.DataHub.ApiWeb.Models.UserTerms;
using JP.DataHub.ApiWeb.Domain.Service;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Models.Terms;
using JP.DataHub.ApiWeb.Domain.Service.Impl;
using JP.DataHub.ApiWeb.Models.UserGroup;

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("75F0A7A1-3A82-4452-AB36-AEF247082967")]
    public class UserGroupController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserGroupViewModel, UserGroupModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IUserGroupService> _lazyUserGroupService = new Lazy<IUserGroupService>(() => UnityCore.Resolve<IUserGroupService>());
        private IUserGroupService _userGroupService { get => _lazyUserGroupService.Value; }

        /// <summary>
        /// データ共有グループの一覧を取得
        /// </summary>
        [HttpGet]
        [ManageAction("3DBC642C-8748-4A14-BC56-249B8EDFE8D1")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60416)]
        public ActionResult<IList<UserGroupViewModel>> GetList()
        {
            var result = _userGroupService.GetList(PerRequestDataContainer.OpenId);
            return Ok(s_mapper.Map<IList<UserGroupModel>>(result));
        }

        /// <summary>
        /// データ共有グループを登録
        /// </summary>
        [Admin]
        [HttpPost]
        [ManageAction("F607E9DD-0FD9-4D81-A7CF-DE534CADEF52")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60416)]
        public ActionResult Register(UserGroupViewModel model)
        {
            var result = _userGroupService.Register(s_mapper.Map<UserGroupModel>(model));
            return Created(string.Empty, new { UserGroupId = result });
        }

        /// <summary>
        /// データ共有グループを削除
        /// </summary>
        [Admin]
        [HttpDelete]
        [ManageAction("99A5BEC9-CBE1-4701-93A5-52DCFE0AA982")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60417)]
        public ActionResult Delete([RequiredGuid] string user_group_id)
        {
            _userGroupService.Delete(user_group_id);
            return NoContent();
        }
    }
}
