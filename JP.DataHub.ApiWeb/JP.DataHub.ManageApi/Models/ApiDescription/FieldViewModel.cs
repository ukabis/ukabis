using System;
using Newtonsoft.Json;

namespace JP.DataHub.ManageApi.Models.ApiDescription
{
    public class FieldViewModel
    {
        [JsonIgnore]
        public string ApiId { get; set; }
        public string FieldId { get; set; }
        public string ParentFieldId { get; set; }
        public string FieldName { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
