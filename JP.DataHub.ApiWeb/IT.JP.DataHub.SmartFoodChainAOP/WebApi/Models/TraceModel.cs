
namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class TraceModel
    {
        public string ArrivalId { get; set; }
        public string AttachFileId { get; set; }
        public DateTime Date { get; set; }
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string OpenId { get; set; }
        public string ProductCode { get; set; }
        public string TraceClass { get; set; }
        public string TraceDetailId { get; set; }
        public string TraceMemo { get; set; }
        public string TraceTitle { get; set; }
    }

    public enum TraceClassEnum
    {
        shp,
        arv,
        trc,
        otr
    }
}
