
namespace JP.DataHub.Com.Consts
{
    public static class MediaTypeConst
    {
        public const string Wildcard = "*/*";
        public static readonly string ApplicationJson = "application/json";
        public static readonly string ApplicationProblemJson = "application/problem+json";
        public static readonly string ApplicationXml = "application/xml";
        public static readonly string ApplicationProblemXml = "application/problem+xml";
        public static readonly string ApplicationGeoJson = "application/geo+json";
        public static readonly string ApplicationVndGeoJson = "application/vnd.geo+json";
        public static readonly string MimeApplicationXWwwFormUrlencoded = "application/x-www-form-urlencoded";
        public static readonly string TextCsv = "text/csv";
        public static readonly string TextXml = "text/xml";
        public static readonly string TextPlain = "text/plain";
        public static readonly string ApplicationOctetStream = "application/octet-stream";
        public static readonly string[] TextContentTypes = { ApplicationJson, ApplicationGeoJson, ApplicationVndGeoJson, ApplicationProblemJson, ApplicationProblemXml, ApplicationXml, TextCsv, TextXml, TextPlain };
    }
}
