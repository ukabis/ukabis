using System;
using Newtonsoft.Json;

namespace JP.DataHub.ManageApi.Service.Model
{
    /// <summary>
    /// リンク情報の基底クラス
    /// </summary>
    public class LinkBase
    {
        [JsonProperty("LinkTitle")]
        public string Title { get; set; }
        [JsonProperty("LinkDetail")]
        public string Detail { get; set; }
        [JsonProperty("LinkUrl")]
        public string Url { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
