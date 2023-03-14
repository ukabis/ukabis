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
using JP.DataHub.ManageApi.Models.Vendor;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Attributes;

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("700D2EC9-7EF0-4C30-B2EF-FA05A49358AF")]
    [UserRoleCheckController("DI_011")]
    public class VendorController : AbstractController
    {
        private Lazy<IVendorService> _lazyVendorService = new(() => UnityCore.Resolve<IVendorService>());
        private IVendorService _vendorService { get => _lazyVendorService.Value; }

        private Lazy<IUserInvitationService> _lazyUserInvitationService = new(() => UnityCore.Resolve<IUserInvitationService>());
        private IUserInvitationService _userInvitationService { get => _lazyUserInvitationService.Value; }

        private Lazy<IAttachFileService> _lazyAttachFileService = new(() => UnityCore.Resolve<IAttachFileService>());
        private IAttachFileService _attachFileService { get => _lazyAttachFileService.Value; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VendorModel, VendorViewModel>();
                cfg.CreateMap<VendorModel, VendorSimpleViewModel>();
                cfg.CreateMap<OpenIdCaModel, OpenIdCaViewModel>()
                    .ForMember(dst => dst.VendorOpenidCaId, ops => ops.MapFrom(src => src.Id));
                cfg.CreateMap<VendorModel, VendorOnlyViewModel>();
                cfg.CreateMap<VendorLinkModel, VendorLinkViewModel>();
                cfg.CreateMap<VendorOpenIdCaModel, OpenIdCaViewModel>()
                    .ForMember(dst => dst.VendorOpenidCaId, ops => ops.MapFrom(src => src.Id))
                    .ForMember(dst => dst.VendorId, ops => ops.MapFrom(src => src.VendorId))
                    .ForMember(dst => dst.ApplicationId, ops => ops.MapFrom(src => src.ApplicationId))
                    .ForMember(dst => dst.ApplicationName, ops => ops.MapFrom(src => src.ApplicationName))
                    .ForMember(dst => dst.IsActive, ops => ops.MapFrom(src => src.IsActive))
                    .ForMember(dst => dst.AccessControl, ops => ops.MapFrom(src => src.AccessControl));
                cfg.CreateMap<VendorSystemModel, VendorSystemViewModel>();
                cfg.CreateMap<StaffRoleModel, StaffViewModel>();
                cfg.CreateMap<AddStafforViewModel, StaffModel>();
                cfg.CreateMap<UpdateVendorViewModel, VendorModel>();
                cfg.CreateMap<VendorModel, RegisterVendorResultViewModel>();
                cfg.CreateMap<RegisterVendorViewModel, VendorModel>();
                cfg.CreateMap<VendorModel, UpdateVendorViewModel>();
                cfg.CreateMap<StaffModel, AddStafforResultViewModel>();
                cfg.CreateMap<UpdateStaffViewModel, StaffModel>().ReverseMap();
                cfg.CreateMap<RegisterVendorLinkViewModel, RegisterVendorLinkModel>().ReverseMap();
                cfg.CreateMap<RegisterOpenIdCaViewModel, RegisterOpenIdCaModel>().ReverseMap();
                cfg.CreateMap<SendInvitationViewModel, SendUserInvitationModel>()
                    .ForMember(dst => dst.VendorId, ops => ops.MapFrom(src => src.VendorId))
                    .ForMember(dst => dst.RoleId, ops => ops.MapFrom(src => src.RoleId))
                    .ForMember(dst => dst.MailAddress, ops => ops.MapFrom(src => src.MailAddress))
                    .AfterMap((src, dst) => UpdateOpenId(dst));
                cfg.CreateMap<AttachFileStorageModel, AttachFileStorageViewModel>();
            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private static void UpdateOpenId(SendUserInvitationModel model)
        {
            var dataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            model.RegistAccount = dataContainer.OpenId;
        }

        /// <summary>
        /// ベンダー、システム、スタッフの一覧を取得する
        /// </summary>
        /// <returns>ベンダー・システム・スタッフ情報</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("A92A642E-BF7B-40B9-90CB-B7F28257FA3C")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<VendorViewModel>> GetList()
            => Ok(s_mapper.Map<IList<VendorViewModel>>(_vendorService.GetVendorList()));

        /// <summary>
        /// ベンダー、システムの一覧を取得する
        /// </summary>
        /// <returns>ベンダー・システム情報</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("7D26F474-1C54-4F85-9D49-BB9A9EC02B4F")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<VendorSimpleViewModel>> GetVendorSimpleList(string vendorId = null)
            => Ok(s_mapper.Map<IList<VendorSimpleViewModel>>(_vendorService.GetEnableVendorSystemNameList(vendorId)));

        /// <summary>
        /// ベンダーの登録
        /// </summary>
        /// <param name="registerViewModel">ベンダー情報</param>
        /// <returns></returns>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("DBF4601A-B670-43A0-B192-F1EE50BC9276")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest)]
        public ActionResult<RegisterVendorResultViewModel> Register(RegisterVendorViewModel registerViewModel)
         => Created(string.Empty, s_mapper.Map<RegisterVendorResultViewModel>(_vendorService.Register(s_mapper.Map<VendorModel>(registerViewModel))));

        /// <summary>
        /// ベンダーAttacFileの登録
        /// </summary>
        /// <param name="registerAttachFileViewModel">ベンダー情報</param>
        /// <returns></returns>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("29FF8380-C23C-4596-95E0-3FA21BD22BEE")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest)]
        public ActionResult<RegisterVendorAttachFileViewModel> RegisterVendorAttachFile(RegisterVendorAttachFileViewModel registerAttachFileViewModel)
        {
            _attachFileService.RegisterVendorAttachFileStorage
                (registerAttachFileViewModel.VendorId.ToString(), registerAttachFileViewModel.AttachFileStorageId.ToString(), true);
            return Created(string.Empty, null);
        }



        /// <summary>
        /// ベンダーの更新
        /// </summary>
        /// <param name="updateVendorViewModel">ベンダー情報</param>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("61E0616B-BFB8-4DB3-8448-8A6EE55ACCA3")]
        [ExceptionFilter(typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(NotFoundException), HttpStatusCode.BadRequest)]
        public ActionResult Update(UpdateVendorViewModel model)
            => Created(string.Empty, s_mapper.Map<UpdateVendorViewModel>(_vendorService.Update(s_mapper.Map<VendorModel>(model))));

        /// <summary>
        /// ベンダーの削除
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("52D0DD50-A74F-46F8-97FA-61A7C9634C5F")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult Delete([RequiredGuid] string vendorId)
        {
            _vendorService.Delete(vendorId);
            return NoContent();
        }

        /// <summary>
        /// ベンダーの取得
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("80F42BD9-5C39-4A0E-9473-82F648FED287")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<VendorViewModel> GetVendor([RequiredGuid]string vendorId)
            => Ok(s_mapper.Map<VendorViewModel>(_vendorService.Get(vendorId)));

        /// <summary>
        /// OpenIdに紐づくベンダーの取得
        /// </summary>
        /// <param name="openId">OpenId</param>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("9A2E34D7-C154-4878-BD27-822DB1249451")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<VendorOnlyViewModel> GetVendorByOpenId([RequiredGuid]string openId)
            => Ok(s_mapper.Map<VendorOnlyViewModel>(_vendorService.GetByOpenId(openId)));

        /// <summary>
        /// ベンダー情報の更新（ベンダースタッフの追加）
        /// </summary>
        /// <param name="addStafforViewModel">ベンダーに追加するスタッフ情報</param>
        /// <returns></returns>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("95CB5437-E497-41F6-9BDC-B849ACEBEF7A")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(InvalidAccountFormat), HttpStatusCode.BadRequest)]
        public ActionResult<AddStafforResultViewModel> AddStaff(AddStafforViewModel model)
            => Created(string.Empty, s_mapper.Map<AddStafforResultViewModel>(_vendorService.AddStaff(s_mapper.Map<StaffModel>(model))));

        /// <summary>
        /// ベンダースタッフの更新
        /// </summary>
        /// <param name="updateStaffViewModel">スタッフ情報</param>
        /// <returns></returns>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("9474660E-4097-43EB-BA51-0CAE881AF0D4")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(AlreadyExistsException), HttpStatusCode.BadRequest, typeof(InvalidAccountFormat), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<UpdateStaffViewModel> UpdateStaff(UpdateStaffViewModel model)
            => Created(string.Empty, s_mapper.Map<UpdateStaffViewModel>(_vendorService.UpdateStaff(s_mapper.Map<StaffModel>(model))));

        /// <summary>
        /// ベンダースタッフの削除
        /// </summary>
        /// <param name="staffId">スタッフID</param>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("A9B41920-9ACF-4463-94DD-7CFDEEAB9B54")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteStaff([RequiredGuid]string staffId)
        {
            _vendorService.DeleteStaff(staffId);
            return NoContent();
        }

        /// <summary>
        /// スタッフアカウント（OpenIDのアカウント）が存在しているかを確認する※GETバージョン
        /// </summary>
        /// <param name="existsStafforViewModel">スタッフアカウント（OpenIDのアカウント）</param>
        /// <returns>存在している場合は所属しているベンダーID。存在しない場合はNotFound</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("D2484F51-F258-449F-8690-FCB42225CE26")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ExistsStaffResultViewModel> ExistsStaffAccount([RequiredGuid] string account)
            => Ok(new { VendorId = _vendorService.GetVendorIdByStaffAccount(account), Account = account });

        /// <summary>
        /// スタッフアカウント（OpenIDのアカウント）が存在しているかを確認する※POSTバージョン
        /// </summary>
        /// <param name="existsStafforViewModel">スタッフアカウント（OpenIDのアカウント）</param>
        /// <returns>存在している場合は所属しているベンダーID。存在しない場合はNotFound</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpPost]
        [ManageAction("D2484F51-F258-449F-8690-FCB42225CE25")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<ExistsStaffResultViewModel> ExistsStaffAccount(ExistsStaffViewModel model)
            => Ok(new { VendorId = _vendorService.GetVendorIdByStaffAccount(model.StaffAccount), StaffAccount = model.StaffAccount });

        /// <summary>
        /// スタッフ一覧を取得する
        /// ベンダーIDを指定した場合はベンダーに所属する一覧を
        /// ベンダーIDを指定しない場合は全スタッフを返す
        /// </summary>
        /// <returns>スタッフ一覧</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("A95A4CED-F915-4C3E-AEBE-4F6E142F48C7")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<StaffViewModel>> GetStaffList([ValidateGuid] string vendorId)
            => Ok(_vendorService.GetStaffListByVendorId(vendorId));

        /// <summary>
        /// 指定したスタッフを取得する
        /// </summary>
        /// <param name="staffId">スタッフID</param>
        /// <returns>スタッフ</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("D2AA6F37-93FA-424A-95CF-600535E8CAD7")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<StaffViewModel> GetStaff([RequiredGuid]string staffId)
            => Ok(_vendorService.GetStaff(staffId));

        /// <summary>
        /// ベンダーリンクの登録及び更新をする
        /// </summary>
        /// <param name="model">ベンダーリンク情報</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns></returns>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("FC81C446-508F-43C0-9196-3DD4BAC427EF")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<VendorLinkViewModel>> RegisterVendorLink(IList<RegisterVendorLinkViewModel> model)
            => Created(string.Empty, s_mapper.Map<IList<VendorLinkViewModel>>(_vendorService.RegisterVendorLink(s_mapper.Map<IList<RegisterVendorLinkModel>>(model))));

        /// <summary>
        /// ベンダーリンクを削除する
        /// </summary>
        /// <param name="vendorLinkId">ベンダーリンクID</param>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("E03FA0E2-A0DC-4624-8E66-233651CD5157")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteVendorLink([RequiredGuid] string vendorLinkId)
        {
            _vendorService.DeleteVendorLink(vendorLinkId);
            return NoContent();
        }

        /// <summary>
        /// 指定したベンダーのリンク一覧を取得する
        /// ベンダーIDを指定しない場合は全ベンダーのを返す
        /// </summary>
        /// <param name="vendorId">vendorId</param>
        /// <returns>リンク一覧</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("4B0CF0B7-76AA-4C43-86C7-5FC5EA43D697")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<VendorLinkViewModel>> GetVendorLinkList([RequiredGuid] string vendorId)
            => Ok(s_mapper.Map<IList<VendorLinkViewModel>>(_vendorService.GetVendorLinkList(vendorId)));

        /// <summary>
        /// 指定したベンダーのリンクを取得する
        /// </summary>
        /// <param name="vendorLinkId">vendorLinkId</param>
        /// <returns>リンク</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("91BFBF40-2838-4699-9B66-76EBCA4F8313")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<VendorLinkViewModel> GetVendorLink([RequiredGuid]string vendorLinkId)
            => Ok(s_mapper.Map<VendorLinkViewModel>(_vendorService.GetVendorLink(vendorLinkId)));

        /// <summary>
        /// OpenID認証局リストを取得する
        /// OpenIdCertificationAuthorityテーブルに登録されているものを全て表示し、VendorOpenIdCAテーブルに未登録のものはIDがNULL
        /// </summary>
        /// <param name="vendorId">vendorId</param>
        /// <returns>OpenID認証局リスト</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("C2DBF4BE-9BA5-4691-BDEA-70EE0B688BA9")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<OpenIdCaViewModel>> GetVendorOpenIdCa([RequiredGuid] string vendorId)
            => Ok(s_mapper.Map<List<OpenIdCaViewModel>>(_vendorService.GetVendorOpenIdCaList(vendorId)));

        /// <summary>
        /// OpenId認証局を登録する。
        /// </summary>
        /// <param name="regVMList">登録するOpenId認証局リスト</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>登録結果</returns>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("7C47C9D6-0ACF-423F-97E0-B3B9095FEE29")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(Rfc7807Exception), HttpStatusCode.BadRequest)]
        public ActionResult<IList<RegisterOpenIdCaViewModel>> RegisterVendorOpenIdCa(List<RegisterOpenIdCaViewModel> model)
            => Created(string.Empty, s_mapper.Map<List<RegisterOpenIdCaViewModel>>(_vendorService.RegisterVendorOpenIdCaList(s_mapper.Map<List<RegisterOpenIdCaModel>>(model))));

        /// <summary>
        /// OpenId認証局を削除する。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="vendorOpenidCaId">ベンダーOpenId認証局ID</param>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("46B8C65E-9CC3-4C95-AEE1-98DCF6986DC9")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteVendorOpenIdCa([RequiredGuid]string vendorId, [RequiredGuid] string vendorOpenidCaId)
        {
            _vendorService.DeleteVendorOpenIdCa(vendorId, vendorOpenidCaId);
            return NoContent();
        }

        /// <summary>
        /// 招待メールを送信する
        /// </summary>
        /// <param name="model">招待送信モデル</param>
        [UserRole("DI_013", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("201a3c9d-14b5-44c7-8335-0b5d50cdb087")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<InvitationIdViewModel> SendInvitation(SendInvitationViewModel model)
            => Created(string.Empty, new InvitationIdViewModel(_userInvitationService.RegisterUserInvitation(s_mapper.Map<SendUserInvitationModel>(model))));

        /// <summary>
        /// 招待されたユーザをStaffに登録する
        /// </summary>
        /// <param name="model">招待ユーザモデル</param>
        [UserRole("DI_013", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("4243B242-F914-45C7-A796-EC8E74B58F86")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(ExpirationException), HttpStatusCode.BadRequest, typeof(RegisteredException), HttpStatusCode.NoContent)]
        public ActionResult<VendorStaffViewModel> AddInvitedUser(AddInvitedUserViewModel model)
            => Created(string.Empty, _userInvitationService.RegistUserInvitationInfoToStaff(model.InvitationId, model.OpenId, model.MailAddress));

        /// <summary>
        /// ベンダーが利用できる添付ファイル一覧を取得する
        /// </summary>
        /// <returns>添付ファイル一覧</returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("089B478E-1664-4BC4-B9D8-B93654D82EAF")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<IList<AttachFileStorageViewModel>> GetAttachFileList()
            => Ok(s_mapper.Map<List<AttachFileStorageViewModel>>(_attachFileService.GetAttachFileStorageList()));

        /// <summary>
        /// ベンダーが利用している添付ファイルを取得する
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns></returns>
        [UserRole("DI_011", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("D1A3A0B7-C94A-462E-BE08-85D07B2B8E88")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult GetAttachFile([RequiredGuid] string vendorId)
            => Ok(new { AttachFileStorageId = _attachFileService.GetAttachFileStorageIdByVendorId(vendorId) });

        /// <summary>
        /// ベンダーが利用できる添付ファイルを登録する
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="attachFileId">添付ファイルID</param>
        /// <param name="isCurrent">登録する添付ファイルIDを有効にする場合はtrueを指定(デフォルトtrue)　登録する添付ファイルID以外の添付ファイルIDが登録済みの場合、登録済みの添付ファイルIDは無効になります</param>
        /// <returns></returns>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("10AFA8FA-E5A8-49D8-8C42-74B436897B57")]
        public ActionResult RegisterAttachFile([RequiredGuid] string vendorId, [RequiredGuid] string attachFileId, bool isCurrent = true)
        {
            _attachFileService.RegisterVendorAttachFileStorage(vendorId, attachFileId, isCurrent);
            return Created(string.Empty, new { attachFileId = attachFileId });
        }

        /// <summary>
        /// ベンダー利用できる添付ファイルを削除する
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        [UserRole("DI_011", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("E175D916-4ED5-40C0-9FF2-C97104B5F6BA")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(AccessDeniedException), HttpStatusCode.Forbidden)]
        public ActionResult DeleteAttachFile([RequiredGuid] string vendorId)
        {
            _attachFileService.DeleteByVendorId(vendorId);
            return NoContent();
        }
    }
}