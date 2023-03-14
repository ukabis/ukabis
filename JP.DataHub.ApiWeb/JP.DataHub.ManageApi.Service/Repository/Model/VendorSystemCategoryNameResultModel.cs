using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Repository.Model
{
    internal class VendorSystemCategoryNameResultModel
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        public bool IsValid()
            => !string.IsNullOrEmpty(VendorId) && !string.IsNullOrEmpty(VendorName) && !string.IsNullOrEmpty(SystemId) && !string.IsNullOrEmpty(SystemName) && !string.IsNullOrEmpty(CategoryId) && !string.IsNullOrEmpty(CategoryName);
        public bool IsVendorValid()
            => !string.IsNullOrEmpty(VendorId) && !string.IsNullOrEmpty(VendorName);
        public bool IsSystemValid()
            => !string.IsNullOrEmpty(SystemId) && !string.IsNullOrEmpty(SystemName);
        public bool IsCategoryValid()
            => !string.IsNullOrEmpty(CategoryId) && !string.IsNullOrEmpty(CategoryName);
    }
}
