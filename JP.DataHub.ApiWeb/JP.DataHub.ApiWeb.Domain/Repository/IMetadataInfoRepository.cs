using JP.DataHub.ApiWeb.Domain.Context.MetadataInfo;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    /// <summary>
    /// APIのメタデータを取得します。
    /// </summary>
    public interface IMetadataInfoRepository
    {
        /// <summary>
        /// API情報の一覧を取得します。
        /// </summary>
        /// <param name="noChildren">子の情報なしフラグ</param>
        /// <param name="localeCode">ロケール</param>
        /// <param name="isActiveOnly">未削除のみ</param>
        /// <param name="isEnableOnly">有効のみ</param>
        /// <param name="isNotHiddenOnly">公開のみ</param>
        /// <returns>API情報の一覧</returns>
        IList<ApiDescription> GetApiDescription(bool noChildren, string localeCode = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false);

        /// <summary>
        /// スキーマ情報の一覧を取得します。
        /// </summary>
        /// <param name="localeCode">ロケール</param>
        /// <returns>スキーマ情報の一覧</returns>
        IList<SchemaDescription> GetSchemaDescription(string localeCode = null);
    }
}
