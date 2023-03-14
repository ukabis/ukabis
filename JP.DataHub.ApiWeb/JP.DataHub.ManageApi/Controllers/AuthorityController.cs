using AutoMapper;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Models.Authority;
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

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("F91835A7-3C29-4F98-BD5D-EBA5636036F5")]
    public class AuthorityController : AbstractController
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoleDetailModel, RoleDetailViewModel>().ReverseMap();
                cfg.CreateMap<CommonIpFilterGroupInfoModel, CommonIpFilterGroupInfoViewModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });

        private static IMapper Mapper { get => s_lazyMapper.Value; }

        private Lazy<IAuthenticationService> _lazyAuthenticationService = new(() => UnityCore.Resolve<IAuthenticationService>());
        private IAuthenticationService RepositoryAuthenticationService { get => _lazyAuthenticationService.Value; }

        private static Lazy<IList<string>> s_lazyOperatingVendorVendorId = new Lazy<IList<string>>(() => UnityCore.Resolve<IList<string>>("OperatingVendorVendorId"));
        private static IList<string> s_operatingVendorVendorId { get => s_lazyOperatingVendorVendorId.Value; }

        /// <summary>
        /// 権限の一覧や詳細リストを取得する
        /// ただし権限一覧を返すのは以下の３つの条件に満たしている場合
        /// ・openidで指定された（自分でなくてもよい）人が有効であること
        /// ・openidの人が所属するベンダーが存在すること
        /// ・そのベンダーが有効であること
        /// ※openidが指定されていない場合は自分のOpenId（ヘッダーで指定したもの）を使って検証する
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("B140A2BE-2C1B-4C02-9F88-E233B3DCBC4A")]
        [UserRole("DI_041", UserRoleAccessType.Read)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public List<RoleDetailViewModel> GetRoleDetailEx(string openId)
        {
            if (string.IsNullOrEmpty(openId))
            {
                openId = PerRequestDataContainer.OpenId;
            }
            return Mapper.Map<List<RoleDetailViewModel>>(RepositoryAuthenticationService.GetRoleDetailEx(openId));
        }

        /// <summary>
        /// 権限の一覧や詳細リストを取得する
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("F72832A3-0075-4924-8BF1-B2E980D0BC87")]
        [UserRole("DI_041", UserRoleAccessType.Read)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public List<RoleDetailViewModel> GetRoleDetail()
        {
            return Mapper.Map<List<RoleDetailViewModel>>(RepositoryAuthenticationService.GetRoleDetail());
        }

        /// <summary>
        /// 指定したベンダーが運営会社ベンダーかどうかを取得する
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("467F9ACF-4F83-460A-8EF2-F75869F7530E")]
        [UserRole("DI_041", UserRoleAccessType.Read)]
        public ActionResult<bool> IsOperatingVendor(string vendorId)
        {
            return s_operatingVendorVendorId.Contains(vendorId.ToLower()) == true;
        }

        /// <summary>
        /// 許可IPリストを取得する
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("53D56A31-612B-4B69-9399-22573CC56C28")]
        [UserRole("DI_041", UserRoleAccessType.Read)]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(ArgumentNullException), HttpStatusCode.BadRequest)]
        public List<CommonIpFilterGroupInfoViewModel> GetCommonIPFilterList([FromQuery] string[] commonIpFilterGroupNames = null)
        {
            return Mapper.Map<List<CommonIpFilterGroupInfoViewModel>>(RepositoryAuthenticationService.GetCommonIPFilterList(commonIpFilterGroupNames.ToList()));
        }
    }
}