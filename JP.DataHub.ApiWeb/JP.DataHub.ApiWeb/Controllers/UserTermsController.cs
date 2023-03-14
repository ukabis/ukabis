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

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("D8A3B066-D970-4202-A03E-EE6DED5A0338")]
    public class UserTermsController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserTermsViewModel, UserTermsModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IUserTermsService> _lazyUserTermsService = new Lazy<IUserTermsService>(() => UnityCore.Resolve<IUserTermsService>());
        private IUserTermsService _userTermsService { get => _lazyUserTermsService.Value; }

        /// <summary>
        /// ユーザーが過去の同意や取消の情報をすべて返す
        /// </summary>
        [HttpGet]
        [ManageAction("7D597521-CDD8-4EFA-A54C-B3DAE4005C9B")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60412)]
        public ActionResult<IList<UserTermsViewModel>> GetList()
        {
            var result = _userTermsService.GetList(PerRequestDataContainer.OpenId);
            return Ok(s_mapper.Map<IList<UserTermsViewModel>>(result));
        }

        /// <summary>
        /// ユーザー規約同意情報を返す
        /// </summary>
        [HttpGet]
        [ManageAction("1C035A19-888B-466B-A6FB-8E89E7BB5A4B")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60412)]
        public ActionResult<UserTermsViewModel> Get([RequiredGuid]string user_terms_id)
        {
            var result = _userTermsService.Get(PerRequestDataContainer.OpenId, user_terms_id);
            return result == null ? throw new NotFoundException() : Ok(s_mapper.Map<UserTermsViewModel>(result));
        }
    }
}
