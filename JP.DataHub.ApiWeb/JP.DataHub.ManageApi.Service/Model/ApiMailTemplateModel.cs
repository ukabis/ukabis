namespace JP.DataHub.ManageApi.Service.Model
{
    public class ApiMailTemplateModel
    {
        public string ApiMailTemplateId { get; set; }
        public string ApiId { get; set; }
        public string VendorId { get; set; }
        public string MailTemplateId { get; set; }
        public bool NotifyRegister { get; set; }
        public bool NotifyUpdate { get; set; }
        public bool NotifyDelete { get; set; }
        public bool IsActive { get; set; }
    }
}
