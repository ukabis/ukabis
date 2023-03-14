using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class OfficeModel
    {
        public string OfficeId { get; set; }
        public string CompanyId { get; set; }
        public string OfficeName { get; set; }
        public List<NameLangModel> OfficeNameLang { get; set; }
        public string OfficeNameKana { get; set; }
        public string ZipCode { get; set; }
        public string Address1 { get; set; }
        public List<AddressLangModel> Address1Lang { get; set; }
        public string Address2 { get; set; }
        public List<AddressLangModel> Address2Lang { get; set; }
        public string Address3 { get; set; }
        public List<AddressLangModel> Address3Lang { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string GlnCode { get; set; }
        public bool IsPublic { get; set; }
    }
}
