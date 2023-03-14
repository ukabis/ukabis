using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class RegisterStaticApiResponseModel
    {
        /// <summary>
        /// API ID
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// APIのURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; set; }
    }
}
