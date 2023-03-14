using System;

namespace JP.DataHub.ManageApi.Models.ApiDescription
{
    public class SampleCodeViewModel
    {
        public string MethodId { get; set; }
        public string SampleCodeId { get; set; }
        public string Language { get; set; }
        public int DisplayOrder { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
