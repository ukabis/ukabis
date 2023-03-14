using System;

namespace JP.DataHub.ManageApi.Models.Document
{
    public class DocumentWithoutFileViewModel
    {
        /// <summary>
        /// <para>ドキュメントID</para>
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// カテゴリー
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 管理者確認
        /// </summary>
        public bool IsAdminCheck { get; set; }

        /// <summary>
        /// 管理者停止
        /// </summary>
        public bool IsAdminStop { get; set; }

        /// <summary>
        /// 規約ID
        /// </summary>
        public string AgreementId { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdDate { get; set; }

        /// <summary>
        /// 未削除
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// ポータルに公開
        /// </summary>
        public bool IsPublicPortal { get; set; }

        /// <summary>
        /// 管理画面に公開
        /// </summary>
        public bool IsPublicAdmin { get; set; }

        /// <summary>
        /// ポータルの一覧に非公開
        /// </summary>
        public bool IsPublicPortalHidden { get; set; }

        /// <summary>
        /// 管理画面の一覧に非公開
        /// </summary>
        public bool IsPublicAdminHidden { get; set; }

        /// <summary>
        /// パスワード   
        /// </summary>
        public string Password { get; set; }
    }
}
