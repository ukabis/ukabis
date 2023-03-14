using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ApiSimpleViewModel
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
        public IEnumerable<MethodSimpleViewModel> MethodList { get; set; }
    }
}
