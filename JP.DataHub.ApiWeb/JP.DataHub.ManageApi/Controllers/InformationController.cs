using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.Information;
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
    [ManageApi("D2E75822-2B49-4C6B-8D2D-98527EB4A87C")]
    [UserRoleCheckController("DI_038")]
    public class InformationController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InformationViewModel, InformationModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IInformationService> _lazyInformationService = new Lazy<IInformationService>(() => UnityCore.Resolve<IInformationService>());
        private IInformationService _informationService { get => _lazyInformationService.Value; }

        /// <summary>
        /// お知らせを取得します。
        /// </summary>
        /// <param name="informationId">informationId</param>
        /// <returns>取得結果</returns>
        [HttpGet]
        [Admin]
        [ManageAction("BEA041E3-F4BE-4E37-8032-82A2E3979480")]
        [UserRole("DI_038", UserRoleAccessType.Read, false)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<InformationViewModel> GetInformation([RequiredGuid] string informationId)
        {
            return Ok(s_mapper.Map<InformationViewModel>(_informationService.Get(informationId)));
        }

        /// <summary>
        /// お知らせ一覧を取得します。
        /// </summary>
        /// <param name="getInformationCount">取得件数、指定しない場合は全件</param>
        /// <param name="isVisibleAdmin">管理画面表示分のみ取得する場合はtrue、指定しない場合はfalse</param>
        /// <param name="isVisibleApi">ヘルプページ表示分のみ取得する場合はtrue、指定しない場合はfalse</param>
        /// <returns>取得結果</returns>
        [HttpGet]
        [Admin]
        [ManageAction("F164DD64-7B99-41FA-8CEC-E2FF5DF18169")]
        [UserRole("DI_038", UserRoleAccessType.Read, false)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<List<InformationViewModel>> GetInformationList(int? getInformationCount = null, bool isVisibleApi = false, bool isVisibleAdmin = false)
        {
            return Ok(s_mapper.Map<List<InformationViewModel>>(_informationService.GetList(getInformationCount, isVisibleApi, isVisibleAdmin)));
        }

        /// <summary>
        /// お知らせを登録します。
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>InformationId</returns>
        [HttpPost]
        [Admin]
        [ManageAction("5D8B0646-467B-49E7-8865-8C8D4B31791E")]
        [UserRole("DI_038", UserRoleAccessType.Write, false)]
        public ActionResult RegisterInformation(InformationViewModel model)
        {
            var result = _informationService.Registration(s_mapper.Map<InformationModel>(model));
            return Ok(new { result.InformationId });
        }

        /// <summary>
        /// お知らせを更新します。
        /// </summary>        
        /// <param name="model">model</param>
        /// <returns>InformationId</returns>
        [HttpPost]
        [Admin]
        [ManageAction("6F0B728F-59CE-4A8E-B7D5-CD6999F9FD67")]
        [UserRole("DI_038", UserRoleAccessType.Write, false)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult UpdateInformation(InformationViewModel model)
        {
            _informationService.Update(s_mapper.Map<InformationModel>(model));
            return Ok(new { model.InformationId });
        }

        /// <summary>
        /// お知らせを削除します。
        /// </summary>
        [HttpDelete]
        [Admin]
        [ManageAction("3C54B35C-ACD5-4C73-B00C-5EE7BAE17EBC")]
        [UserRole("DI_038", UserRoleAccessType.Write, false)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteInformation([RequiredGuid] string informationId)
        {
            _informationService.Delete(informationId);
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}