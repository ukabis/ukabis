using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class JasPrintModel
    {
        public string JasPrintId { get; set; }
        public DateTime PrintDate { get; set; }
        public string ProductCode { get; set; }
        public int PrintCount { get; set; }
        public int ReprintCount { get; set; }
        public string ReprintReason { get; set; }
        public string CompanyId { get; set; }
        public string PrinterId { get; set; }
        public string PrintUser { get; set; }
        public string LastGln { get; set; }
        public string OpenId { get; set; }
        public string ArrilvaId { get; set; }
    }

    public class JasPrintRequestModel
    {
        public string ProductCode { get; set; }
        public int PrintCount { get; set; }
        public string CompanyId { get; set; }
        public string PrinterId { get; set; }
        public string PrintUser { get; set; }
        public string LastGln { get; set; }
        public string ArrivalId { get; set; }
        public string OpenId { get; set; }
    }

    public class JasRePrintRequestModel
    {
        public string ProductCode { get; set; }
        public int ReprintCount { get; set; }
        public string ReprintReason { get; set; }
        public string CompanyId { get; set; }
        public string PrinterId { get; set; }
        public string PrintUser { get; set; }
        public string LastGln { get; set; }
        public string ArrivalId { get; set; }
        public string OpenId { get; set; }
    }

    public class  GetPrintableCountResultModel
    {
        public string ProductCode { get; set; } 
        public int Count { get; set; }
        public int PrintableCount { get; set; }
        public int PrintedCount { get; set; }

    }
}
