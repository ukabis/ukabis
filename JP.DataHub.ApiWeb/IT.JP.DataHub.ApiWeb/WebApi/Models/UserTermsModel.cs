using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class UserTermsModel
    {
        /// <summary>
        /// ユーザー規約ID		
        /// </summary>
        public string UserTermsId { get; set; }
        /// <summary>
        /// ユーザーのOpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 規約ID
        /// </summary>
        public string TermsId { get; set; }
        /// <summary>
        /// 同意した日時
        /// </summary>
        public DateTime AgreementDate { get; set; }
        /// <summary>
        /// 取消した日時
        /// </summary>
        public DateTime RevokeDate { get; set; }
    }
}
