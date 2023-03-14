using System;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Validations.Annotations.Attributes;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.ApiWeb.Models.Terms;
using JP.DataHub.ApiWeb.Domain.Service;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("BBDCC9E6-ABCF-4865-AC85-1F37A3CE156F")]
    public class TermsGroupController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TermsGroupViewModel, TermsGroupModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<ITermsService> _lazyTermsService = new Lazy<ITermsService>(() => UnityCore.Resolve<ITermsService>());
        private ITermsService _termsService { get => _lazyTermsService.Value; }

        /// <summary>
        /// すべての規約グループを取得する
        /// </summary>
        [HttpGet]
        [ManageAction("60D1B701-FFF1-4239-85A7-56FFE2D0ABE7")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60403)]
        public ActionResult<IList<TermsGroupViewModel>> GetList()
            => Ok(s_mapper.Map<List<TermsGroupViewModel>>(_termsService.GroupGetList()));

        /// <summary>
        /// 規約グループを登録する
        /// </summary>
        [Admin]
        [HttpPost]
        [ManageAction("5A81A919-68D2-4034-91B8-EF7B91A2E073")]
        [ExceptionFilter(typeof(ForeignKeyException), ErrorCodeMessage.Code.W60405)]
        public ActionResult Register([Required] TermsGroupViewModel model)
        {
            var result = _termsService.GroupRegister(s_mapper.Map<TermsGroupModel>(model));
            return Created(string.Empty, new { TermsGroupCode =result });
        }

        /// <summary>
        /// 規約グループを削除する
        /// </summary>
        [Admin]
        [HttpDelete]
        [ManageAction("205B59F6-726F-4EC9-AB07-6084B77983C5")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60404)]
        public ActionResult Delete([Required] string terms_group_code)
        {
            _termsService.GroupDelete(terms_group_code);
            return NoContent();
        }
    }
}
