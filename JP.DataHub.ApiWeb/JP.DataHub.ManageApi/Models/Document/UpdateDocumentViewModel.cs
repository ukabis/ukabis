using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.Document
{
    public class UpdateDocumentViewModel
    {
        /// <summary>
        /// ドキュメントID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid DocumentId { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Document, DocumentDatabase.TABLE_DOCUMENT, DocumentDatabase.COLUMN_DOCUMENT_TITLE)]
        public string Title { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Document, DocumentDatabase.TABLE_DOCUMENT, DocumentDatabase.COLUMN_DOCUMENT_DETAIL)]
        public string Detail { get; set; }

        /// <summary>
        /// カテゴリーID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid SystemId { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 管理者確認
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public bool IsAdminCheck { get; set; }

        /// <summary>
        /// 管理者停止
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public bool IsAdminStop { get; set; }

        /// <summary>
        /// 規約ID
        /// </summary>
        public Guid? AgreementId { get; set; }

        /// <summary>
        /// パスワード   
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ポータル公開設定 
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public PublicStatus IsPublicPortalStatus { get; set; }

        /// <summary>
        /// 管理画面公開設定 
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public PublicStatus IsPublicAdminStatus { get; set; }
    }
}
