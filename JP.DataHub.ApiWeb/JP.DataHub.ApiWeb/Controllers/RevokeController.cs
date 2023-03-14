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
using JP.DataHub.ApiWeb.Models.Revoke;
using JP.DataHub.ApiWeb.Domain.Service;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.Api.Core.Exceptions;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("BBDCC9E6-ABCF-4865-AC85-1F37A3CE156F")]
    public class RevokeController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserRevokeViewModel, UserRevokeModel>().ReverseMap();
                cfg.CreateMap <RemoveHistoryViewModel, RemoveHistoryModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IRevokeService> _lazyRevokeService = new Lazy<IRevokeService>(() => UnityCore.Resolve<IRevokeService>());
        private IRevokeService _revokeService { get => _lazyRevokeService.Value; }

        /// <summary>
        /// リボークバッチの開始
        /// </summary>
        [HttpGet]
        [ManageAction("BF3D9C1C-AC9A-48EC-BB3D-93F50F5AAAD0")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60422, typeof(ForeignKeyException), ErrorCodeMessage.Code.E60423)]
        public ActionResult<UserRevokeViewModel> Start([RequiredGuid] string user_terms_id, [RequiredGuid] string open_id)
        {
            var result = _revokeService.Start(user_terms_id, open_id);
            return Ok(s_mapper.Map<UserRevokeViewModel>(result));
        }

        /// <summary>
        /// リボークバッチの終了
        /// </summary>
        [HttpGet]
        [ManageAction("48911D04-3A4F-4BF7-8BE5-309C363083EC")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60424, typeof(AlreadyExistsException), ErrorCodeMessage.Code.E60425)]
        public ActionResult Stop([RequiredGuid] string user_revoke_id, [RequiredGuid] string open_id)
        {
            _revokeService.Stop(user_revoke_id, open_id);
            return Ok();
        }

        /// <summary>
        /// リボークバッチのリソース削除開始
        /// </summary>
        [HttpGet]
        [ManageAction("45CD972A-EDA9-4CCA-8046-68EF57CF1621")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60426)]
        public ActionResult<RemoveHistoryModel> RemoveResourceStart([RequiredGuid] string user_revoke_id, [RequiredGuid] string resource_id, [RequiredGuid] string open_id)
        {
            var result = _revokeService.RemoveResourceStart(user_revoke_id, resource_id, open_id);
            return Ok(s_mapper.Map<RemoveHistoryViewModel>(result));
        }

        /// <summary>
        /// リボークバッチのリソース削除終了
        /// </summary>
        [Admin]
        [HttpGet]
        [ManageAction("A283BA2D-CB99-4493-BA0D-6E3CF60204B9")]
        [ExceptionFilter(typeof(NotFoundException), ErrorCodeMessage.Code.E60427, typeof(AlreadyExistsException), ErrorCodeMessage.Code.E60428)]
        public ActionResult RemoveResourceStop([RequiredGuid]string revoke_history_id, [RequiredGuid] string open_id)
        {
            _revokeService.RemoveResourceStop(revoke_history_id, open_id);
            return Ok();
        }
    }
}
