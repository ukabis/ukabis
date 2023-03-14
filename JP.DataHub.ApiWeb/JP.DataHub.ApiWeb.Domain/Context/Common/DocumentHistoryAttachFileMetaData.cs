using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    /// <summary>
    /// 履歴用の添付ファイル情報
    /// 履歴から添付ファイルを取得するのに最低限必要なデータだけ保持する
    /// </summary>
    internal class DocumentHistoryAttachFileMetaData
    {
        public DocumentHistoryAttachFileMetaData(string contentType, string key)
        {
            this.ContentType = string.IsNullOrEmpty(contentType) ? null : contentType;
            this.Key = string.IsNullOrEmpty(key) ? null : key;
        }

        /// <summary>
        /// MIMEタイプ
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; }
    }
}
