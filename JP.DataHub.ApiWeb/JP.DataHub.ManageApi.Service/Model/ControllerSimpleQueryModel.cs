using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ControllerSimpleQueryModel
    {
        public ControllerSimpleQueryModel()
        {
            ApiList = new List<ApiSimpleQueryModel>();
        }

        /// <summary>
        /// ControllerのID
        /// </summary>
        public string ControllerId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// Controllerの相対パス
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// Apiリスト
        /// </summary>
        public IEnumerable<ApiSimpleQueryModel> ApiList { get; set; }
    }
}
