using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Models.CertifiedApplication
{
    public class CertifiedApplicationViewModel
    {
        /// <summary>
        /// 認定アプリケーションID		
        /// </summary>
        [Type(typeof(Guid))]
        public string CertifiedApplicationId { get; set; }
        /// <summary>
        /// 認定アプリケーション名
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }
        /// <summary>
        /// 認定アプリケーションのベンダーID
        /// </summary>
        [Required]
        public string VendorId { get; set; }
        /// <summary>
        /// 認定アプリケーションのシステムID
        /// </summary>
        [Required]
        public string SystemId { get; set; }
    }
}
