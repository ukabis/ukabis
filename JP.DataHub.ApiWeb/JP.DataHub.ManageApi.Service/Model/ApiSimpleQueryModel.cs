using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ApiSimpleQueryModel
    {
        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// ControllerId
        /// </summary>
        public string ControllerId { get; set; }

        /// <summary>
        /// メソッドタイプ
        /// </summary>
        public string MethodType { get; set; }

        /// <summary>
        /// ApiUrl
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// 透過APIか
        /// </summary>
        public bool IsTransparent { get; set; }
    }
}
