
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    /// メソッドリンク情報
    /// </summary>
    public class MethodLinkModel : LinkModelBase
    {
        /// <summary>メソッドID</summary>
        public Guid MethodId { get; set; }

        /// <summary>メソッドリンクID</summary>
        public Guid MethodLinkId { get; set; }
    }
}
