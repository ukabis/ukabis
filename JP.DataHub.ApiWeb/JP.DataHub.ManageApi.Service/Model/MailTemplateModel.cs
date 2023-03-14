namespace JP.DataHub.ManageApi.Service.Model
{
    public class MailTemplateModel
    {
        public string MailTemplateId { get; set; }
        public string VendorId { get; set; }
        public string MailTemplateName { get; set; }
        public string FromMailAddress { get; set; }
        public string[] ToMailAddress { get; set; }
        public string[] CcMailAddress { get; set; }
        public string[] BccMailAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsActive { get; set; }
    }
}
