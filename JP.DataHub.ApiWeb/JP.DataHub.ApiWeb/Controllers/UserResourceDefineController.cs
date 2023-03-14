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
using JP.DataHub.ApiWeb.Models.UserResourceShare;

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("89E243FC-A76E-4086-B87D-421BF01523F4")]
    public class UserResourceDefineController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserResourceShareViewModel, UserResourceShareModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IUserResourceShareService> _lazyUserResourceShareService = new Lazy<IUserResourceShareService>(() => UnityCore.Resolve<IUserResourceShareService>());
        private IUserResourceShareService _userResourceShareService { get => _lazyUserResourceShareService.Value; }

        /// <summary>
        /// ユーザーのデータ共有定義の一覧を取得
        /// </summary>
        [HttpGet]
        [ManageAction("5CE64C9D-68C1-4D46-8E1C-121BA695FEC3")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60418)]
        public ActionResult<IList<UserResourceShareViewModel>> GetList()
        {
            var result = _userResourceShareService.GetList(PerRequestDataContainer.OpenId);
            return Ok(s_mapper.Map<IList<UserResourceShareModel>>(result));
        }

        /// <summary>
        /// ユーザーのデータ共有定義を登録
        /// </summary>
        [Admin]
        [HttpPost]
        [ManageAction("53E3C99E-20C4-4B80-BDF0-3A36D8C79E37")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60419, typeof(ForeignKeyException), ErrorCodeMessage.Code.E60420)]
        public ActionResult Register(UserResourceShareViewModel model)
        {
            var result = _userResourceShareService.Register(s_mapper.Map<UserResourceShareModel>(model));
            return Created(string.Empty, new { UserResourceGroupId = result });
        }

        /// <summary>
        /// ユーザーのデータ共有定義を削除
        /// </summary>
        [Admin]
        [HttpDelete]
        [ManageAction("57912F29-5F1D-4264-A2F3-DA8D4DD56218")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60421)]
        public ActionResult Delete([RequiredGuid] string user_resource_group_id)
        {
            _userResourceShareService.Delete(user_resource_group_id);
            return NoContent();
        }
    }
}
