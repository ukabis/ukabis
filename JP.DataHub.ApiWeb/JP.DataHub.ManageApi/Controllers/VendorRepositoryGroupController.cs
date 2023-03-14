using AutoMapper;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Models.VendorRepositoryGroup;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.ManageApi.Attributes;

namespace JP.DataHub.ManageApi.Controllers
{
    /// <summary>
    /// ベンダーリポジトリグループのAPIを提供します。
    /// </summary>
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("029C9A17-C5F5-46DC-85E6-7432404BC2CA")]
    public class VendorRepositoryGroupController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VendorRepositoryGroupModel, ActivateVendorRepositoryGroupViewModel>().ReverseMap();
                cfg.CreateMap<VendorRepositoryGroupModel, VendorRepositoryGroupViewModel>().ReverseMap();
                cfg.CreateMap<VendorRepositoryGroupListModel, VendorRepositoryGroupListViewModel>().ReverseMap();
                cfg.CreateMap<VendorRepositoryGroupModel, VendorRepositoryGroupListItemsViewModel>()
                    .ForMember(dst => dst.RepositoryGroupId, ops => ops.MapFrom(src => src.RepositoryGroupId))
                    .ForMember(dst => dst.RepositoryGroupName, ops => ops.MapFrom(src => src.RepositoryGroupName))
                    .ForMember(dst => dst.Used, ops => ops.MapFrom(src => src.Used));
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get => s_lazyMapper.Value; }

        private Lazy<IRepositoryGroupService> _lazyRepositoryGroupService = new(() => UnityCore.Resolve<IRepositoryGroupService>());
        private IRepositoryGroupService _repositoryGroupService { get => _lazyRepositoryGroupService.Value; }

        /// <summary>
        /// ベンダーリポジトリグループを有効・無効化します。
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [ManageAction("3CBC19B9-3321-4ABA-9793-B57DF65497F1")]
        [UserRole("DI_042", UserRoleAccessType.Write)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(ArgumentNullException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest,typeof(InUseException), HttpStatusCode.BadRequest)]
        public ActionResult<ActivateVendorRepositoryGroupViewModel> ActivateVendorRepositoryGroup(ActivateVendorRepositoryGroupViewModel model)
        {
            _repositoryGroupService.ActivateVendorRepositoryGroup(s_mapper.Map<VendorRepositoryGroupModel>(model));
            return Created(string.Empty, model);
        }

        /// <summary>
        /// ベンダーリポジトリグループを有効・無効化します。
        /// ※有効なものだけを送ってくること。
        /// 内部処理としては、有効のものをINSERTする前に全部論理削除する
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [ManageAction("3CBC19B9-3321-4ABA-9793-B57DF65497F2")]
        [UserRole("DI_042", UserRoleAccessType.Write)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(ArgumentNullException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest, typeof(InUseException), HttpStatusCode.BadRequest)]
        public ActionResult<IList<ActivateVendorRepositoryGroupViewModel>> ActivateVendorRepositoryGroupList(IList<ActivateVendorRepositoryGroupViewModel> model)
        {
            if (model.Count() == 0)
            {
                return BadRequest(new { Message = "リストで要素は1以上を指定してください" });
            }
            if (model.Where(x => x.Active == false).Count() > 0)
            {
                return BadRequest(new { Message = "無効にするものは送る必要がありません。有効なリストのみを指定してください" });
            }
            _repositoryGroupService.ActivateVendorRepositoryGroupList(s_mapper.Map<IList<VendorRepositoryGroupModel>>(model));
            return Created(string.Empty, model);
        }

        /// <summary>
        /// ベンダーが利用できるリポジトリグループを取得します。
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [ManageAction("278FAE70-10E1-4FB5-8AD9-20CB75F75340")]
        [UserRole("DI_042", UserRoleAccessType.Read)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public ActionResult<IList<VendorRepositoryGroupViewModel>> GetRepositoryGroupListByVendor([RequiredGuid] string vendorId)
            => Ok(s_mapper.Map<IList<VendorRepositoryGroupViewModel>>(_repositoryGroupService.GetVendorRepositoryGroupList(vendorId)));

        /// <summary>
        /// ベンダーリポジトリグループを取得します。
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [ManageAction("278FAE70-10E1-4FB5-8AD9-20CB75F7523F")]
        [UserRole("DI_042", UserRoleAccessType.Read)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public ActionResult<VendorRepositoryGroupViewModel> GetVendorRepositoryGroup([RequiredGuid] string vendorId, [RequiredGuid] string repositoryGroupId)
            => Ok(s_mapper.Map<VendorRepositoryGroupViewModel>(_repositoryGroupService.GetVendorRepositoryGroup(repositoryGroupId, vendorId)));

        /// <summary>
        /// ベンダー毎のベンダーリポジトリグループの一覧を取得します。
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [ManageAction("EEC4E0F5-0ACC-4836-8C0A-7FAD22BF21FA")]
        [UserRole("DI_042", UserRoleAccessType.Read)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public ActionResult<List<VendorRepositoryGroupListViewModel>> GetVendorRepositoryGroupList([ValidateGuid] string vendorId = null)
        {
            IList<VendorRepositoryGroupModel> vendorRepositoryGroupModels;

            if (string.IsNullOrEmpty(vendorId))
            {
                vendorRepositoryGroupModels = _repositoryGroupService.GetVendorRepositoryGroupList();
            }
            else
            {
                vendorRepositoryGroupModels = _repositoryGroupService.GetVendorRepositoryGroupList(vendorId);
            }
            var vendorIdList = vendorRepositoryGroupModels.Select(x => x.VendorId).Distinct().ToList();
            var result = vendorIdList.Select(x => new VendorRepositoryGroupListViewModel() { VendorId = x, VendorRepositoryGroupItems = vendorRepositoryGroupModels.Where(v => x == v.VendorId).Select(x => s_mapper.Map<VendorRepositoryGroupListItemsViewModel>(x)).ToList() }).ToList();
            return Ok(result);

        }
    }
}