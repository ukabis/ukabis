using System;

namespace JP.DataHub.ManageApi.Models.ApiDescription
{
    public class SchemaDescriptionViewModel
    {
        public string SchemaId { get; set; }
        public string SchemaName { get; set; }
        public string JsonSchema { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
