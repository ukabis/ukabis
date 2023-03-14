using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ApiWeb.Models.Terms
{
    public class TermsGroupViewModel
    {
        /// <summary>
        /// 規約グループコード		
        /// </summary>
        [Required]
        public string TermsGroupCode { get; set; }
        /// <summary>
        /// 規約グループ名		
        /// </summary>
        [Required]
        public string TermsGroupName { get; set; }
        /// <summary>
        /// リソースグループタイプ		
        /// </summary>
        [MaxLength(3)]
        [Required]
        public string ResourceGroupTypeCode { get; set; }
    }
}
