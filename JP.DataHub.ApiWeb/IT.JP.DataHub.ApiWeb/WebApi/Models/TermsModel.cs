using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class TermsModel
    {
        public string TermsId { get; set; }
        /// <summary>
        /// 規約バージョン
        /// </summary>
        public string VersionNo { get; set; }
        /// <summary>
        /// 規約適用開始日
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// 規約文書
        /// </summary>
        public string TermsText { get; set; }
        /// <summary>
        /// 規約グループコード
        /// </summary>
        public string TermsGroupCode { get; set; }
    }
    public class TermsRegisterResponseModel
    {
        public string TermsId { get; set; }
    }
}
