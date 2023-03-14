using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    internal class UpdateJson
    {
        /// <summary>
        /// 更新のためのURL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 更新先のコントローラのURL
        /// </summary>
        public string ControllerUrl { get; set; }
        /// <summary>
        /// RollbackのためのURL
        /// </summary>
        public string RollbackUrl { get; set; }
        /// <summary>
        /// 更新情報のjson
        /// </summary>
        public JToken Json { get; set; }
        /// <summary>
        /// 更新によって変更されたプロパティの以前の値
        /// </summary>
        public string BeforeJson { get; set; }
        /// <summary>
        /// 更新の成功可否
        /// </summary>
        public bool? IsUpdated { get; set; } = null;
        /// <summary>
        /// Rollbackしたときに成功可否
        /// </summary>
        public bool? IsRollbackFail { get; set; } = null;
        /// <summary>
        /// ReferenceSourceのregist/update/delete のタイプ
        /// </summary>
        public HttpMethod targetHttpMethod { get; set; } = new HttpMethod("Patch");
        /// <summary>
        /// Rollbackのためのメソッドタイプ
        /// </summary>
        public HttpMethod rollbackHttpMethod { get; set; } = new HttpMethod("Patch");
    }
}
