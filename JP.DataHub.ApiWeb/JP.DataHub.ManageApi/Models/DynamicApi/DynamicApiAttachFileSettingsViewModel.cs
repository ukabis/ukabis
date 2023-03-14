namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class DynamicApiAttachFileSettingsViewModel
    {
        public bool IsEnable { get; set; } = false;
        public string MetaRepositoryId { get; set; }
        public string BlobRepositoryId { get; set; }
    }
}
