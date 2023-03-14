using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    /// メソッドリンク情報
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class MethodLink : LinkBase
    {
        /// <summary>メソッドID</summary>
        [Key(6)]
        public Guid MethodId { get; set; }

        /// <summary>メソッドリンクID</summary>
        [Key(7)]
        public Guid MethodLinkId { get; set; }
    }
}
