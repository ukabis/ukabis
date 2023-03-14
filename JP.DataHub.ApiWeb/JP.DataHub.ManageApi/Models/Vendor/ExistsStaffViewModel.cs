using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class ExistsStaffViewModel
    {
        [Required(ErrorMessage = "必須項目です。")]
        public string StaffAccount { get; set; }
    }
}
