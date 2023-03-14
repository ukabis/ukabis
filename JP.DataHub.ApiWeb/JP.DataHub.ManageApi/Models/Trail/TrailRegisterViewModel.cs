namespace JP.DataHub.ManageApi.Models.Trail
{
    public class TrailRegisterViewModel
    {
        public string TrailId { get; set; }
        public string TrailType { get; set; }
        public bool Result { get; set; }
        public TrailRegisterDetailViewModel Detail { get; set; }
    }

    public class TrailRegisterDetailViewModel
    {
        public string Screen { get; set; }
        public string OpenId { get; set; }
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Url { get; set; }
        public string HttpMethodType { get; set; }
        public string ContollerClassName { get; set; }
        public string ActionMethodName { get; set; }
        public string TrailOperation { get; set; }
        public string HttpStatusCode { get; set; }
        public string MethodParameter { get; set; }
        public string MethodResult { get; set; }
    }
}
