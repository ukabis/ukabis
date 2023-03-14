using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.ManageApi.Core.DataContainer;
using JP.DataHub.ManageApi.Models.System;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Validators;
using JP.DataHub.ManageApi.Attributes;

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("A5430B5C-1FE2-42FB-A98A-B46FF014BF00")]
    [UserRoleCheckController("DI_012")]
    public class SystemController : AbstractController
    {
        private Lazy<ISystemService> _lazySystemService = new(() => UnityCore.Resolve<ISystemService>());
        private ISystemService _systemService { get => _lazySystemService.Value; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterSystemViewModel, SystemModel>();
                cfg.CreateMap<RegisterClientViewModel, ClientModel>();
                cfg.CreateMap<RegisterSystemLinkViewModel, SystemLinkModel>();
                cfg.CreateMap<SystemModel, RegisterSystemResultViewModel>();
                cfg.CreateMap<SystemModel, UpdateSystemViewModel>().ReverseMap();
                cfg.CreateMap<SystemModel, SystemViewModel>().ReverseMap();
                cfg.CreateMap<SystemLinkViewModel, SystemLinkModel>().ReverseMap();
                cfg.CreateMap<RegisterSystemLinkViewModel, SystemLinkModel>().ReverseMap();
                cfg.CreateMap<SystemAdminViewModel, SystemAdminModel>().ReverseMap();
                cfg.CreateMap<ClientViewModel,ClientModel>().ReverseMap()
                    .ForMember(dst => dst.AccessTokenExpirationTimeSpan, ops => ops.MapFrom(src => src.AccessTokenExpirationTimeSpan.Days > 0 ?
                     "24:00" : // DBの登録値が24時間以上(1日以上)の場合は24:00とする
                     src.AccessTokenExpirationTimeSpan.ToString(@"hh\:mm")));

                cfg.CreateMap<RegisterClientModel ,RegisterClientViewModel >().ReverseMap()
                    .ForMember(dst => dst.AccessTokenExpirationTimeSpan, ops => ops.MapFrom(src => src.AccessTokenExpirationTimeSpan == "24:00" ?
                     new TimeSpan(1, 0, 0, 0) : // 24:00の場合(そのままParseするとエラーになるため)
                    TimeSpan.Parse(src.AccessTokenExpirationTimeSpan)));

                cfg.CreateMap<ClientModel, RegisterClientViewModel>().ReverseMap()
                    .ForMember(dst => dst.AccessTokenExpirationTimeSpan, ops => ops.MapFrom(src => src.AccessTokenExpirationTimeSpan == "24:00" ?
                     new TimeSpan(1, 0, 0, 0) : // 24:00の場合(そのままParseするとエラーになるため)
                    TimeSpan.Parse(src.AccessTokenExpirationTimeSpan)));

                cfg.CreateMap<UpdateClientModel ,UpdateClientViewModel>().ReverseMap()
                   .ForMember(dst => dst.AccessTokenExpirationTimeSpan, ops => ops.MapFrom(src => src.AccessTokenExpirationTimeSpan == "24:00" ?
                    new TimeSpan(1, 0, 0, 0) : // 24:00の場合(そのままParseするとエラーになるため)
                    TimeSpan.Parse(src.AccessTokenExpirationTimeSpan)));

            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        /// <summary>
        /// システムの登録
        /// </summary>
        /// <param name="registerViewModel">システム情報</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("4A47EDDF-105E-494B-8353-56E15AD522AA")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(DomainException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<RegisterSystemResultViewModel> RegisterSystem(RegisterSystemViewModel model)
            => Created(string.Empty, s_mapper.Map<RegisterSystemResultViewModel>(_systemService.RegisterSystem(s_mapper.Map<SystemModel>(model), SystemItem.All)));

        /// <summary>
        /// システムの更新
        /// </summary>
        /// <param name="updateSystemViewModel">システム情報</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("37973952-175D-4380-9E44-56FBBA4668B0")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(DomainException), HttpStatusCode.BadRequest, typeof(AlreadyExistsException), HttpStatusCode.BadRequest)]
        public ActionResult<UpdateSystemViewModel> UpdateSystem(UpdateSystemViewModel model)
            => Created(string.Empty, s_mapper.Map<UpdateSystemViewModel>(_systemService.UpdateSystem(s_mapper.Map<SystemModel>(model), SystemItem.Head)));

        /// <summary>
        /// システムの削除
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("97172747-6BC9-42D2-A0D7-1D74733625E8")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteSystem([RequiredGuid] string systemId)
        {
            _systemService.DeleteSystem(systemId);

            return NoContent();
        }

        /// <summary>
        /// システムの取得
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="includesFunctions">Functionsを含める場合はtrueを指定する(デフォルトfalse)</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("EF4F8BB1-1D31-427A-B68D-68697E6A9591")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        // 本来ならSystemViewModelを指定したいがなぜかswaggerが表示しなくなる。swaggerの定義にリターン型の情報が出なくなってしまうが
        // とりあえずの暫定対処として、違うモデルを指定してある
        public ActionResult<SystemTestViewModel> GetSystem([RequiredGuid] string systemId, bool includeClient = false, bool includeLink = false, bool includeAdmin = false)
        {
            SystemItem item = SystemItem.Head;
            if (includeClient) item |= SystemItem.Client;
            if (includeLink) item |= SystemItem.Link;
            if (includeAdmin) item |= SystemItem.Admin;
            return Ok(_systemService.GetSystem(systemId, item));
        }

        /// <summary>
        /// システム一覧の取得
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("D96A7690-18DF-40EC-8FC3-7250500D66AF")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<SystemTestViewModel> GetList([RequiredGuid] string vendorId)
            => Ok(s_mapper.Map<IList<SystemViewModel>>(_systemService.GetSystemListByVendorId(vendorId)));

        /// <summary>
        /// システムのAdmin認証を取得する
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("D69ADF3B-7FCC-4475-8C1F-5120379F9D35")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<SystemAdminViewModel> GetSystemAdmin([RequiredGuid] string systemId)
            => Ok(s_mapper.Map<SystemAdminViewModel>(_systemService.GetAdminBySystemId(systemId)));

        /// <summary>
        /// システムのAdmin認証を登録する
        /// </summary>
        /// <param name="systemAdminViewModel">Admin認証</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("6685B379-AE4C-4DF2-B7FE-C20A93FBFC9D")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult RegisterSystemAdmin(SystemAdminViewModel model)
            => Created(string.Empty, _systemService.RegisterAdmin(s_mapper.Map<SystemAdminModel>(model)));

        /// <summary>
        /// システムのAdmin認証を削除する
        /// </summary>
        /// <param name="systemId">システムID</param>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("5BB73E35-C1DA-4824-8D51-C7C6E258B03F")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteSystemAdmin([RequiredGuid] string systemId)
        {
            _systemService.DeleteAdminBySystemId(systemId);
            return NoContent();
        }

        /// <summary>
        /// システムリンク一覧取得
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("4EBD8478-B4D5-4D0C-AEC3-CE8E14BCD2CE")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<SystemLinkViewModel>> GetSystemLinkList([RequiredGuid] string systemId)
            => Ok(s_mapper.Map<IList<SystemLinkViewModel>>(_systemService.GetSystemLinkListBySystemId(systemId)));

        /// <summary>
        /// システムリンク取得
        /// </summary>
        /// <param name="systemLinkId">システムリンクID</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Read)]
        [HttpGet]
        [Admin]
        [ManageAction("E50CE151-DEE6-46C6-8789-0CA85A302896")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<SystemLinkViewModel> GetSystemLink([RequiredGuid] string systemLinkId)
            => Ok(s_mapper.Map<SystemLinkViewModel>(_systemService.GetSystemLink(systemLinkId)));

        /// <summary>
        /// システムリンクの登録及び更新
        /// </summary>
        /// <param name="upsertSystemLinkViewModel">システムリンク情報</param>
        /// <param name="systemId">システムID</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("E46C3110-0908-47C8-AADD-A55BFC1C50C6")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<SystemLinkViewModel>> RegisterSystemLink(IList<RegisterSystemLinkViewModel> model)
        {
            var result = _systemService.UpsertSystemLink(s_mapper.Map<List<SystemLinkModel>>(model));
            return Created(string.Empty, s_mapper.Map<List<SystemLinkViewModel>>(result));
        }

        /// <summary>
        /// システムリンクを削除する
        /// </summary>
        /// <param name="systemLinkId">システムリンクID</param>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("E1CFDADD-4A38-457F-AA78-C6D649B11555")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteSystemLink([RequiredGuid] string systemLinkId)
        {
            _systemService.DeleteSystemLink(systemLinkId);
            return NoContent();
        }

        /// <summary>
        /// クライアント認証一覧を取得する
        /// </summary>
        /// <remarks>クライアントシークレットは暗号化されているため取得しない</remarks>
        /// <param name="systemId">システムID</param>
        /// <returns>クライアント認証一覧</returns>
        [UserRole("DI_012", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("9F63C0F5-ADC7-47FA-B8B4-017E3ABA977E")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<ClientViewModel>> GetClientList([RequiredGuid] string systemId)
            => Ok(s_mapper.Map<List<ClientViewModel>>(_systemService.GetClientListBySystemId(systemId)));

        /// <summary>
        /// クライアント認証を取得する
        /// </summary>
        /// <remarks>クライアントシークレットは暗号化されているため取得しない</remarks>
        /// <param name="clientId">クライアントID</param>
        /// <returns>クライアント認証</returns>
        [UserRole("DI_012", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("B64518D9-6BC1-45A0-9C61-F7CBE870EB21")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ClientViewModel> GetClient([RequiredGuid] string clientId)
            => Ok(s_mapper.Map<ClientViewModel>(_systemService.GetClient(clientId)));

        /// <summary>
        /// クライアント認証を登録する
        /// </summary>
        /// <param name="registerClientModel">クライアント認証情報</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("ECEB38F4-8560-47C3-8F79-3E8175A18ED4")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ClientViewModel> RegisterClient(RegisterClientViewModel model)
            => Created(string.Empty, _systemService.RegisterClient(s_mapper.Map<RegisterClientModel>(model)));

        /// <summary>
        /// クライアント認証を更新する
        /// </summary>
        /// <param name="updateClientModel">クライアント認証情報</param>
        /// <returns></returns>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("2D843EE5-A3B8-4958-A856-03DE70E24152")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ClientViewModel> UpdateClient(UpdateClientViewModel model)
            => Created(string.Empty, s_mapper.Map<ClientViewModel>(_systemService.UpdateClient(s_mapper.Map<UpdateClientModel>(model))));

        /// <summary>
        /// クライアント認証を削除する
        /// </summary>
        /// <param name="clientId">クライアントID</param>
        [UserRole("DI_012", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("E6EB3D13-13DE-47DF-9418-B1A5D06606A1")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult DeleteClient([RequiredGuid] string clientId)
        {
            _systemService.DeleteClient(clientId);
            return NoContent();
        }
    }
}