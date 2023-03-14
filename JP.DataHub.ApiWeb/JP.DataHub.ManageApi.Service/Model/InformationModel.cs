namespace JP.DataHub.ManageApi.Service.Model
{
    public class InformationModel {
        public Guid InformationId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Date { get; set; }
        public bool IsVisibleApi { get; set; }
        public bool IsVisibleAdmin { get; set; }
    }
}
