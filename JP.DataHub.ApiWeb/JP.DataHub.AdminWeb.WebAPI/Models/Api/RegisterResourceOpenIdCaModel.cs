using System;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class RegisterResourceOpenIdCaModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ControllerId { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// アクセス制御（alw:許可 / dny:拒否 / inh:継承）
        /// </summary>
        public string AccessControl { get; set; }

        public class RegisterResourceOpenIdCaModelComparer : IEqualityComparer<RegisterResourceOpenIdCaModel>
        {
            public bool Equals(RegisterResourceOpenIdCaModel a, RegisterResourceOpenIdCaModel b)
            {
                if(a.ControllerId == b.ControllerId && a.ApplicationId == b.ApplicationId && 
                     a.AccessControl == b.AccessControl && a.IsActive == b.IsActive)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(RegisterResourceOpenIdCaModel a)
            {
                return a.ApplicationId.GetHashCode();
            }
        }
    }
}
