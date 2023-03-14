using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// スタッフ情報
    /// </summary>
    public class StaffModel
    {
        /// <summary>
        /// StaffId
        /// </summary>
        public string StaffId { get; set; }

        /// <summary>
        /// OpenIDアカウント
        /// </summary>
        [RequiredGUID(ErrorMessageWhenNull = "スタッフのアカウントは必須項目です。", ErrorMessageWhenNotGuid = "アカウントはGUID形式で入力してください。")]
        public string Account { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        [Required(ErrorMessage = "スタッフのメールアドレスは必須項目です。")]
        [EmailAddress(ErrorMessage = "メールアドレスの形式で入力してください。")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// StaffRoleId
        /// </summary>

        public string StaffRoleId { get; set; }


        /// <summary>
        /// RoleId
        /// </summary>
        [Required(ErrorMessage = "スタッフの権限は必須項目です。")]
        public string RoleId { get; set; }

        /// <summary>
        /// 権限名
        /// </summary>
        public string RoleName { get; set; }


        /// <summary>
        /// VendorId
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 画面入力で新規追加したスタッフか
        /// </summary>
        public bool IsNewStaff { get; set; } = false;

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public StaffModel Clone()
        {
            return (StaffModel)this.MemberwiseClone();
        }
    }
}
