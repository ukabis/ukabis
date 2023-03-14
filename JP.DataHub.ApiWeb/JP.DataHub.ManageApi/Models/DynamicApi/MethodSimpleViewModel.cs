using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class MethodSimpleViewModel
    {
        /// <summary>
        /// MethodId
        /// </summary>
        public string MethodId { get; set; }

        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// メソッドタイプ
        /// </summary>
        public string MethodType { get; set; }

        /// <summary>
        /// MethodUrl
        /// </summary>
        public string MethodUrl { get; set; }

        /// <summary>
        /// 透過Methodか
        /// </summary>
        public bool IsTransparent { get; set; }
    }
}
