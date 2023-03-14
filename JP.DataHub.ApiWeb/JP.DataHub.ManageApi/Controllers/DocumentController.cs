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
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.ManageApi.Models.Document;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Attributes;

namespace JP.DataHub.ManageApi.Controllers
{
    /// <summary>
    /// 文書管理のためのAPIを提供します
    /// </summary>
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("F6CFC121-E8B6-4CCE-9DE2-394E47D6862B")]
    public class DocumentController : AbstractController
    {
        private static Lazy<string> s_lazyPortalUri = new Lazy<string>(() => s_appConfig.GetValue<string>("PortalUri"));
        private static string s_PortalUri { get => s_lazyPortalUri.Value; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DocumentInformationModel, DocumentViewModel>();
                cfg.CreateMap<DocumentModel, DocumentWithoutFileViewModel>();
                cfg.CreateMap<FileModel, FileViewModel>();
                cfg.CreateMap<AgreementModel, DocumentAgreementViewModel>();
                cfg.CreateMap<DocumentCategoryModel, DocumentCategoryViewModel>();
                cfg.CreateMap<RegisterDocumentViewModel, DocumentModel>();
                cfg.CreateMap<UpdateDocumentViewModel, DocumentModel>();
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get => s_lazyMapper.Value; }

        private Lazy<IDocumentService> _lazyDocumentService = new(() => UnityCore.Resolve<IDocumentService>());
        private IDocumentService _documentService { get => _lazyDocumentService.Value; }
        private Lazy<IVendorService> _lazyVendorService = new(() => UnityCore.Resolve<IVendorService>());
        private IVendorService _vendorService { get => _lazyVendorService.Value; }
        private Lazy<IAgreementService> _lazyAgreementService = new(() => UnityCore.Resolve<IAgreementService>());
        private IAgreementService _agreementService { get => _lazyAgreementService.Value; }

        private readonly string publicDestinationIsPortal = "portal";
        private readonly string publicDestinationIsAdmin = "admin";
        private readonly string publicDestinationIsAll = "all";

        /// <summary>
        /// ドキュメントのリストを取得します。（ポータル用）
        /// </summary>
        /// <param name="publicDestination">公開先　"portal"：ポータル / "admin"：管理画面 / "all"：全ドキュメント　※指定無はポータル</param>
        /// <returns></returns>
        [UserRole("DI_039", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("05F392F1-D724-47C0-A972-9BF57282AC51")]
        public ActionResult<List<DocumentViewModel>> GetList(string areaToPublic = null)
        {
            // 公開先指定のパラメータを設定
            bool isPublicPortal = false;
            bool isPublicAdmin = false;

            // 公開先指定(パラメータ)無し＝公開先＝ポータルと同等
            if (string.IsNullOrEmpty(areaToPublic))
            {
                isPublicPortal = true;
            }
            // 公開先指定(パラメータ)有り
            else
            {
                // 公開先＝ポータル指定
                if (publicDestinationIsPortal.Equals(areaToPublic.ToLower()))
                {
                    isPublicPortal = true;
                }
                // 公開先＝管理画面指定
                else if (publicDestinationIsAdmin.Equals(areaToPublic.ToLower()))
                {
                    isPublicAdmin = true;
                }
                // 公開先＝全て指定
                else if (publicDestinationIsAll.Equals(areaToPublic.ToLower()))
                {
                    isPublicPortal = true;
                    isPublicAdmin = true;
                }
                // 公開先指定誤り
                else
                {
                    // BadRequestの理由を設定
                    return BadRequest(new { Message = $"AreaToPublic={areaToPublic} Specification Error" });
                }
            }

            // 公開先指定を取得条件に追加
            var documentViewModelList = s_mapper.Map<List<DocumentViewModel>>(_documentService.GetDocumentInformation(isPublicPortal, isPublicAdmin));
            IList<VendorModel> vendors = null;
            if (documentViewModelList.Count > 0)
            {
                vendors = _vendorService.GetEnableVendorSystemNameList();
            }

            List<DocumentViewModel> result = new List<DocumentViewModel>();
            foreach (var documentViewModel in documentViewModelList)
            {
                var vendor = vendors.FirstOrDefault(x => x.VendorId == documentViewModel.VendorId);
                documentViewModel.VendorName = vendor?.VendorName;
                documentViewModel.SystemName = vendor?.SystemList.FirstOrDefault(x => x.SystemId == documentViewModel.SystemId)?.SystemName;
                result.Add(documentViewModel);
            }
            return result;
        }

        /// <summary>
        /// ドキュメント規約のリストを取得します。（ポータル用）
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_046", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("78B15852-A367-44EF-9C0D-410D3CBF0F71")]
        public ActionResult<List<DocumentAgreementViewModel>> GetAgreementList()
            => s_mapper.Map<List<DocumentAgreementViewModel>>(_agreementService.GetAgreementList());

        /// <summary>
        /// ドキュメントカテゴリのリストを取得します。（ポータル用）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("7FC97AAE-5F5A-4B21-808F-C4CC32D2A9AD")]
        public ActionResult<List<DocumentCategoryViewModel>> GetCategoryList()
            => s_mapper.Map<List<DocumentCategoryViewModel>>(_documentService.GetDocumentCategory());

        /// <summary>
        /// ドキュメント情報を取得します
        /// </summary>        
        /// <returns></returns>
        [UserRole("DI_039", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("35AEAE57-CCEA-42EB-9A3A-06799DF13C68")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<DocumentViewModel> GetDocument([RequiredGuid] string documentId)
        { 
            return Ok(s_mapper.Map<DocumentViewModel>(_documentService.GetDocumentInformation(documentId)));
        }
        /// <summary>
        /// 指定されえたベンダーが持っているドキュメントのリストを取得します
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_039", UserRoleAccessType.Read)]
        [HttpGet]
        [ManageAction("820C9B7F-14B2-40DE-B87F-69A15F1ECF9C")]
        public ActionResult<IList<DocumentWithoutFileViewModel>> GetDocumentList([ValidateGuid]string? vendorId)
        {
            var result = s_mapper.Map<List<DocumentWithoutFileViewModel>>(_documentService.GetDocumentList(vendorId));
            return result?.Count != 0 ? Ok(result) : NotFound();
        }

        /// <summary>
        /// ドキュメントを登録します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_045", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("A2464AE8-098E-46C2-936D-F521A6A6B99D")]
        [ExceptionFilter(typeof(ForeignKeyException), HttpStatusCode.BadRequest, typeof(NotFoundException), HttpStatusCode.BadRequest)]
        public ActionResult<DocumentWithoutFileViewModel> RegisterDocument(RegisterDocumentViewModel model)
        {
            var documentModel = s_mapper.Map<DocumentModel>(model);
            documentModel.IsActive = true; // 登録なのでActiveにする
            //公開設定の値をセット
            SetPublicPortalStatus(model.IsPublicPortalStatus, documentModel);
            SetPublicAdminStatus(model.IsPublicAdminStatus, documentModel);
            var result = _documentService.Register(s_mapper.Map<DocumentModel>(documentModel));
            return Created(string.Empty, s_mapper.Map<DocumentWithoutFileViewModel>(result));
        }

        /// <summary>
        /// ドキュメントを更新します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_045", UserRoleAccessType.Write)]
        [HttpPost]
        [ManageAction("ED96EDD2-4F13-41A4-8F73-79E89144B693")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.BadRequest, typeof(DBConcurrencyException), HttpStatusCode.BadRequest, typeof(ForeignKeyException), HttpStatusCode.BadRequest)]
        public ActionResult<DocumentWithoutFileViewModel> UpdateDocument(UpdateDocumentViewModel model)
        {
            var documentModel = s_mapper.Map<DocumentModel>(model);
            //公開設定の値をセット
            SetPublicPortalStatus(model.IsPublicPortalStatus, documentModel);
            SetPublicAdminStatus(model.IsPublicAdminStatus, documentModel);
            documentModel.IsActive = true; //Active
            return Created(string.Empty, s_mapper.Map<DocumentWithoutFileViewModel>(_documentService.Update(documentModel)));
        }

        /// <summary>
        /// ドキュメントを削除します。
        /// </summary>
        /// <returns></returns>
        [UserRole("DI_045", UserRoleAccessType.Write)]
        [HttpDelete]
        [ManageAction("8C870D05-C6BC-4707-AA21-44502742E67C")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound, typeof(MultipleUpdateException), HttpStatusCode.BadRequest)]
        public ActionResult DeleteDocument([RequiredGuid] string documentId)
        {
            _documentService.DeleteDocument(documentId);
            return NoContent();
        }

        private void SetPublicPortalStatus(PublicStatus value, DocumentModel model)
        {
            //PortalUriが設定されていない場合（Portalが無い場合）は強制的にNoneにする
            if (string.IsNullOrEmpty(s_PortalUri))
            {
                value = PublicStatus.None;
            }

            switch (value)
            {
                case PublicStatus.None:
                    model.IsPublicPortal = false;
                    model.IsPublicPortalHidden = false;
                    break;
                case PublicStatus.Public:
                    model.IsPublicPortal = true;
                    model.IsPublicPortalHidden = false;
                    break;
                case PublicStatus.Hidden:
                    model.IsPublicPortal = true;
                    model.IsPublicPortalHidden = true;
                    break;
            }
        }

        private void SetPublicAdminStatus(PublicStatus value, DocumentModel model)
        {
            switch (value)
            {
                case PublicStatus.None:
                    model.IsPublicAdmin = false;
                    model.IsPublicAdminHidden = false;
                    break;
                case PublicStatus.Public:
                    model.IsPublicAdmin = true;
                    model.IsPublicAdminHidden = false;
                    break;
                case PublicStatus.Hidden:
                    model.IsPublicAdmin = true;
                    model.IsPublicAdminHidden = true;
                    break;
            }
        }
    }
}
