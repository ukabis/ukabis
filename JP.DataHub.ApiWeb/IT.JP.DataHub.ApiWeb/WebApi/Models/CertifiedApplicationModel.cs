using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class CertifiedApplicationModel
    {
        /// <summary>
        /// 認定アプリケーションID		
        /// </summary>
        public string CertifiedApplicationId { get; set; }
        /// <summary>
        /// 認定アプリケーション名
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// 認定アプリケーションのベンダーID
        /// </summary>
        public string VendorId { get; set; }
        /// <summary>
        /// 認定アプリケーションのシステムID
        /// </summary>
        public string SystemId { get; set; }
    }

    public class CertifiedApplicationRegisterResponseModel
    {
        /// <summary>
        /// 認定アプリケーションID		
        /// </summary>
        public string CertifiedApplicationId { get; set; }
    }
}
