using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.ResourceSharingPerson;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("15F5CB26-CEB6-4DEE-8B9E-8855E26FBC44")]
    public class ResourceSharingPersonController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ResourceSharingPersonRuleModel, RegisterResourceSharingPersonViewModel>().ReverseMap();
                cfg.CreateMap<ResourceSharingPersonRuleModel, ResourceSharingPersonViewModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IResourceSharingPersonService> _lazyResourceSharingPersonService = new Lazy<IResourceSharingPersonService>(() => UnityCore.Resolve<IResourceSharingPersonService>());
        private IResourceSharingPersonService _resourceSharingPersonService { get => _lazyResourceSharingPersonService.Value; }

        /// <summary>
        ///  個人間データ共有を登録します。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAction("D316AD99-E00F-4540-BF7F-84730A58CBDF")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult RegisterResourceSharingPerson(RegisterResourceSharingPersonViewModel model)
        {
            var result = _resourceSharingPersonService.Register(s_mapper.Map<ResourceSharingPersonRuleModel>(model));
            return Created(string.Empty, s_mapper.Map<ResourceSharingPersonViewModel>(result));
        }

        /// <summary>
        ///  個人間データ共有を更新します。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAction("6FE7E0DB-B34A-4B9D-A5E7-F270724403DB")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult UpdateResourceSharingPerson(ResourceSharingPersonViewModel model)
        {
            var result = _resourceSharingPersonService.Update(s_mapper.Map<ResourceSharingPersonRuleModel>(model));
            return Created(string.Empty, s_mapper.Map<ResourceSharingPersonViewModel>(result));
        }

        /// <summary>
        ///  個人間データ共有を削除します。
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ManageAction("AA5C731B-85FD-46BD-9E72-6A8D41F139FF")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteResourceSharingPerson([RequiredGuid] string resourceSharingRuleId)
        {
            this._resourceSharingPersonService.Delete(resourceSharingRuleId);
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  個人間データ共有を取得します。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("36AE9DA9-16D7-471B-9650-5E69CA928D14")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult GetResourceSharingPerson([RequiredGuid] string resourceSharingRuleId)
        {
            var result = s_mapper.Map<ResourceSharingPersonViewModel>(_resourceSharingPersonService.Get(resourceSharingRuleId));
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        ///  個人間データ共有一覧を取得します。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("2FB38C1E-E25A-466D-B916-B9FF17F1679B")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult GetResourceSharingListByResourcePath(string resourcePath)
        {
            var result = s_mapper.Map<List<ResourceSharingPersonViewModel>>(_resourceSharingPersonService.GetList(resourcePath));
            return result?.Count != 0 ? Ok(result) : NotFound();
        }
    }
}