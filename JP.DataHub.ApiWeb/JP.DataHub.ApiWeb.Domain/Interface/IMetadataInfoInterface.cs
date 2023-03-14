using System;
using JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    /// <summary>
    /// APIのメタデータを取得します。
    /// </summary>
    public interface IMetadataInfoInterface
    {
        /// <summary>
        /// API情報の一覧を取得します。
        /// </summary>
        /// <param name="noChildren">子の情報なしフラグ</param>
        /// <param name="culture">カルチャ(ロケール)</param>
        /// <param name="isActiveOnly">未削除のみ</param>
        /// <param name="isEnableOnly">有効のみ</param>
        /// <param name="isNotHiddenOnly">公開のみ</param>
        /// <returns>API情報の一覧</returns>
        IEnumerable<ApiDescriptionModel> GetApiDescription(bool noChildren, string culture = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false);

        /// <summary>
        /// スキーマ情報の一覧を取得します。
        /// </summary>
        /// <param name="culture">カルチャ(ロケール)</param>
        /// <returns>スキーマ情報の一覧</returns>
        IEnumerable<SchemaDescriptionModel> GetSchemaDescription(string culture = null);

        /// <summary>
        /// メタデータを作成します。
        /// </summary>
        /// <param name="apis">API情報</param>
        /// <param name="schemas">スキーマ情報</param>
        /// <param name="urlSchemas">URLスキーマ情報</param>
        /// <returns>メタデータ</returns>
        string CreateMetadata(List<ApiDescriptionModel> apis, List<SchemaDescriptionModel> schemas, List<SchemaDescriptionModel> urlSchemas);
    }
}
