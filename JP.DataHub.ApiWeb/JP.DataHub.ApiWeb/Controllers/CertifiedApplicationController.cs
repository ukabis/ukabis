using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Dapper;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Validations.Annotations.Attributes;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.ApiWeb.Models.CertifiedApplication;
using JP.DataHub.ApiWeb.Domain.Service;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.ApiWeb.Domain.Service.Impl;

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("DA62F51C-F3CA-4C18-89F5-56648BCCBEA0")]
    public class CertifiedApplicationController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CertifiedApplicationViewModel, CertifiedApplicationModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<ICertifiedApplicationService> _lazyCertifiedApplicationService = new Lazy<ICertifiedApplicationService>(() => UnityCore.Resolve<ICertifiedApplicationService>());
        private ICertifiedApplicationService _certifiedApplicationService { get => _lazyCertifiedApplicationService.Value; }

        /// <summary>
        /// 認定アプリケーションの取得
        /// </summary>
        [Admin]
        [HttpGet]
        [ManageAction("624DB3A3-04DD-4027-9DDA-F2522916F1F6")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60413)]
        public ActionResult<IList<CertifiedApplicationViewModel>> GetList()
        {
            var result = _certifiedApplicationService.GetList();
            return Ok(s_mapper.Map<IList<CertifiedApplicationViewModel>>(result));
        }

        /// <summary>
        /// 認定アプリケーションを１つ取得
        /// </summary>
        [Admin]
        [HttpGet]
        [ManageAction("B7DB85B6-5381-42D3-8E9C-A7357CECA0B4")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60413)]
        public ActionResult<CertifiedApplicationViewModel> Get([RequiredGuid] string certified_application_id)
        {
            var result = _certifiedApplicationService.Get(certified_application_id);
            return result == null ? NotFound() : Ok(s_mapper.Map<CertifiedApplicationViewModel>(result));
        }

        /// <summary>
        /// 認定アプリケーションを登録する
        /// </summary>
        [Admin]
        [HttpPost]
        [ManageAction("12914B91-EFD8-4958-8668-34BC5E2704CE")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60414)]
        public ActionResult Register(CertifiedApplicationViewModel model)
        {
            var result = _certifiedApplicationService.Register(s_mapper.Map<CertifiedApplicationModel>(model));
            return Created(string.Empty, new { CertifiedApplicationId = result });
        }

        /// <summary>
        /// 認定アプリケーションを削除する
        /// </summary>
        [Admin]
        [HttpDelete]
        [ManageAction("1901274A-FE7F-451D-9060-5BB0EBE5C705")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.W60415)]
        public ActionResult Delete([RequiredGuid] string certified_application_id)
        {
            _certifiedApplicationService.Delete(certified_application_id);
            return NoContent();
        }
    }
}
