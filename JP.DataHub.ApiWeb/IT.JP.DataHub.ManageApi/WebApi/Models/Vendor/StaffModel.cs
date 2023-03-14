using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Vendor
{

    public class RegisterStafforModel
    {
        public string VendorId { get; set; }
        public string Account { get; set; }
        public string RoleId { get; set; }
    }
    public class UpdateStaffModel
    {
        public string VendorId { get; set; }
        public string StaffId { get; set; }
        public string Account { get; set; }
        public string EmailAddress { get; set; }
        public string RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class ExistsStaffModel
    {
        public string StaffAccount { get; set; }
    }
    public class ExistsStaffResultModel
    {
        public string VendorId { get; set; }
        public string StaffAccount { get; set; }
    }
    public class StaffModel
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
