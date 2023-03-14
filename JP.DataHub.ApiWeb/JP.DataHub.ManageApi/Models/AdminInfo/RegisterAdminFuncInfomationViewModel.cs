using JP.DataHub.ManageApi.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.AdminInfo
{
    public class RegisterAdminFuncInfomationViewModel
    {
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string AdminFuncId { get; set; }
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string RoleId { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
    }
}
