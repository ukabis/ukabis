using System;

namespace JP.DataHub.ManageApi.Models.AdminInfo
{
    public class AdminFuncInfomationViewModel
    {
        public Guid AdminFuncId { get; set; }
        public string AdminName { get; set; }
        public Guid AdminFuncRoleId { get; set; }
        public Guid RoleId { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
        public string DisplayDescription { get; set; }
    }
}
