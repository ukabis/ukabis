using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class DocumentHistorySettingsModel
    {
        public bool IsEnable { get; set; }

        [RequiredBlobWhenHistoryEnable]
        public string HistoryRepositoryId { get; set; }

        public DocumentHistorySettingsModel(bool isEnable, string historyRepositoryId)
        {
            IsEnable = isEnable;
            HistoryRepositoryId = historyRepositoryId?.ToLower();
        }
    }
}
