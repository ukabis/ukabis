namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class RegisterDynamicApiAttachFileSettingsModel
    {
        public string ControllerId { get; set; }
        public bool IsEnable { get; set; } = false;
        public string MetaRepositoryId { get; set; }
        public string BlobRepositoryId { get; set; }
    }
}
