using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Resolution;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Web.WebRequest;
using Org.BouncyCastle.Ocsp;
using Polly;
using JP.DataHub.Batch.DomainDataSync.Repository;

namespace JP.DataHub.Batch.DomainDataSync.Domain
{
    public class SyncEntity : ISyncEntity
    {
        /// <summary>
        /// 1ホストに1クライアント
        /// </summary>
        private readonly IDynamicApiClient clientForApiServer = UnityCore.Resolve<IDynamicApiClient>();

        private readonly IDomainDataSyncResource domainDataSyncResource = UnityCore.Resolve<IDomainDataSyncResource>();
        /// <summary>
        /// 環境設定のサーバーURL
        /// </summary>
        private readonly string url = UnityCore.Resolve<IServerEnvironment>().Url;

        /// <summary>
        /// 永続化層へのアクセス手段
        /// </summary>
        private ISyncRepository AccessToOutside { get; }

        /// <summary>
        /// ログ出力する何か
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// 同期イベント名
        /// </summary>
        private string eventName;

        /// <summary>
        /// 同期イベント名設定
        /// これを設定しないとコンフィグからマッピングデータを取得出来ない
        /// </summary>
        private string SetEventName
        {
            set
            {
                eventName = value;
            }
        }

        /// <summary>
        /// 同期元DB、同期先DBのConnectionString
        /// </summary>
        /// <remarks>クラスにしようかと思ったが、コンフィグの定義変える度にソース直すのが面倒だから文字列のまま操作</remarks>
        private JObject DBConnection { get; }
        private enum DBConnectionFields
        {
            SourceDBConnectionString,
            DestinationDBConnectionString
        }

        /// <summary>
        /// 同期元はどれで、同期先はどれなのかの定義
        /// </summary>
        /// <remarks>クラスにしようかと思ったが、コンフィグの定義変える度にソース直すのが面倒だから文字列のまま操作</remarks>
        private JObject SyncMapping { get; }
        private enum SyncMappingFields
        {
            SourceCatalog,
            SourceTable,
            DestinationCatalog,
            DestinationTable,
            DeleteAction,
            ColumnMapping,
            RelationTable,
            GetRelationDataSQL
        }
        private enum ColumnMappingFields
        {
            SourceColumnName,
            DestinationColumnName
        }
        private enum DeleteActionFields
        {
            Physical,
            Logical
        }
        private enum RelationTableFileds
        {
            TableName
        }

        /// <summary>
        /// 列名
        /// </summary>
        /// <remarks>クラスにしようかと思ったが、コンフィグの定義変える度にソース直すのが面倒だから文字列のまま操作</remarks>
        private JObject ColumnNames { get; }
        private enum ColumnNamesFields
        {
            SourceUpdateDateColumnName,
            SourceLogicalDeleteColumnName,
            DestinationUpdateDateColumnName,
            DestinationLogicalDeleteColumnName
        }

        /// <summary>
        /// データ同期に伴い削除すべきキャッシュのKey名
        /// </summary>
        /// <remarks>クラスにしようかと思ったが、コンフィグの定義変える度にソース直すのが面倒だから文字列のまま操作</remarks>
        private JObject DeleteCacheKey { get; }
        private enum DeleteCacheKeyFields
        {
            Id,
            Entity,
            DeleteTargetName
        }

        /// <summary>
        /// IDによるキャッシュ削除ApiのURL
        /// </summary>
        private string ClearCacheByIdApiUrl { get; }

        /// <summary>
        /// Entity名によるキャッシュ削除ApiのURL
        /// </summary>
        private string ClearCacheByEntityApiUrl { get; }

