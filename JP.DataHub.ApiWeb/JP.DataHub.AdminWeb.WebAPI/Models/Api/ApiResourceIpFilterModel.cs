using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{ 
    public class ApiResourceIpFilterModel
    {
        public string ControllerId { get; set; }

        [RequiredIpAddress]
        public string IpAddress { get; set; }

        public bool IsEnable { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public ApiResourceIpFilterModel Clone()
        {
            return (ApiResourceIpFilterModel)MemberwiseClone();
        }
    }
}
