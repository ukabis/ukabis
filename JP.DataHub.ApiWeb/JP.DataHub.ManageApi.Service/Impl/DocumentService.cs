using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Attributes;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class DocumentService : AbstractService, IDocumentService
    {
        private Lazy<IDocumentRepository> _lazyDocumentRepository = new(() => UnityCore.Resolve<IDocumentRepository>());
        private IDocumentRepository _documentRepository { get => _lazyDocumentRepository.Value; }
        private Lazy<ISendMailRepository> _lazySendMailRepository = new(() => UnityCore.Resolve<ISendMailRepository>());
        private ISendMailRepository _sendMailRepository { get => _lazySendMailRepository.Value; }

        protected static Lazy<IConfigurationSection> s_lazyDocument = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("Document"));
        protected static IConfigurationSection s_document { get => s_lazyDocument.Value; }
        private static bool s_useDocumentPublishmentNotificationMail { get => s_document.GetValue<bool>("UseDocumentPublishmentNotificationMail", false); }
        private static bool s_useDocumentRegistrationNotificationMail { get => s_document.GetValue<bool>("UseDocumentRegistrationNotificationMail", false); }
        private static string s_sendMailTamplateCdForPublishment { get => s_document.GetValue<string>("SendMailTamplateCdForPublishment"); }
        private static string s_sendMailTamplateCdForRegistration { get => s_document.GetValue<string>("SendMailTamplateCdForRegistration"); }
        private static string s_PortalUri { get => s_document.GetValue<string>("PortalUri"); }
        private static string s_PortalDocumentsPath { get => s_document.GetValue<string>("PortalDocumentsPath"); }
        private static string s_adminDocumentDownloadUri { get => s_document.GetValue<string>("AdminDocumentDownloadUri"); }

        public DocumentInformationModel GetDocumentInformation(string documentId)
            => _documentRepository.GetDocumentInformation(documentId);

        public IList<DocumentInformationModel> GetDocumentInformation(bool isPublicPortal, bool isPublicAdmin)
            => _documentRepository.GetDocumentInformation(isPublicPortal, isPublicAdmin);

        public IList<DocumentCategoryModel> GetDocumentCategory()
            => _documentRepository.GetDocumentCategory();

        public IList<DocumentModel> GetDocumentList(string vendorId, bool? isEnable = null, bool? isAdminCheck = null, bool? isAdminStop = null, bool? isActive = null,
            bool? isPublicPortal = null, bool? isPublicAdmin = null, bool? isPublicPortalHidden = null, bool? isPublicAdminHidden = null)
            => _documentRepository.GetDocumentList(vendorId, isEnable, isAdminCheck, isAdminStop, isActive, isPublicPortal, isPublicAdmin, isPublicPortalHidden, isPublicAdminHidden);

        public DocumentModel Register(DocumentModel model, IList<DocumentFileModel> files = null)
        {
            var result = _documentRepository.Register(model, files);
            // ドキュメントの新規登録やファイル追加時のメール通知（運営宛）
            var registration = GetRegistrationMailParametersIfNecessary(true, model, files);
            if (registration != null)
            {
                _sendMailRepository.SendMailAsync(registration.MailTemplateCode, registration.Parameters, new Dictionary<string, string>() { { "DocumentId", model.DocumentId } });
            }
            return result;
        }

        public DocumentModel Update(DocumentModel model, IList<DocumentFileModel> files = null)
        {
            bool isPublish = false;
            var publish = GetPublishmentMailParametersIfNecessary(model);
            if (publish != null)
            {
                isPublish = true;
                var toEmailAddress = _documentRepository.GetDocumentOwnerEmailAddressList(model.DocumentId);
                if (toEmailAddress?.Count > 0)
                {
                    publish.Parameters.Add("doc_reguser_mail", string.Join(";", toEmailAddress));
                }
                else
                {
                    isPublish = false;
                }
            }
            var result = _documentRepository.Update(model, files);

            // fileListがnullの場合（ManageAPIからDocumentのみ更新の際）、file新規追加の場合(ManageAPIからFileを新規追加した際）を考慮し、ファイルリストを改めて取得する
            var nowfiles = _documentRepository.GetDocumentFileList(model.DocumentId, model.Title);

            // ドキュメントの新規登録やファイル追加時のメール通知（運営宛）
            var paramRegistration = GetRegistrationMailParametersIfNecessary(false, model, nowfiles);
            if (paramRegistration != null)
            {
                _sendMailRepository.SendMailAsync(paramRegistration.MailTemplateCode, paramRegistration.Parameters, new Dictionary<string, string>() { { "DocumentId", model.DocumentId } });
            }
            // ドキュメント公開時のメール通知（ドキュメント登録者・更新者宛）
            if (isPublish == true)
            {
                _sendMailRepository.SendMailAsync(publish.MailTemplateCode, publish.Parameters, new Dictionary<string, string>() { { "DocumentId", model.DocumentId } });
            }
            return result;
        }

        public void DeleteDocument(string documentId)
        {
            var model = _documentRepository.GetDocumentInformation(documentId);
            if (PerRequestDataContainer.VendorCheck(model) == false)
            {
                throw new AccessDeniedException();
            }
            _documentRepository.DeleteDocument(documentId);
        }

        private class MailParameters
        {
            public string MailTemplateCode { get; set; }
            public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        }

        private MailParameters GetPublishmentMailParametersIfNecessary(DocumentModel model)
        {
            // 運用会社ユーザによる更新時に公開状態なら送信
            if (!PerRequestDataContainer.IsOperatingVendorUser || !model.IsAdminCheck || model.IsAdminStop || s_useDocumentPublishmentNotificationMail == false)
            {
                return null;
            }
            var result = new MailParameters() { MailTemplateCode = s_sendMailTamplateCdForPublishment };
            result.Parameters.Add("titleName", model.Title);
            if (model.IsPublicPortal && !string.IsNullOrWhiteSpace(s_PortalUri))
            {
                result.Parameters.Add("portalDocumentUrl", s_PortalUri + s_PortalDocumentsPath + model.DocumentId);
            }
            if (model.IsPublicAdmin)
            {
                result.Parameters.Add("adminDocumentUrl", s_adminDocumentDownloadUri);
            }
            return result;
        }

        private MailParameters GetRegistrationMailParametersIfNecessary(bool isRegistration, DocumentModel model, IList<DocumentFileModel> files)
        {
            if (!isRegistration && (files == null || files.Count() <= 0) || s_useDocumentRegistrationNotificationMail == false)
            {
                return null;
            }

            List<object> fileDatas = new List<object>();
            files = files == null ? new List<DocumentFileModel>() : files;
            foreach (var file in files)
            {
                string updateInfo = "　　";
                switch (file.DocumentFileChangeInfo)
                {
                    case DocumentFileChangeInfo.Create:
                        updateInfo = "登録";
                        break;
                    case DocumentFileChangeInfo.Update:
                        updateInfo = "更新";
                        break;
                    case DocumentFileChangeInfo.Delete:
                        updateInfo = "削除";
                        break;
                }

                fileDatas.Add(
                    new
                    {
                        Status = updateInfo,
                        Title = file.Title,
                        FileName = file.Url.Split('/').Last(),
                    });
            }

            var result = new MailParameters() { MailTemplateCode = s_sendMailTamplateCdForRegistration };
            var vscn = _documentRepository.GetVendorSystemCategoryName(model.VendorId, model.SystemId, model.CategoryId);
            if (vscn.IsValid() == false)
            {
                if (vscn.IsVendorValid() == false) throw new NotFoundException("vendor_id is invalid.");
                if (vscn.IsSystemValid() == false) throw new NotFoundException("system_id is invalid.");
                if (vscn.IsCategoryValid() == false) throw new NotFoundException("category_id is invalid.");
            }
            result.Parameters = new Dictionary<string, object>()
            {
                { "documentStatus", isRegistration ? "登録" : "更新" },
                { "vendorName",  vscn.VendorName },
                { "systemName", vscn.SystemName },
                { "categoryName", vscn.CategoryName },
                { "titleName", model.Title },
                { "description", model.Detail },
                { "files",fileDatas }
            };
            return result;
        }
    }
}
