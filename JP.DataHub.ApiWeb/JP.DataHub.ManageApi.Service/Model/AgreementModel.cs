namespace JP.DataHub.ManageApi.Service.Model
{
    public class AgreementModel {
        public string AgreementId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string VendorId { get; set; }
        public DateTime UpdDate { get; set; }
        public bool IsActive { get; set; }
    }
}
