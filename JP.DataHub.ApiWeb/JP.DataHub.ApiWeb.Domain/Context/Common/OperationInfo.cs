using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    /// <summary>
    /// データ操作の情報
    /// </summary>
    internal record OperationInfo : IValueObject
    {
        public enum OperationType
        {
            Normal,
            VersionInfo,
            AttachFileMeta,
            DocumentVersion
        }

        /// <summary>
        /// 実行中のActionの種類
        /// </summary>
        public OperationType Type { get; } = OperationType.Normal;

        /// <summary>
        /// 実行中のActionが通常のデータ操作であるかどうか
        /// </summary>
        public bool IsNormalOperation => Type == OperationType.Normal;

        /// <summary>
        /// 実行中のActionがバージョン情報に対する操作であるかどうか
        /// </summary>
        public bool IsVersionOperation => Type == OperationType.VersionInfo;

        /// <summary>
        /// 実行中のActionが添付ファイルメタ情報に対する操作であるかどうか
        /// </summary>
        public bool IsAttachFileOperation => Type == OperationType.AttachFileMeta;

        /// <summary>
        /// 実行中のActionが履歴情報に対する操作であるかどうか
        /// </summary>
        public bool IsDocumentVersionOperation => Type == OperationType.DocumentVersion;

        /// <summary>
        /// 添付ファイルメタ情報の操作の詳細
        /// </summary>
        public AttachFileOperation AttachFileOperation { get; } = null;


        public OperationInfo(OperationType type, string attachFileMetaQuerySelectFields = null)
        {
            Type = type;

            if (Type == OperationType.AttachFileMeta)
            {
                AttachFileOperation = new AttachFileOperation(attachFileMetaQuerySelectFields);
            }
        }


        public static bool operator ==(OperationInfo me, object other) => me?.Equals(other) == true;

        public static bool operator !=(OperationInfo me, object other) => !me?.Equals(other) == true;
    }
}