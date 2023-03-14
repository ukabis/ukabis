using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class TermsGroupModel
    {
        /// <summary>
        /// 規約グループコード		
        /// </summary>
        public string TermsGroupCode { get; set; }
        /// <summary>
        /// 規約グループ名		
        /// </summary>
        public string TermsGroupName { get; set; }
        /// <summary>
        /// リソースグループタイプ		
        /// </summary>
        public string ResourceGroupTypeCode { get; set; }
    }
    public class TermsGroupRegisterResponseModel
    {
        /// <summary>
        /// 規約グループコード		
        /// </summary>
        public string TermsGroupCode { get; set; }
    }
}