        /// <summary>
        /// データ同期コンフィグ解析、ログ出力準備
        /// </summary>
        /// <param name="syncConfig">データ同期コンフィグの定義内容</param>
        /// <param name="logger">ログ出力するための何か</param>
        public SyncEntity(string syncConfig, string clearCacheIdApiUrl, string clearCacheEntityApiUrl, ILogger logger)
        {
            // 同期コンフィグ解析
            JObject all = JObject.Parse(syncConfig);
            DBConnection = all["DBConnection"] as JObject;
            SyncMapping = all["SyncMapping"] as JObject;
            ColumnNames = all["ColumnNames"] as JObject;
            DeleteCacheKey = all["DeleteCacheKey"] as JObject;

            // ApiURL
            this.ClearCacheByIdApiUrl = clearCacheIdApiUrl;
            this.ClearCacheByEntityApiUrl = clearCacheEntityApiUrl;

            // ログ出力準備
            Logger = logger;

            // 永続化層へのアクセス準備
            AccessToOutside = DomainDataSyncUnityContainer.Resolve<ISyncRepository>(new ParameterOverride("logger", Logger));
        }

        /// <summary>
        /// 同期に必要な情報がコンフィグに定義されているか確認
        /// </summary>
        /// <param name="eventName">連携されてきたイベント名</param>
        /// <returns>コンフィグが同期可能な状態か</returns>
        private bool CheckConfigDefinition(string eventName)
        {
            // DBConnection 定義確認
            if (DBConnection == null)
            {
                throw new Exception($"{ nameof(DBConnection) }が定義されていません。");
            }

            if (DBConnection[DBConnectionFields.SourceDBConnectionString.ToString()] == null)
            {
                throw new Exception($"{ nameof(DBConnection) }.{ DBConnectionFields.SourceDBConnectionString.ToString() }が定義されていません。");
            }

            if (DBConnection[DBConnectionFields.DestinationDBConnectionString.ToString()] == null)
            {
                throw new Exception($"{ nameof(DBConnection) }.{ DBConnectionFields.DestinationDBConnectionString.ToString() } が定義されていません。");
            }

            // SyncMapping 定義確認
            if (SyncMapping == null)
            {
                throw new Exception($"{ nameof(SyncMapping) }が定義されていません。");
            }

            if (SyncMapping[eventName] == null)
            {
                return false;
            }

            // 同期元と同期先が1:Nの場合
            if (SyncMapping[eventName] is JArray)
            {
                for (int i = 0; i < SyncMapping[eventName].Count(); i++)
                {
                    var item = SyncMapping[eventName][i];
                    var msgCnt = i + 1;

                    if (item[SyncMappingFields.SourceCatalog.ToString()] == null)
                    {
                        throw new Exception($"{ nameof(SyncMapping) }.{ eventName }の{ msgCnt }個目の{ SyncMappingFields.SourceCatalog.ToString() }が定義されていません。");
                    }

                    if (item[SyncMappingFields.DestinationCatalog.ToString()] == null)
                    {
                        throw new Exception($"{ nameof(SyncMapping) }.{ eventName }の{ msgCnt }個目の{ SyncMappingFields.DestinationCatalog.ToString() }が定義されていません。");
                    }

                    if (item[SyncMappingFields.SourceTable.ToString()] == null)
                    {
                        throw new Exception($"{ nameof(SyncMapping) }.{ eventName }の{ msgCnt }個目の{ SyncMappingFields.SourceTable.ToString() }が定義されていません。");
                    }

                    if (item[SyncMappingFields.DestinationTable.ToString()] == null)
                    {
                        throw new Exception($"{ nameof(SyncMapping) }.{ eventName }の{ msgCnt }個目の{ SyncMappingFields.DestinationTable.ToString() }が定義されていません。");
                    }

                    if (item[SyncMappingFields.DeleteAction.ToString()] == null)
                    {
                        throw new Exception($"{ nameof(SyncMapping) }.{ eventName }の{ msgCnt }個目の{ SyncMappingFields.DeleteAction.ToString() }が定義されていません。");
                    }

                    foreach (JToken mappingDefinition in item[SyncMappingFields.ColumnMapping.ToString()].ToList())
                    {
                        if (mappingDefinition[ColumnMappingFields.SourceColumnName.ToString()] == null)
                        {
                            throw new Exception($"{ nameof(SyncMapping) }.{ ColumnMappingFields.SourceColumnName.ToString() }が定義されていません。");
                        }

                        if (mappingDefinition[ColumnMappingFields.DestinationColumnName.ToString()] == null)
                        {
                            throw new Exception($"{ nameof(SyncMapping) }.{ ColumnMappingFields.DestinationColumnName.ToString() }が定義されていません。");
                        }
                    }
                }
            }
            // 同期元と同期先が1:1の場合
            else
            {
                if (SyncMapping[eventName][SyncMappingFields.SourceCatalog.ToString()] == null)
                {
                    throw new Exception($"{ nameof(SyncMapping) }.{ eventName }.{ SyncMappingFields.SourceCatalog.ToString() }が定義されていません。");
                }

                if (SyncMapping[eventName][SyncMappingFields.DestinationCatalog.ToString()] == null)
                {
                    throw new Exception($"{ nameof(SyncMapping) }.{ eventName }.{ SyncMappingFields.DestinationCatalog.ToString() }が定義されていません。");
                }

                if (SyncMapping[eventName][SyncMappingFields.SourceTable.ToString()] == null)
                {
                    throw new Exception($"{ nameof(SyncMapping) }.{ eventName }.{ SyncMappingFields.SourceTable.ToString() }が定義されていません。");
                }

                if (SyncMapping[eventName][SyncMappingFields.DestinationTable.ToString()] == null)
                {
                    throw new Exception($"{ nameof(SyncMapping) }.{ eventName }.{ SyncMappingFields.DestinationTable.ToString() }が定義されていません。");
                }

                if (SyncMapping[eventName][SyncMappingFields.DeleteAction.ToString()] == null)
                {
                    throw new Exception($"{ nameof(SyncMapping) }.{ eventName }.{ SyncMappingFields.DeleteAction.ToString() }が定義されていません。");
                }

                foreach (JToken mappingDefinition in SyncMapping[eventName][SyncMappingFields.ColumnMapping.ToString()].ToList())
                {
                    if (mappingDefinition[ColumnMappingFields.SourceColumnName.ToString()] == null)
                    {
                        throw new Exception($"{ nameof(SyncMapping) }.{ ColumnMappingFields.SourceColumnName.ToString() }が定義されていません。");
                    }

                    if (mappingDefinition[ColumnMappingFields.DestinationColumnName.ToString()] == null)
                    {
                        throw new Exception($"{ nameof(SyncMapping) }.{ ColumnMappingFields.DestinationColumnName.ToString() }が定義されていません。");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Topic連携分データ同期
        /// </summary>
        /// <param name="eventName">管理画面側から送信されてきたイベント名</param>
        public int Sync(string eventName, string pkValue)
        {
            SetEventName = eventName;

            // コンフィグ定義確認
            if (this.CheckConfigDefinition(eventName) == false)
            {
                Logger.LogInformation($"{ nameof(SyncMapping) }.{ eventName }が定義されていないため、同期処理は行いません。");
                return 0;
            }
            int registerCount = 0;

            if (SyncMapping[eventName] is JArray)
            {
                // 同期先が配列、1:Nの場合
                var array = SyncMapping[eventName] as JArray;
                foreach (var item in array)
                {
                    registerCount += ExecuteSync(item, pkValue);
                }
            }
            else
            {
                // 同期先が配列でない、1:1の場合
                registerCount = ExecuteSync(SyncMapping[eventName], pkValue);
            }
            return registerCount;
        }

        private int ExecuteSync(JToken item, string pkValue)
        {
            int registerCount = 0;
            #region 同期元変数

            // 同期元対象列
            List<string> sourceColumnNames = item[SyncMappingFields.ColumnMapping.ToString()]
                    .Select(map => map[ColumnMappingFields.SourceColumnName.ToString()]?.ToString())
                    .Where(map => map != null)
                    .ToList();
            // 同期元DB
            string souceDb = string.Format(DBConnection[DBConnectionFields.SourceDBConnectionString.ToString()].ToString(), item[SyncMappingFields.SourceCatalog.ToString()].ToString());
            // 同期先テーブル
            string sourceTbl = item[SyncMappingFields.SourceTable.ToString()].ToString();

            #endregion

            #region 同期先変数

            // 同期先対象列
            List<string> destinationColumnNames = item[SyncMappingFields.ColumnMapping.ToString()]
                    .Select(map => map[ColumnMappingFields.DestinationColumnName.ToString()]?.ToString())
                    .Where(map => map != null)
                    .ToList();
            // 同期先DB
            string destinationDb = string.Format(DBConnection[DBConnectionFields.DestinationDBConnectionString.ToString()].ToString(), item[SyncMappingFields.DestinationCatalog.ToString()].ToString());
            // 同期先テーブル
            string destinationTbl = this.GetDestinationTableName(item);

            #endregion

            // 削除処理(物理削除 or 論理削除)
            string deleteAction = item[SyncMappingFields.DeleteAction.ToString()]?.ToString();

            // 同期対象列名の定義は省略可
            if (sourceColumnNames.Count == 0)
            {
                // 省略されている場合は全列を同期
                sourceColumnNames = AccessToOutside.GetColumnNames(souceDb, sourceTbl);
                destinationColumnNames = AccessToOutside.GetColumnNames(destinationDb, this.GetDestinationTableName(item));
            }

            if (sourceColumnNames.Count == 0)
            {
                throw new Exception("同期対象列の取得に失敗しました。");
            }

            Dictionary<string, string> sourceData;
            if (string.IsNullOrEmpty(item[SyncMappingFields.GetRelationDataSQL.ToString()]?["Select"]?.ToString()))
            {
                sourceData = AccessToOutside.GetData(souceDb, sourceTbl, sourceColumnNames, pkValue);
            }
            else
            {
                sourceData = AccessToOutside.GetDataByConfigSql(
                                souceDb,
                                item[SyncMappingFields.GetRelationDataSQL.ToString()]["Select"]?.ToString(),
                                item[SyncMappingFields.GetRelationDataSQL.ToString()]["Where"]?.ToString(),
                                pkValue);
            }

            // 同期
            if (sourceData.Count == 0)
            {
                Logger.LogInformation($"同期元データなし。{ nameof(SyncMapping) }.{ eventName },sourceTbl={sourceTbl},pkValue={pkValue}");
                // 同期元無しで、同期先は論理削除する設定
                if (deleteAction == DeleteActionFields.Logical.ToString())
                {
                    if (destinationColumnNames.Contains(this.GetDestinationLogicalDeleteColumnName(item)))
                    {
                        registerCount = SyncMerge(sourceData, destinationColumnNames, destinationDb, destinationTbl, item, pkValue, sourceColumnNames);
                    }
                }
                // 同期元無しで、同期先は物理削除する設定
                else if (deleteAction == DeleteActionFields.Physical.ToString())
                {
                    DeleteForeignKeyTableData(pkValue, item, souceDb, sourceTbl, destinationDb);
                    registerCount = SyncDelete(pkValue, destinationDb, destinationTbl);
                }
            }
            else
            {
                // 同期元が論理削除された
                if (sourceData.ContainsKey(GetSourceLogicalDeleteColumnName(item)) && sourceData[GetSourceLogicalDeleteColumnName(item)].ToString() == "1")
                {
                    // 同期先は物理削除する設定
                    if (deleteAction == DeleteActionFields.Physical.ToString())
                    {
                        DeleteForeignKeyTableData(pkValue, item, souceDb, sourceTbl, destinationDb);
                        registerCount = SyncDelete(pkValue, destinationDb, destinationTbl);
                    }
                    // 同期先も論理削除する設定
                    else if (destinationColumnNames.Contains(this.GetDestinationLogicalDeleteColumnName(item)))
                    {
                        registerCount = SyncMerge(sourceData, destinationColumnNames, destinationDb, destinationTbl, item);
                    }
                    // 論理削除フラグが無いなら同期の必要なし（何も更新しないので無意味）
                }
                // 同期元が更新された
                else
                {
                    registerCount = SyncMerge(sourceData, destinationColumnNames, destinationDb, destinationTbl, item);
                }
            }
            return registerCount;
        }

        /// <summary>
        /// 同期先のテーブル名を取得する。取得できない場合は同期元のテーブル名を返す。
        /// </summary>
        /// <param name="item">同期情報</param>
        /// <returns>テーブル名</returns>
        private string GetDestinationTableName(JToken item)
        {
            string destinationTableName = item[SyncMappingFields.DestinationTable.ToString()]?.ToString();

            if (string.IsNullOrEmpty(destinationTableName))
            {
                return item[SyncMappingFields.SourceTable.ToString()].ToString();
            }
            else
            {
                return destinationTableName;
            }
        }

        /// <summary>
        /// 同期先の論理削除フラグ列名
        /// </summary>
        private string GetDestinationLogicalDeleteColumnName(JToken item)
        {
            if (ColumnNames == null ||
                ColumnNames[eventName] == null ||
                item[ColumnNamesFields.DestinationLogicalDeleteColumnName.ToString()] == null)
            {
                if (string.IsNullOrEmpty(UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DefaultLogicalDeleteColumnName")))
                {
                    throw new Exception("DomainDataSyncSetting:DefaultLogicalDeleteColumnNameの定義がありません。");
                }
                else
                {
                    return UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DefaultLogicalDeleteColumnName");
                }
            }
            else
            {
                return item[ColumnNamesFields.DestinationLogicalDeleteColumnName.ToString()].ToString();
            }
        }

        /// <summary>
        /// 同期元の論理削除フラグ列名
        /// </summary>
        private string GetSourceLogicalDeleteColumnName(JToken item)
        {
            if (ColumnNames == null ||
                ColumnNames[eventName] == null ||
                item[ColumnNamesFields.SourceLogicalDeleteColumnName.ToString()] == null)
            {
                if (string.IsNullOrEmpty(UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DefaultLogicalDeleteColumnName")))
                {
                    throw new Exception("コンフィグにDomainDataSyncSetting:DefaultLogicalDeleteColumnNameの定義がありません。");
                }
                else
                {
                    return UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DefaultLogicalDeleteColumnName");
                }
            }
            else
            {
                return item[ColumnNamesFields.SourceLogicalDeleteColumnName.ToString()].ToString();
            }
        }

        private List<string> GetRelationTableNames(JToken item)
        {
            try
            {
                return item[SyncMappingFields.RelationTable.ToString()]
                    .Select(map => map[RelationTableFileds.TableName.ToString()]?.ToString())
                    .Where(map => map != null)
                    .ToList();
            }
            catch (Exception)
            {
                throw new Exception($"{ nameof(SyncMapping) }.{ eventName }.{ SyncMappingFields.RelationTable.ToString() }が定義されていません。");
            }
        }

        /// <summary>
        /// 外部キー制約の関連テーブルデータ削除
        /// </summary>
        /// <param name="foreignKey">外部キー</param>
        private void DeleteForeignKeyTableData(string foreignKey, JToken item, string sourceDBCon, string sourceTbl, string destinationDbCon)
        {
            foreach (string relationTableName in GetRelationTableNames(item))
            {
                List<string> relationTablePkValues = AccessToOutside.GetForeignKeyTableData(sourceDBCon, sourceTbl, relationTableName, foreignKey);

                foreach (string relationTablePkValue in relationTablePkValues)
                {
                    AccessToOutside.Delete(destinationDbCon, relationTableName, relationTablePkValue);
                }
            }
        }

        /// <summary>
        /// 同期元にあり、同期先に無いデータを同期（登録更新）
        /// </summary>
        /// <param name="sourceData">同期データ</param>
        /// <param name="destinationColumnNames">同期先列名</param>
        /// <param name="destinationDbCon">同期先接続先</param>
        /// <param name="destinationTbl">同期先テーブル</param>
        /// <param name="item">同期情報</param>
        /// <returns></returns>
        private int SyncMerge(Dictionary<string, string> sourceData, List<string> destinationColumnNames, string destinationDbCon, string destinationTbl, JToken item, string pkValue = null, List<string> sourceColumnNames = null)
        {
            // 同期先の列に同期元のデータを設定
            Dictionary<string, string> upsertDataCollection = new Dictionary<string, string>();

            foreach (JObject map in item[SyncMappingFields.ColumnMapping.ToString()])
            {
                upsertDataCollection.Add(
                    map[ColumnMappingFields.DestinationColumnName.ToString()]?.ToString(),
                    sourceData.Count == 0 ? pkValue : sourceData[map[ColumnMappingFields.SourceColumnName.ToString()]?.ToString()]?.ToString());
            }

            if (upsertDataCollection.Count == 0)
            {
                foreach (string columnName in destinationColumnNames)
                {
                    if ((sourceData.Count != 0 && !sourceData.ContainsKey(columnName)) || sourceColumnNames?.Contains(columnName) == false)
                    {
                        throw new Exception($"columnName:{columnName}が存在しません。");
                    }

                    upsertDataCollection.Add(columnName, sourceData.Count == 0 ? columnName : sourceData[columnName]?.ToString());
                }
            }

            // 登録更新
            return AccessToOutside.Merge(destinationDbCon, destinationTbl, upsertDataCollection);
        }

        /// <summary>
        /// 同期元に無く、同期先にあるデータを同期（削除）
        /// </summary>
        /// <param name="destinationDbConnectionString">同期先DB接続文字列</param>
        /// <param name="allSourceData">同期元全データ</param>
        /// <param name="sourceTableName">同期元テーブル名</param>
        /// <param name="allDestinationData">同期先全データ</param>
        /// <param name="destinationTableName">同期先テーブル名</param>
        private int SyncDelete(string pkValue, string destinationDbCon, string destinationTbl)
        {
            return AccessToOutside.Delete(destinationDbCon, destinationTbl, pkValue);
        }

        /// <summary>
        /// 定期実行全データ同期
        /// </summary>
        public bool SyncAll(bool ignoreUpdateDate)
        {
            foreach (KeyValuePair<string, JToken> eventDefinition in SyncMapping)
            {
                SetEventName = eventDefinition.Key;

                // コンフィグ定義確認
                this.CheckConfigDefinition(eventDefinition.Key);

                // 1:Nの同期
                if (eventDefinition.Value is JArray)
                {
                    foreach (var val in eventDefinition.Value)
                    {
                        this.SyncTable(val, ignoreUpdateDate);
                    }
                }
                // 1:1の場合
                else
                {
                    this.SyncTable(eventDefinition.Value, ignoreUpdateDate);
                }
            }

            return true;
        }

        private bool SyncTable(JToken item, bool ignoreUpdateDate)
        {
            #region 同期元変数

            // 同期元対象列
            List<string> sourceColumnNames = item[SyncMappingFields.ColumnMapping.ToString()]
                    .Select(map => map[ColumnMappingFields.SourceColumnName.ToString()]?.ToString())
                    .Where(map => map != null)
                    .ToList();
            // 同期元DB
            string sourceDb = string.Format(DBConnection[DBConnectionFields.SourceDBConnectionString.ToString()].ToString(), item[SyncMappingFields.SourceCatalog.ToString()].ToString());
            // 同期先テーブル
            string sourceTbl = item[SyncMappingFields.SourceTable.ToString()].ToString();

            #endregion

            #region 同期先変数

            // 同期先対象列
            List<string> destinationColumnNames = item[SyncMappingFields.ColumnMapping.ToString()]
                    .Select(map => map[ColumnMappingFields.DestinationColumnName.ToString()]?.ToString())
                    .Where(map => map != null)
                    .ToList();
            // 同期先DB
            string destinationDb = string.Format(DBConnection[DBConnectionFields.DestinationDBConnectionString.ToString()].ToString(), item[SyncMappingFields.DestinationCatalog.ToString()].ToString());
            // 同期先テーブル
            string destinationTbl = this.GetDestinationTableName(item);

            #endregion

            // 削除処理(物理削除 or 論理削除)
            string deleteAction = item[SyncMappingFields.DeleteAction.ToString()]?.ToString();

            // 定義は省略可
            if (sourceColumnNames.Count == 0)
            {
                // 省略されている場合は全列を同期
                sourceColumnNames = AccessToOutside.GetColumnNames(sourceDb, sourceTbl);
                destinationColumnNames = AccessToOutside.GetColumnNames(destinationDb, destinationTbl);
            }

            if (sourceColumnNames.Count == 0)
            {
                throw new Exception("同期対象列の取得に失敗しました。");
            }

            List<Dictionary<string, string>> allSourceData;

            if (string.IsNullOrEmpty(item[SyncMappingFields.GetRelationDataSQL.ToString()]?["Select"]?.ToString()))
            {
                allSourceData = AccessToOutside.GetAllData(sourceDb, sourceTbl, sourceColumnNames);
            }
            else
            {
                allSourceData = AccessToOutside.GetAllDataByConfigSql(sourceDb, item[SyncMappingFields.GetRelationDataSQL.ToString()]["Select"].ToString());
            }

            // 同期データ取得
            List<Dictionary<string, string>> allDestinationData = AccessToOutside.GetAllData(destinationDb, destinationTbl, destinationColumnNames);

            // 更新日付の比較を行うかどうか(同期元と同期先の両方が更新日付を持っているか)
            bool shouldCompareUpdDate = !ignoreUpdateDate && sourceColumnNames.Contains(GetSourceUpdateDateColumnName(item)) && destinationColumnNames.Contains(GetDestinationUpdateDateColumnName(item));

            // PK列名
            string sourceTablePkName = AccessToOutside.GetPrimaryKeyColumnName(sourceDb, sourceTbl);
            string destinationTablePkName = AccessToOutside.GetPrimaryKeyColumnName(destinationDb, destinationTbl);

            int upsertCount = 0;

            // 同期元に存在するが同期先に存在しないデータは登録、同期元も同期先も存在するデータは更新日付が異なる場合のみ更新
            foreach (Dictionary<string, string> sourceData in allSourceData)
            {
                if (shouldCompareUpdDate)
                {
                    // 現在処理中の同期元データのPKと一致する同期先データ
                    Dictionary<string, string> matchDestData = allDestinationData.Where(destData => destData[destinationTablePkName] == sourceData[sourceTablePkName]).FirstOrDefault();

                    // 更新日付が同じなら同期不要
                    if (matchDestData != null && sourceData[GetSourceUpdateDateColumnName(item)] == matchDestData[GetDestinationUpdateDateColumnName(item)])
                    {
                        continue;
                    }
                }

                upsertCount += SyncMerge(sourceData, destinationColumnNames, destinationDb, destinationTbl, item, null, null);
            }

            int deleteCount = 0;

            // 同期元無しで、同期先は物理削除する設定
            if (deleteAction == DeleteActionFields.Physical.ToString())
            {
                List<string> sourceDataPkValues = allSourceData.Select(data => data[sourceTablePkName]).ToList();
                List<string> destinationDataPkValues = allDestinationData.Select(data => data[destinationTablePkName]).ToList();

                // 削除対象データ特定
                HashSet<string> hs = new HashSet<string>(sourceDataPkValues);
                List<string> deletePkValues = destinationDataPkValues.Where(pk => !hs.Contains(pk)).ToList();

                // 同期先に存在するが同期元に存在しないデータ
                foreach (string pkValue in deletePkValues)
                {
                    deleteCount += AccessToOutside.Delete(destinationDb, destinationTbl, pkValue);
                }
            }

            Logger.LogInformation($"イベント名：{ eventName }　更新登録：{ upsertCount }　削除：{ deleteCount }");

            return true;
        }

        /// <summary>
        /// 同期元の更新日付列名
        /// </summary>
        private string GetSourceUpdateDateColumnName(JToken item)
        {
            if (ColumnNames == null ||
                item == null ||
                item[ColumnNamesFields.SourceUpdateDateColumnName.ToString()] == null)
            {
                if (string.IsNullOrEmpty(UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DefaultUpdateDateColumnName")))
                {
                    throw new Exception("コンフィグにDomainDataSyncSetting:DefaultUpdateDateColumnNameの定義がありません。");
                }
                else
                {
                    return UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DefaultUpdateDateColumnName");
                }
            }
            else
            {
                return item[ColumnNamesFields.SourceUpdateDateColumnName.ToString()].ToString();
            }
        }

        /// <summary>
        /// 同期先の更新日付列名
        /// </summary>
        private string GetDestinationUpdateDateColumnName(JToken item)
        {
            if (ColumnNames == null ||
                item == null ||
                item[ColumnNamesFields.DestinationUpdateDateColumnName.ToString()] == null)
            {
                if (string.IsNullOrEmpty(UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DefaultUpdateDateColumnName")))
                {
                    throw new Exception("コンフィグにDomainDataSyncSetting:DefaultUpdateDateColumnNameの定義がありません。");
                }
                else
                {
                    return UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DefaultUpdateDateColumnName");
                }
            }
            else
            {
                return item[ColumnNamesFields.DestinationUpdateDateColumnName.ToString()].ToString();
            }
        }

        /// <summary>
        /// コンフィグのキャッシュ削除定義確認
        /// </summary>
        /// <param name="eventName">管理画面側から送信されてきたイベント名</param>
        private bool CheckDeleteCacheDefinition(string eventName)
        {
            if (DeleteCacheKey == null)
            {
                throw new Exception($"{ nameof(DeleteCacheKey) }が定義されていません。");
            }

            if (DeleteCacheKey[eventName] == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// キャッシュ削除
        /// </summary>
        /// <returns>Api実行のHttpステータス</returns>
        public async Task ClearCache(string eventName)
        {
            if (CheckDeleteCacheDefinition(eventName) == false)
            {
                return;
            }

            if (DeleteCacheKey[eventName][DeleteCacheKeyFields.Id.ToString()] != null)
            {
                await ClearCacheById(
                        DeleteCacheKey[eventName][DeleteCacheKeyFields.Id.ToString()]
                            .Select(id => id[DeleteCacheKeyFields.DeleteTargetName.ToString()]?.ToString())
                            .Where(id => id != null)
                            .ToArray());
            }

            if (DeleteCacheKey[eventName][DeleteCacheKeyFields.Entity.ToString()] != null)
            {
                await ClearCacheByEntity(
                        DeleteCacheKey[eventName][DeleteCacheKeyFields.Entity.ToString()]
                            .Select(entity => entity[DeleteCacheKeyFields.DeleteTargetName.ToString()]?.ToString())
                            .Where(entity => entity != null)
                            .ToArray());
            }
        }

        /// <summary>
        /// 全キャッシュ削除
        /// </summary>
        /// <returns>Api実行のHttpステータス</returns>
        public async Task ClearCacheAll()
        {
            foreach (KeyValuePair<string, JToken> pair in DeleteCacheKey)
            {
                await ClearCache(pair.Key);
            }
        }

        /// <summary>
        /// Id指定でキャッシュ削除
        /// </summary>
        /// <param name="ids">削除対象キャッシュID</param>
        /// <returns>task</returns>
        private async Task ClearCacheById(params string[] ids)
        {
            try
            {
                foreach (string id in ids)
                {
                    var ret = clientForApiServer.GetWebApiResponseResult(domainDataSyncResource.ClearById(id));
                    if (!ret.IsSuccessStatusCode && ret.StatusCode != System.Net.HttpStatusCode.NotFound)
                        throw new Exception(ret.ContentString);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Entity指定でキャッシュ削除
        /// </summary>
        /// <param name="entities">削除対象キャッシュエンティティ名</param>
        /// <returns>task</returns>
        private async Task ClearCacheByEntity(params string[] entities)
        {
            try
            {
                foreach (string entity in entities)
                {
                    var ret = clientForApiServer.GetWebApiResponseResult(domainDataSyncResource.ClearByEntity(entity));
                    if (!ret.IsSuccessStatusCode && ret.StatusCode != System.Net.HttpStatusCode.NotFound)
                        throw new Exception(ret.ContentString);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
                throw;
            }
        }
    }
}