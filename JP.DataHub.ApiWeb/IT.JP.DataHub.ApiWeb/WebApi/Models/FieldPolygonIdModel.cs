using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class FieldPolygonIdModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FieldPolygonId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? MinLongitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? MaxLongitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? MinLatitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? MaxLatitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FieldType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearch GeoSearch { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FieldPolygonIdPolygonItem> Polygons { get; set; }
    }

    public class FieldPolygonIdGeoSearch
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_4 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_5 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_6 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_7 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_8 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_9 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_10 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_11 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_12 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_13 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_14 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_15 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_16 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_17 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_18 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_19 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_20 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_21 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_22 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_23 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_24 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_25 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_26 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_27 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_28 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_29 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_30 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_31 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_32 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_33 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_34 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_35 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_36 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_37 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_38 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_39 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_40 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_41 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_42 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_43 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_44 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_45 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_46 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_47 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_48 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_49 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_50 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_51 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_52 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_53 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_54 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_55 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_56 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_57 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_58 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_59 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_60 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_61 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_62 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_63 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_64 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_65 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_66 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_67 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_68 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_69 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_70 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_71 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_72 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_73 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_74 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_75 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_76 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_77 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_78 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_79 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_80 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_81 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_82 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_83 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_84 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_85 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_86 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_87 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_88 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_89 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_90 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_91 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_92 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_93 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_94 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_95 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_96 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_97 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_98 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldPolygonIdGeoSearchItem GeoSearch_99 { get; set; }
    }

    public class FieldPolygonIdGeoSearchItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<List<List<decimal>>> coordinates { get; set; }
    }

    public class FieldPolygonIdPolygonItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FieldPolygonIdPolygonItemPoint> Coordinates { get; set; }
    }

    public class FieldPolygonIdPolygonItemPoint
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Latitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Longitude { get; set; }
    }
}
