namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class DynamicApiMethodLinkViewModel
    {
        /// <summary>
        /// MethodLinkId
        /// </summary>
        public string MethodLinkId { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 詳細
        /// </summary>
        public string Detail { get; set; }

        // 表示/非表示
        public bool IsVisible { get; set; }
    }
}
