using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.RepositoryGroup;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.MVC.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace JP.DataHub.ManageApi.Controllers
{
    /// <summary>
    /// リポジトリグループのAPIを提供します。
    /// </summary>
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("812DCED7-C77A-4D47-9EC6-6A959685D395")]
    [UserRoleCheckController("DI_051")]
    public class RepositoryGroupController : AbstractController
    {
        private static IMapper Mapper => s_lazyMapper.Value;
        private static Lazy<IMapper> s_lazyMapper = new(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RepositoryGroupViewModel, RepositoryGroupModel>().ReverseMap();
                cfg.CreateMap<PhysicalRepositoryViewModel, PhysicalRepositoryModel>().ReverseMap();
                cfg.CreateMap<RepositoryTypeViewModel, RepositoryTypeModel>().ReverseMap();
                cfg.CreateMap<RegisterRepositoryGroupViewModel, RepositoryGroupModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });

        private IRepositoryGroupService RepositoryGroupService => _lazyRepositoryGroupService.Value;
        private Lazy<IRepositoryGroupService> _lazyRepositoryGroupService = new(() => UnityCore.Resolve<IRepositoryGroupService>());

        /// <summary>
        /// リポジトリーグループを取得します。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("05FAA0C3-AC42-4439-AD2F-65ED8788A3CC")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        [UserRole("DI_051", UserRoleAccessType.Read, false)]
        public ActionResult<RepositoryGroupViewModel> GetRepositoryGroup([RequiredGuid] string repositoryGroupId)
            => Mapper.Map<RepositoryGroupViewModel>(RepositoryGroupService.GetRepositoryGroup(repositoryGroupId)).ToActionResult();

        /// <summary>
        /// リポジトリーグループリストを取得します。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("2994A4F1-8284-4637-94B3-333B03E58763")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        [UserRole("DI_051", UserRoleAccessType.Read, false)]
        public ActionResult<List<RepositoryGroupViewModel>> GetRepositoryGroupList(string vendorId = null)
            => Mapper.Map<List<RepositoryGroupViewModel>>(RepositoryGroupService.GetRepositoryGroupListIncludePhysicalRepository(vendorId)).ToActionResult();

        /// <summary>
        /// リポジトリーグループタイプリストを取得します。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("F714A8F8-510F-4F3A-9850-771B0E5C4D66")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        [UserRole("DI_051", UserRoleAccessType.Read, false)]
        public ActionResult<List<RepositoryTypeViewModel>> GetRepositoryGroupTypeList()
            => Mapper.Map<List<RepositoryTypeViewModel>>(RepositoryGroupService.GetRepositoryGroupTypeList()).ToActionResult();

        /// <summary>
        /// リポジトリーグループを登録/更新します。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ManageAction("293D0AA6-60F8-4D56-9881-6783A99CC9BB")]
        [ExceptionFilter(typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        [UserRole("DI_051", UserRoleAccessType.Write, false)]
        public ActionResult<RepositoryGroupViewModel> RegisterRepositoryGroup(RegisterRepositoryGroupViewModel model)
        {
            var result = RepositoryGroupService.MergeRepositoryGroup(Mapper.Map<RepositoryGroupModel>(model));
            return Created(string.Empty, Mapper.Map<RepositoryGroupViewModel>(result));
        }

        /// <summary>
        /// リポジトリーグループを削除します。
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ManageAction("6C80415F-44A4-4947-9EAD-5CCB050576E2")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(InUseException), HttpStatusCode.BadRequest, typeof(MultipleUpdateException), HttpStatusCode.BadRequest)]
        [UserRole("DI_051", UserRoleAccessType.Write, false)]
        public ActionResult DeleteRepositoryGroup([RequiredGuid] string repositoryGroupId)
        {
            RepositoryGroupService.DeleteReposigoryGroup(repositoryGroupId);
            return NoContent();

        }
    }
}
