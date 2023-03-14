
namespace JP.DataHub.Com.Net.Http.Models
{
    public class GetDocumentVersionResponseModel
    {
        public string VersionKey { get; set; }
        public int VersionNo { get; set; }
        public string CreateDate { get; set; }
        public string OpenId { get; set; }
        public string LocationType { get; set; }
        public string RepositoryGroupId { get; set; }
        public string PhysicalRepositoryId { get; set; }
        public string Location { get; set; }
    }
}
