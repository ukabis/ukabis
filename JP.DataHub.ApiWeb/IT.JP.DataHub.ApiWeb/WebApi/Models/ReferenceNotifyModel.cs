using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ReferenceNotifyModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? temp { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},ConversionSquareMeters;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Ref1 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},ConversionSquareMeters2;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Ref2 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},Obj;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ReferenceNotifyObject Ref3 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},Array[2];Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref4 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},NumberArray[1];Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Ref5 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},ArrayObject[1].Date;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref6 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},Object.Prop3.Prop31;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref7 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},abc;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref8 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},ArrayObject[0];Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PropArrayItem Ref9 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},ArrayObject;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<PropArrayItem> Ref10 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},BooleanNullProp;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Ref11 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},NumberNullProp;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Ref12 { get; set; }

        // Reference /API/IntegratedTest/ReferenceSource/Get/{code},StringNullProp;Notify;Protect
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref13 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<PropArrayItem> PropArray { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PropObject PropObject { get; set; }
    }

    public class ReferenceNotifyObject
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prop1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prop2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prop3 { get; set; }
    }

    /// <summary>
    /// 全てnullプロパティあり
    /// </summary>
    public class ReferenceNotifyObjectEx
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string prop1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string prop2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string prop3 { get; set; }
    }
}
