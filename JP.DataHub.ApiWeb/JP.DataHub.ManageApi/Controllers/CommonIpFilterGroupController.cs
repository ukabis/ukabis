using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Models.Authority;
using JP.DataHub.ManageApi.Models.CommonIpFilterGroup;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("9DCAEA08-27A3-41CA-9D2C-F73ED03F7D9A")]
    [UserRoleCheckController("DI_040")]
    public class CommonIpFilterGroupController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CommonIpFilterGroupModel, CommonIpFilterGroupIpListViewModel>()
                    .ForMember(d => d.CommonIpFilterGroupId, o => o.MapFrom(s => s.CommonIpFilterGroupId))
                    .ForMember(d => d.CommonIpFilterGroupName, o => o.MapFrom(s => s.CommonIpFilterGroupName))
                    .ForMember(d => d.IpList, o => o.MapFrom(s => s.IpList))
                    .ReverseMap();
                cfg.CreateMap<CommonIpFilterModel, CommonIpFilterViewModel>()
                    .ForMember(d => d.CommonIpFilterId, o => o.MapFrom(s => s.CommonIpFilterId))
                    .ForMember(d => d.IpAddress, o => o.MapFrom(s => s.IpAddress))
                    .ForMember(d => d.IsEnable, o => o.MapFrom(s => s.IsEnable))
                    .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive))
                    .ReverseMap();
                cfg.CreateMap<CommonIpFilterGroupNameModel, CommonIpFilterGroupInfoViewModel>()
                    .ForMember(d => d.CommonIpFilterGroupId, o => o.MapFrom(s => Guid.Parse(s.CommonIpFilterGroupId)))
                    .ForMember(d => d.CommonIpFilterGroupName, o => o.MapFrom(s => s.CommonIpFilterGroupName))
                    .ReverseMap();
                cfg.CreateMap<RegisterCommonIpFilterGroupViewModel, CommonIpFilterGroupModel>()
                    .ForMember(d => d.CommonIpFilterGroupId, o => o.MapFrom(s => Guid.NewGuid().ToString()))
                    .ForMember(d => d.CommonIpFilterGroupName, o => o.MapFrom(s => s.CommonIpFilterGroupName))
                    .ForMember(d => d.IpList, o => o.MapFrom(s => s.IpList));
                cfg.CreateMap<RegisterCommonIpFilterViewModel, CommonIpFilterModel>()
                    .ForMember(d => d.CommonIpFilterId, o => o.MapFrom(s => Guid.NewGuid().ToString()))
                    .ForMember(d => d.IpAddress, o => o.MapFrom(s => s.IpAddress))
                    .ForMember(d => d.IsEnable, o => o.MapFrom(s => s.IsEnable));
            }).CreateMapper();
        });
        private static IMapper Mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IAuthenticationService> _lazyCommonIpFilterGroupService = new(() => UnityCore.Resolve<IAuthenticationService>());

        private IAuthenticationService AuthenticationService { get => _lazyCommonIpFilterGroupService.Value; }

        /// <summary>
        /// IPフィルタグループを登録します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_040", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("7D21CF87-2A88-4EA3-B170-035F974C5280")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<CommonIpFilterGroupIpListViewModel> RegisterCommonIpFilterGroup(RegisterCommonIpFilterGroupViewModel model)
        {
            IsValidDuplicateIpAddress(model.IpList.Select(x => x.IpAddress).ToList());
            var result = AuthenticationService.RegistrationCommonIpFilterGroup(Mapper.Map<CommonIpFilterGroupModel>(model));
            return Created(string.Empty, Mapper.Map<CommonIpFilterGroupIpListViewModel>(result));
        }

        /// <summary>
        /// IPフィルタグループを更新します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_040", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("91CC0209-27C9-4162-9973-52611A652AE4")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<CommonIpFilterGroupIpListViewModel> UpdateCommonIpFilterGroup(CommonIpFilterGroupIpListViewModel model)
        {
            IsValidDuplicateIpAddress(model.IpList.Select(x => x.IpAddress).ToList());
            var result = AuthenticationService.UpdateCommonIpFilterGroup(Mapper.Map<CommonIpFilterGroupModel>(model));
            return Created(string.Empty, Mapper.Map<CommonIpFilterGroupIpListViewModel>(result));
        }

        /// <summary>
        /// IPフィルタグループを削除します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_040", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("AF6C9724-0971-4CB1-A38B-8528F0413C21")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteCommonIpFilterGroup([RequiredGuid] string commonIpFilterGroupId)
        {
            AuthenticationService.DeleteCommonIpFilterGroup(commonIpFilterGroupId);
            return NoContent();
        }

        /// <summary>
        /// IPフィルタグループを取得します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_040", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("6C8FC784-26BB-4E46-BDED-1C99641E957D")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<CommonIpFilterGroupIpListViewModel> GetCommonIpFilterGroup([RequiredGuid] string commonIpFilterGroupId)
        {
            return Ok(Mapper.Map<CommonIpFilterGroupIpListViewModel>(AuthenticationService.GetCommonIpFilterGroup(commonIpFilterGroupId)));
        }

        /// <summary>
        /// IPフィルタグループのリストを取得します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_040", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("89DB4E25-D9AF-481D-B403-43F4DFFDB2B8")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<List<CommonIpFilterGroupInfoViewModel>> GetCommonIpFilterGroupList()
        {
            return Ok(Mapper.Map<List<CommonIpFilterGroupInfoViewModel>>(AuthenticationService.GetCommonIpFilterGroupList()));
        }

        /// <summary>
        /// IPフィルタグループのリストを取得します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_040", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("8B9D7A57-C462-49E7-AD57-20BF941E64BA")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<List<CommonIpFilterGroupIpListViewModel>> GetCommonIpFilterGroupListWithIpAddress()
        {
            return Ok(Mapper.Map<List<CommonIpFilterGroupIpListViewModel>>(AuthenticationService.GetCommonIpFilterGroupListWithIpAddress()));
        }

        private void IsValidDuplicateIpAddress(List<string> ipAddressList)
        {
            if(ipAddressList.Count == 0)
            {
                return;
            }
            // 重複しているIPを抽出
            var duplicateList = ipAddressList.GroupBy(ip => ip)
                .Where(group => group.Count() >= 2).ToList();

            if(duplicateList.Count > 0)
            {
                string displayIpAddresses = string.Join(",", duplicateList.Select(x => x.Key));
                throw new AlreadyExistsException($@"IPアドレスが重複しています。:{displayIpAddresses}");
            }
            return;
        }
    }
}