using System;
using Newtonsoft.Json;

namespace JP.DataHub.ManageApi.Models.ApiDescription
{
    public class CategoryViewModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
