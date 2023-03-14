using System;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Models.UserTerms
{
    /// <summary>
    /// ユーザーの規約・同意・取消
    /// </summary>
    public class UserTermsViewModel
    {
        /// <summary>
        /// ユーザー規約ID		
        /// </summary>
        [Type(typeof(Guid))]
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
