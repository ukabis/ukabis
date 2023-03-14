namespace JP.DataHub.ManageApi.Service.Model
{
    public class CommonIpFilterGroupModel
    {
        public string CommonIpFilterGroupId { get; set; }
        public string CommonIpFilterGroupName { get; set; }
        public IList<CommonIpFilterModel> IpList { get; set; }
    }
}
