namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class RegisterApiLinkViewModel
    {
        /// <summary>
        /// APIリンクID
        /// </summary>
        public string ApiLinkId { get; set; }

        /// <summary>
        /// APIリンク表示名
        /// </summary>
        public string LinkTitle { get; set; }

        /// <summary>
        /// APIリンク詳細
        /// </summary>
        public string LinkDetail { get; set; }

        /// <summary>
        /// APIリンクURL
        /// </summary>
        public string LinkUrl { get; set; }
    }
}
