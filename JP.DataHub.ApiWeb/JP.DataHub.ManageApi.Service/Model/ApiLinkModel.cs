using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ApiLinkModel
    {
        /// <summary>
        /// ApiリンクID
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

        /// <summary>
        /// APIリンク表示フラグ
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// アクティブか
        /// </summary>
        public bool IsActive { get; set; }
    }
}
