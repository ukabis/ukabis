using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Core.Component.Models
{
    public class MethodRepositoryGroupViewModel
    {
        /// <summary>
        /// リポジトリグループID
        /// </summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>
        /// リポジトリグループ名
        /// </summary>
        public string RepositoryGroupName { get; set; }

        /// <summary>
        /// リポジトリグループタイプ
        /// </summary>
        public string RepositoryTypeCd { get; set; }

        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// プライマリーかどうか
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// セカンダリかどうか
        /// </summary>
        public bool IsSecondary { get; set; }

        /// <summary>
        /// デフォルトかどうか
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// RDBMSかどうか
        /// </summary>
        public bool IsRdbmsRepository => (RepositoryTypeCd == "ss2");

        /// <summary>
        /// DataLakeStoreかどうか
        /// </summary>
        public bool IsDataLakeStore => (RepositoryTypeCd == "dls");

        /// <summary>
        /// 添付ファイルBlobストレージか
        /// </summary>
        public bool IsAttachFileBlobStorage => (RepositoryTypeCd == "afb");

        /// <summary>
        /// 添付ファイルメタストレージか
        /// </summary>
        public bool IsAttachFileMetaStorage => (RepositoryTypeCd == "afm" || RepositoryTypeCd == "afs");

        /// <summary>
        /// ブロックチェーンノードか
        /// </summary>
        public bool IsBlockchainNode => (RepositoryTypeCd == "bcn");

        /// <summary>
        /// 履歴用ストレージか
        /// </summary>
        public bool IsHistoryStorage => (RepositoryTypeCd == "dhs");

        /// <summary>
        /// メソッド画面の一覧に表示するか
        /// </summary>
        public bool IsMethodDetailListVisible => (!IsAttachFileBlobStorage && !IsAttachFileMetaStorage && !IsHistoryStorage && !IsBlockchainNode);
    }
}
