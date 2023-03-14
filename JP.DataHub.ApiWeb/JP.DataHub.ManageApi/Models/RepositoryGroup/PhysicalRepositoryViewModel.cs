using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.RepositoryGroup
{
    public class PhysicalRepositoryViewModel
    {
        public string PhysicalRepositoryId { get; set; }

        [Required(ErrorMessage = "必須項目です。")]
        public string ConnectionString { get; set; }

        public bool IsFull { get; set; }

        public bool IsActive { get; set; }
    }
}
