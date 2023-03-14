using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Models.Terms
{
    /// <summary>
    /// 規約
    /// </summary>
    public class TermsViewModel
    {
        /// <summary>
        /// 規約ID
        /// </summary>
        [Type(typeof(Guid))]
        public string TermsId { get; set; }
        /// <summary>
        /// 規約バージョン
        /// </summary>
        [Required]
        public string VersionNo { get; set;}
        /// <summary>
        /// 規約適用開始日
        /// </summary>
        [Required]
        public DateTime FromDate { get; set; }
        /// <summary>
        /// 規約文書
        /// </summary>
        [Required]
        public string TermsText { get; set; }
        /// <summary>
        /// 規約グループコード
        /// </summary>
        [Required]
        public string TermsGroupCode { get; set; }
    }
}
