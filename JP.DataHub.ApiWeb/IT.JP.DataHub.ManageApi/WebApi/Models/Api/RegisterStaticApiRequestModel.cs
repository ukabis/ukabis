using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Api
{
    public class RegisterStaticApiRequestModel
    {
        public string ApiId { get; set; }
    }
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
