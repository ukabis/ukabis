namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class RegisterAccessVendorModel
    {
        /// <summary>
        /// 許可・除外対象のベンダーID
        /// </summary>
        public string VendorId { get; set; }
        /// <summary>
        /// 許可・除外対象のシステムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// アクセスコントロールの許可設定（true:許可 false:非許可）
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// アクセスコントロールのアクセスキー
        /// </summary>
        public string AccessKey { get; set; }
    }
}
