using JP.DataHub.ManageApi.Service.DymamicApi;
using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IDataSchemaRepository
    {
        /// <summary>
        /// 指定されたベンダーIDのDynamicAPIのスキーマの一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>
        /// 指定されたベンダーのDynamicAPIのスキーマの一覧。
        /// <paramref name="vendorId"/>がnullの場合は全ベンダーのDynamicAPIのスキーマの一覧。
        /// </returns>
        IEnumerable<SchemaModel> GetSchemas(string vendorId);

        /// <summary>
        /// 指定されたベンダーIDのデータスキーマ情報を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>データスキーマ情報の一覧</returns>
        IEnumerable<DataSchemaInformationModel> GetDataSchemaImformationList(string vendorId);

        /// <summary>
        /// 指定されたスキーマIDのDynamicAPIのスキーマの一件を取得します。
        /// </summary>
        /// <param name="schemaId">スキーマID</param>
        /// <returns>DynamicAPIのスキーマ</returns>
        SchemaModel GetDataSchemaById(string dataSchemaId);

        /// <summary>
        /// 指定されたデータスキーマ名のデータスキーマ情報を取得します。
        /// </summary>
        /// <param name="dataSchemaName">データスキーマ名</param>
        /// <returns>データスキーマ情報</returns>
        SchemaModel GetDataSchemaByName(string dataSchemaName);

        /// <summary>
        /// 指定されたデータスキーマIDのデータスキーマ情報を取得します。
        /// </summary>
        /// <param name="dataSchemaId">データスキーマID</param>
        /// <returns>データスキーマ情報</returns>
        DataSchemaInformationModel GetDataSchemaImformation(string dataSchemaId);

        /// <summary>
        /// データスキーマを取得します。
        /// </summary>
        /// <param name="dataSchemaId">データスキーマID</param>
        /// <returns>データスキーマ</returns>
        DataSchema GetDataSchema(string dataSchemaId);

        /// <summary>
        /// コントローラIDに紐づくデータスキーマIDを取得します。
        /// </summary>
        /// <param name="controllerId"></param>
        /// <returns>データスキーマID</returns>
        string GetDataSchemaIdByControllerId(string controllerId);

        /// <summary>
        /// データスキーマ情報を登録または更新します。
        /// </summary>
        /// <param name="dataSchemaInformation">データスキーマ情報</param>
        /// <returns>データスキーマ情報</returns>
        DataSchemaInformationModel UpsertSchema(DataSchemaInformationModel dataSchemaInformation);

        ///// <summary>
        ///// データスキーマ情報を更新します。
        ///// </summary>
        ///// <param name="dataSchemaInformation">データスキーマ情報</param>
        ///// <returns>データスキーマ情報</returns>
        //DataSchemaInformation UpdateDataSchema(DataSchemaInformation dataSchemaInformation);

        /// <summary>
        /// 指定されたデータスキーマIDのデータスキーマ情報を削除します。
        /// </summary>
        /// <param name="dataSchemaId">データスキーマID</param>
        void DeleteDataSchema(string dataSchemaId);

        /// <summary>
        /// DataSchema名重複チェック
        /// </summary>
        /// <param name="schemaName">スキーマ名</param>
        /// <returns>true:登録済/false:未登録</returns>
        bool ExistsSameSchemaName(string schemaName);

        /// <summary>
        /// 引数のDataSchemeIdを使用しているApiが存在しているか
        /// </summary>
        /// <param name="dataSchemaId">DataSchemaId</param>
        /// <returns>使用している場合はtrue、していない場合はfalseを返す</returns>
        bool IsUsedFromApi(string dataSchemaId);

        /// <summary>
        /// 引数のDataSchemaIdを使用しているControllerが存在しているか
        /// </summary>
        /// <param name="dataSchemaId">DataSchemaId</param>
        /// <returns>使用している場合はtrue、していない場合はfalseを返す</returns>
        bool IsUsedFromController(string dataSchemaId);
    }
}
