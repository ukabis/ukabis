using AutoMapper;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.ResourceSharing;
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
    [ManageApi("B807119A-5B5C-469F-AD21-852E1923C9D8")]
    public class ResourceSharingController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ResourceSharingRuleModel, ResourceSharingViewModel>().ReverseMap();
                cfg.CreateMap<ResourceSharingViewModel, RegisterResourceSharingViewModel>().ReverseMap();
                cfg.CreateMap<ResourceSharingRuleModel, RegisterResourceSharingViewModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IResourceSharingService> _lazyResourceSharingService = new Lazy<IResourceSharingService>(() => UnityCore.Resolve<IResourceSharingService>());
        private IResourceSharingService _resourceSharingService { get => _lazyResourceSharingService.Value; }


        /// <summary>
        ///  データ共有を登録します。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAction("2A971734-9F94-42AF-A6D0-39D558A38AA9")]
        [ExceptionFilter(typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult RegisterResourceSharing(RegisterResourceSharingViewModel model)
        {
            var result = _resourceSharingService.RegisterResourceSharingRule(s_mapper.Map<ResourceSharingRuleModel>(model));
            return Created(string.Empty, result);
        }

        /// <summary>
        ///  データ共有を更新します。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAction("7444AFC8-40E4-4A40-A69A-DC9E27259505")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult UpdateResourceSharing(ResourceSharingViewModel model)
        {
            _resourceSharingService.UpdateResourceSharingRule(s_mapper.Map<ResourceSharingRuleModel>(model));
            return Created(string.Empty, model);
        }

        /// <summary>
        ///  データ共有を削除します。
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ManageAction("99E4C2AE-3DF4-494E-BA79-7C632D79BAE7")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]

        public ActionResult DeleteResourceSharing([RequiredGuid] string resourceSharingRuleId)
        {
            _resourceSharingService.DeleteResourceSharingRule(resourceSharingRuleId);
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  データ共有を取得します。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("7ADDA30A-5D31-49EB-A60F-3AF68846BDA8")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult GetResourceSharing([RequiredGuid] string resourceSharingRuleId)
        {
            var result = _resourceSharingService.GetResourceSharingRule(resourceSharingRuleId);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        ///  データ共有一覧を取得します。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("F1E2EF5A-B3AE-4A1E-8C87-152577847FB4")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]

        public ActionResult GetResourceSharingList([RequiredGuid] string vendorId,
            [RequiredGuid] string systemId,
            [RequiredGuid] string apiId)
        {
            var result = _resourceSharingService.GetResourceSharingRuleList(apiId,vendorId, systemId);
            return result?.Count != 0 ? Ok(s_mapper.Map<List<ResourceSharingViewModel>>(result)) : NotFound();
        }
    }
}