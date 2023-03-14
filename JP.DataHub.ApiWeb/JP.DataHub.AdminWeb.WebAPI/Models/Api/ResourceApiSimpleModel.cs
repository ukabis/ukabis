using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ResourceApiSimpleModel
    {
        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// Apiの相対パス
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// Methodリスト
        /// </summary>
        public IEnumerable<ApiSimpleModel> MethodList { get; set; }
    }
}
