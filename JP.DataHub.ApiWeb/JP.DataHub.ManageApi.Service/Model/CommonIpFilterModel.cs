namespace JP.DataHub.ManageApi.Service.Model
{
    public class CommonIpFilterModel
    {
        public string CommonIpFilterId { get; set; }
        public string IpAddress { get; set; }
        public bool IsEnable { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
