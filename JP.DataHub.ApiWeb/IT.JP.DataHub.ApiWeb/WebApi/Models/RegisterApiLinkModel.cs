using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class RegisterApiLinkModel
    {
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
