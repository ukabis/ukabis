using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    /// <summary>
    /// 添付ファイルメタ情報に対する操作の情報
    /// </summary>
    internal record AttachFileOperation : IValueObject
    {
        /// <summary>
        /// 実行中のActionが添付ファイルメタ情報の検索(GetAttachFileMetaList)かどうか
        /// </summary>
        public bool IsMetaQuery => (QuerySelectFields != null);

        /// <summary>
        /// 添付ファイルメタ情報の検索で取得する項目
        /// </summary>
        public string QuerySelectFields { get; }


        public AttachFileOperation(string querySelectFields = null)
        {
            QuerySelectFields = querySelectFields;
        }


        public static bool operator ==(AttachFileOperation me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AttachFileOperation me, object other) => !me?.Equals(other) == true;
    }
}