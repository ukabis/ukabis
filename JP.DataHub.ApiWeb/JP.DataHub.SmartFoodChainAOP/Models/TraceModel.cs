using MessagePack;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
    public class TraceModel
    {
        public enum TraceClassEnum
        {
            shp,
            arv,
            trc,
            otr
        }

        public string ArrivalId { get; set; }

        public DateTime Date { get; set; }

        public string OfficeId { get; set; }

        public string OpenId { get; set; }

        public string ProductCode { get; set; }

        public string ShipmentId { get; set; }

        public TraceClassEnum TraceClass { get; set; }

        public string TraceDetailId { get; set; }

        public string TraceMemo { get; set; }

        public string TraceTitle { get; set; }
    }
}
