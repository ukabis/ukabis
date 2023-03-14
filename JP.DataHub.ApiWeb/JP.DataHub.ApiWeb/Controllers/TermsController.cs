using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Validations.Annotations.Attributes;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
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
    public class TermsController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TermsViewModel, TermsModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<ITermsService> _lazyTermsService = new Lazy<ITermsService>(() => UnityCore.Resolve<ITermsService>());
        private ITermsService _termsService { get => _lazyTermsService.Value; }

        /// <summary>
        /// 規約IDに合致する規約情報を返す
        /// </summary>
        [HttpGet]
        [ManageAction("BF3D9C1C-AC9A-48EC-BB3D-93F50F5AAAD0")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60406)]
        public ActionResult<TermsViewModel> Get([RequiredGuid] string terms_id)
        {
            var result = _termsService.TermsGet(terms_id);
            return Ok(s_mapper.Map<TermsViewModel>(result));
        }

        /// <summary>
        /// すべての規約を取得する
        /// </summary>
        [HttpGet]
        [ManageAction("48911D04-3A4F-4BF7-8BE5-309C363083EC")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60406)]
        public ActionResult<IList<TermsViewModel>> GetList()
        {
            var result = _termsService.TermsGetList();
            return Ok(s_mapper.Map<List<TermsViewModel>>(result));
        }

        /// <summary>
        /// 規約グループコードに合致した規約を取得する
        /// </summary>
        [HttpGet]
        [ManageAction("45CD972A-EDA9-4CCA-8046-68EF57CF1621")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60406)]
        public ActionResult<IList<TermsViewModel>> GetListByTermGroupCode(string terms_group_code)
        {
            var result = _termsService.TermsGetListByTermGroupCode(terms_group_code);
            return Ok(s_mapper.Map<List<TermsViewModel>>(result));
        }

        /// <summary>
        /// 規約を登録する
        /// </summary>
        [Admin]
        [HttpPost]
        [ManageAction("A283BA2D-CB99-4493-BA0D-6E3CF60204B9")]
        [ExceptionFilter(typeof(ForeignKeyException), ErrorCodeMessage.Code.W60408)]
        public ActionResult Register(TermsViewModel model)
        {
            var result = _termsService.TermsRegister(s_mapper.Map<TermsModel>(model));
            return Created(string.Empty, new { TermsId = result });
        }

        /// <summary>
        /// 規約を削除する
        /// </summary>
        [Admin]
        [HttpDelete]
        [ManageAction("5F30FB5D-4471-4295-9B06-C2D0E64DA415")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60407)]
        public ActionResult Delete([RequiredGuid] string terms_id)
        {
            _termsService.TermsDelete(terms_id);
            return NoContent();
        }

        /// <summary>
        /// ユーザーがterms_idで指定した規約に同意する
        /// </summary>
        /// <param name="terms_id">規約ID</param>
        /// <returns></returns>
        [HttpPost]
        [ManageAction("0D0A775A-FFDB-4B31-A00F-E9EDF3E8EC50")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60409, typeof(AlreadyExistsException), ErrorCodeMessage.Code.E60410)]
        public ActionResult Agreement([RequiredGuid] string terms_id)
        {
            _termsService.Agreement(this.PerRequestDataContainer.OpenId, terms_id);
            return Ok();
        }

        /// <summary>
        /// ユーザーがterms_idで指定した規約の同意に撤回する
        /// </summary>
        /// <param name="terms_id">規約ID</param>
        /// <returns></returns>
        [HttpPost]
        [ManageAction("4978C741-E728-4D52-80C4-3F525D6DBEE0")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60411)]
        public ActionResult Revoke([RequiredGuid] string terms_id)
        {
            _termsService.Revoke(this.PerRequestDataContainer.OpenId, terms_id);
            return Ok();
        }
    }
}
