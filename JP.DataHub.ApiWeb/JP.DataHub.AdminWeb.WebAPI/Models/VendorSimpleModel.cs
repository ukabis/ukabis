
namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// ベンダー情報
    /// </summary>
    public class VendorSimpleModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// システム一覧
        /// </summary>
        public List<SystemModel> SystemList { get; set; }


        public class SystemModel
        {
            /// <summary>
            /// システムID
            /// </summary>
            public string SystemId { get; set; }
            /// <summary>
            /// システム名
            /// </summary>
            public string SystemName { get; set; }
        }
    }
}
