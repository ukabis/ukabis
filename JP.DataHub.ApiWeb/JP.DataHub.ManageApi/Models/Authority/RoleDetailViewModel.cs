namespace JP.DataHub.ManageApi.Models.Authority
{
    public class RoleDetailViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string FuncName { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
    }
}
