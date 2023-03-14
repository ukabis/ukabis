using JP.DataHub.ManageApi.Attributes;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class RegisterApiIpFilterViewModel
    {
        [ValidateIpAddress]
        public string IpAddress { get; set; }

        public bool IsEnable { get; set; }
        public bool IsActive { get; set; }
    }
}
