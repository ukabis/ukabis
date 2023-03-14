using JP.DataHub.ManageApi.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.AdminInfo
{
    public class UpdateAdminFuncInfomationViewModel
    {
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string AdminFuncRoleId { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
    }
}
