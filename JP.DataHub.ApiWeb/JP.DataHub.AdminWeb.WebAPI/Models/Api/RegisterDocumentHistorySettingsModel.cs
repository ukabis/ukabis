namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class RegisterDocumentHistorySettingsModel
    {
        public string ControllerId { get; set; }

        public bool IsEnable { get; set; } = false;

        public string HistoryRepositoryId { get; set; }
    }
}
