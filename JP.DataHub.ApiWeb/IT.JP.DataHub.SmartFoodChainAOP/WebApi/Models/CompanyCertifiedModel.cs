using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class CompanyCertifiedModel
    {
        public string CompanyCertifiedId { get; set; }
        public string DataSourceCertificationId { get; set; }
        public string DataSourceType { get; set; }
        public string CertificationName { get; set; }
        public string CertificationNo { get; set; }
        public CertificationIssuanceModel CertificationIssuance { get; set; }
        public string CompanyId { get; set; }
        public string CertificationAcquisitionDate { get; set; }
        public string ExpireDate { get; set; }
        public string RegisteredDate { get; set; }
        public List<EvidenceFileModel> EvidenceFiles { get; set; }
    }

    public class CertificationIssuanceModel
    {
        public string CertificationIssuanceName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Tel { get; set; }
        public string MailAddress { get; set; }
    }

    public class EvidenceFileModel
    {
        public string FilePath { get; set; }
        public string FileTypeCode { get; set; }
    }
}
