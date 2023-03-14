using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.System
{
    public class SystemFunctionViewModel
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
        public IList<SystemFunctionViewModel> ChildList { get; set; }
    }
}
