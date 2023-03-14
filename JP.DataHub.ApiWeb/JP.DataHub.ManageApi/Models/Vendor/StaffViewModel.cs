namespace JP.DataHub.ManageApi.Models.Vendor
{
    /// <summary>
    /// スタッフ情報
    /// </summary>
    public class StaffViewModel
    {
        /// <summary>
        /// StaffId
        /// </summary>
        public string StaffId { get; set; }

        /// <summary>
        /// OpenIDアカウント
        /// </summary>

        public string Account { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// StaffRoleId
        /// </summary>

        public string StaffRoleId { get; set; }


        /// <summary>
        /// RoleId
        /// </summary>
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
    }
}