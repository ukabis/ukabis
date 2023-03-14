using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.System
{
    public class SystemFunctionModel
    {
        /// <summary>
        /// ファンクション名
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string AbsoluteUrl { get; set; }

        /// <summary>
        /// 認証されているか
        /// </summary>
        public bool Authrize { get; set; }

        /// <summary>
        /// ファンクションの子要素
        /// </summary>
        public IList<SystemFunctionModel> ChildList { get; set; }
    }
}
