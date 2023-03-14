using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class IsDuplicateMethodModel
    {
        /// <summary>
        /// アクションタイプ
        /// </summary>
        public string ActionType { get; set; }
        /// <summary>
        /// メソッドタイプ
        /// </summary>
        public string HttpMethodType { get; set; }
        /// <summary>
        /// API(コントローラー)ID
        /// </summary>
        public string ApiId { get; set; }
        /// <summary>
        /// メソッドURL
        /// </summary>
        public string MethodUrl { get; set; }
        /// <summary>
        /// メソッドID
        /// </summary>
        public string MethodId { get; set; }
    }
}
