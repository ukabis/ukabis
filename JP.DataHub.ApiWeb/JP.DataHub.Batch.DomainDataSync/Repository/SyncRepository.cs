using JP.DataHub.Batch.DomainDataSync.Domain;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Core.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Resolution;

namespace JP.DataHub.Batch.DomainDataSync.Repository
{
    public class SyncRepository : ISyncRepository
    {
        private ILogger Logger { get; }
        private readonly BlobStorageClient blobClient;

        public SyncRepository(ILogger logger,IConfiguration config)
        {
            Logger = logger;
            blobClient = new BlobStorageClient(config.GetValue<string>("DomainDataSyncStorageConnectionStrings"),
                config.GetValue<string>("DomainDataSyncSetting:SyncConfigContainerName"), "/");
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        /// <returns>コンフィグ情報を保持したDomainインスタンス</returns>
        public ISyncEntity Init()
        {
            // app.configの情報でblobからデータ同期コンフィグ取得
            Stream getStream = blobClient.GetStream(UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:SyncConfigFileName"));
            StreamReader reader = new StreamReader(getStream);
            String syncConfigData = reader.ReadToEnd();

            // コンフィグと業務処理を持ったインスタンス
            return DomainDataSyncUnityContainer.Resolve<ISyncEntity>(
                new ParameterOverride("syncConfig", syncConfigData),
                new ParameterOverride("clearCacheIdApiUrl", UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DeleteCacheByIdApiUrl")),
                new ParameterOverride("clearCacheEntityApiUrl", UnityCore.Resolve<IConfiguration>().GetValue<string>("DomainDataSyncSetting:DeleteCacheByEntityApiUrl")),
                new ParameterOverride("logger", this.Logger));
        }

        /// <summary>
        /// データ取得
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="tableName">対象テーブル名</param>
        /// <param name="columnNames">対象テーブルから取得する列名</param>
        /// <returns>同期するデータの列名と値の連想配列</returns>
        public List<Dictionary<string, string>> GetAllData(string connectionString, string tableName, List<string> columnNames)
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // SQL構築
                string sql = $"SELECT { string.Join(",", columnNames) } FROM { tableName }";

                try
                {
                    // SQL実行
                    using (SqlDataReader reader = SetCommand(con, sql, new Dictionary<string, string>()).ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // 使いやすいよう詰め替え
                                Dictionary<string, string> temp = new Dictionary<string, string>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var isValueNull = reader.IsDBNull(i);
                                    temp.Add(reader.GetName(i), isValueNull ? null : reader[reader.GetName(i)]?.ToString());
                                }

                                ret.Add(temp);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw;
                }
            }

            return ret;
        }

        /// <summary>
        /// データ取得
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="sql">コンフィグに定義されたSQL</param>
        /// <returns>同期するデータの列名と値の連想配列</returns>
        public List<Dictionary<string, string>> GetAllDataByConfigSql(string connectionString, string sql)
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                try
                {
                    // SQL実行
                    using (SqlDataReader reader = SetCommand(con, sql, new Dictionary<string, string>()).ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // 使いやすいよう詰め替え
                                Dictionary<string, string> temp = new Dictionary<string, string>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var isValueNull = reader.IsDBNull(i);
                                    temp.Add(reader.GetName(i), isValueNull ? null : reader[reader.GetName(i)]?.ToString());
                                }

                                ret.Add(temp);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw;
                }
            }

            return ret;
        }

        /// <summary>
        /// データ取得
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="tableName">対象テーブル名</param>
        /// <param name="columnNames">対象テーブルから取得する列名</param>
        /// <returns>同期するデータの列名と値の連想配列</returns>
        public Dictionary<string, string> GetData(string connectionString, string tableName, List<string> columnNames, string pkValue)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // SQL構築
                string pkName = GetPrimaryKeyColumnName(con, tableName);
                string sql = $"SELECT { string.Join(",", columnNames) } FROM { tableName } WHERE { pkName } = @PkValue";

                Dictionary<string, string> parameterPair = new Dictionary<string, string>()
                {
                    { "PkValue", pkValue }
                };

                try
                {
                    // SQL実行
                    using (SqlDataReader reader = SetCommand(con, sql, parameterPair).ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // 使いやすいよう詰め替え
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var isValueNull = reader.IsDBNull(i);
                                    ret.Add(reader.GetName(i), isValueNull ? null : reader[reader.GetName(i)]?.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw;
                }
            }

            return ret;
        }

        /// <summary>
        /// SQLを指定してデータ取得
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="tableName">対象テーブル名</param>
        /// <param name="columnNames">対象テーブルから取得する列名</param>
        /// <returns>同期するデータの列名と値の連想配列</returns>
        public Dictionary<string, string> GetDataByConfigSql(string connectionString, string select, string where, string pkValue)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                Dictionary<string, string> parameterPair = new Dictionary<string, string>()
                {
                    { "PkValue", pkValue }
                };

                try
                {
                    // SQL実行
                    using (SqlDataReader reader = SetCommand(con, select + " " + where, parameterPair).ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // 使いやすいよう詰め替え
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var isValueNull = reader.IsDBNull(i);
                                    ret.Add(reader.GetName(i), isValueNull ? null : reader[reader.GetName(i)]?.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw;
                }
            }

            return ret;
        }

        /// <summary>
        /// FKテーブルのデータ取得
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="tableName">対象テーブル名</param>
        /// <param name="columnNames">対象テーブルから取得する列名</param>
        /// <returns>同期するデータの列名と値の連想配列</returns>
        public List<string> GetForeignKeyTableData(string connectionString, string tableName, string foreignKeyTableName, string pkValue)
        {
            List<string> ret = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // SQL構築
                string pkName = GetPrimaryKeyColumnName(con, tableName);
                string foreignKeyTablePkName = GetPrimaryKeyColumnName(con, foreignKeyTableName);
                string sql = $"SELECT { foreignKeyTablePkName } FROM { foreignKeyTableName } WHERE { pkName } = @PkValue";

                Dictionary<string, string> parameterPair = new Dictionary<string, string>()
                {
                    { "PkValue", pkValue }
                };

                try
                {
                    // SQL実行
                    using (SqlDataReader reader = SetCommand(con, sql, parameterPair).ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // 使いやすいよう詰め替え
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var isValueNull = reader.IsDBNull(i);
                                    ret.Add(isValueNull ? null : reader[reader.GetName(i)]?.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw;
                }
            }

            return ret;
        }

        /// <summary>
        /// データ削除
        /// </summary>
        /// <param name="connectionString">同期先DB接続文字列</param>
        /// <param name="tableName">同期先テーブル名</param>
        /// <param name="specificData">同期対象データを特定するためのPK</param>
        /// <returns>削除件数</returns>
        public int Delete(string connectionString, string tableName, string specificData)
        {
            int ret = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 対象テーブルのPK列名取得
                string pkColumnName = GetPrimaryKeyColumnName(con, tableName);

                // SQL設定
                string sql = $"DELETE FROM { tableName } WHERE { pkColumnName } = @PrimaryKey";
                Dictionary<string, string> parameterPair = new Dictionary<string, string>()
                {
                    {"@PrimaryKey", specificData }
                };

                try
                {
                    // 実行
                    ret = SetCommand(con, sql, parameterPair).ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw;
                }
            }

            return ret;
        }

        /// <summary>
        /// データ登録更新
        /// </summary>
        /// <param name="connectionString">同期先DB接続文字列</param>
        /// <param name="tableName">同期先テーブル名</param>
        /// <param name="upsertCollection">同期データ</param>
        /// <param name="specificData">同期対象データを特定するためのPK</param>
        /// <returns></returns>
        public int Merge(string connectionString, string tableName, Dictionary<string, string> upsertCollection)
        {
            int ret = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 対象テーブルのPK列名取得
                string pkColumnName = GetPrimaryKeyColumnName(con, tableName);

                if (upsertCollection.ContainsKey(pkColumnName) == false)
                {
                    throw new Exception("マージ対象の列を特定出来ませんでした。同期コンフィグにpk列が定義されていることを確認してください。");
                }

                // SQL構築
                string sql = $@"MERGE INTO 
                                    { tableName } AS Source 
                                USING 
                                    ( SELECT '{ upsertCollection[pkColumnName] }' AS { pkColumnName } ) AS ConfirmationPK 
                                ON 
                                    (Source.{ pkColumnName } = ConfirmationPK.{ pkColumnName }) 
                                WHEN MATCHED THEN 
                                    UPDATE SET 
                                    {
                                        string.Join(", ", upsertCollection.Select(pair => pair.Key + " = @" + pair.Key + Environment.NewLine))
                                    }
                                WHEN NOT MATCHED THEN 
                                    {
                                        "INSERT (" + string.Join(",", upsertCollection.Select(pair => pair.Key + Environment.NewLine)) +
                                        ") VALUES (@" + string.Join(",@", upsertCollection.Select(pair => pair.Key + Environment.NewLine)) + ")"
                                    };";

                Dictionary<string, string> parameterPair = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> pair in upsertCollection)
                {
                    parameterPair.Add("@" + pair.Key, pair.Value);
                }

                try
                {
                    ret = SetCommand(con, sql, parameterPair).ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw;
                }
            }

            return ret;
        }

        /// <summary>
        /// テーブル名からPKの列名取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tableName">PK取得対象テーブル名</param>
        /// <returns>pk列名</returns>
        private string GetPrimaryKeyColumnName(SqlConnection con, string tableName)
        {
            string pkColumnName = "";

            // 対象テーブルのPK列名取得
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = con;

                string sql = @"SELECT
                                 c.name AS pk_column_name 
                               FROM
                                 sys.indexes AS i 
                                 INNER JOIN sys.index_columns AS ic 
                                   ON i.object_id = ic.object_id 
                                   AND i.index_id = ic.index_id 
                                 INNER JOIN sys.tables AS t 
                                   ON t.object_id = i.object_id 
                                 INNER JOIN sys.columns AS c 
                                   ON ic.object_id = c.object_id 
                                   AND ic.column_id = c.column_id 
                               WHERE
                                 i.is_primary_key = 1 
                                 AND t.name = @TableName";

                Dictionary<string, string> parameterPair = new Dictionary<string, string>()
                {
                    {"@TableName", tableName }
                };

                try
                {
                    using (SqlDataReader reader = SetCommand(con, sql, parameterPair).ExecuteReader())
                    {
                        reader.Read();
                        pkColumnName = reader["pk_column_name"]?.ToString();
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw;
                }
            }

            return pkColumnName;
        }

        /// <summary>
        /// テーブル名からPKの列名取得
        /// </summary>
        /// <param name="connectionString">同期先DB接続文字列</param>
        /// <param name="tableName">PK取得対象テーブル名</param>
        /// <returns>pk列名</returns>
        public string GetPrimaryKeyColumnName(string connectionString, string tableName)
        {
            string pkColumnName = "";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                // 対象テーブルのPK列名取得
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;

                    string sql = @"SELECT
                                 c.name AS pk_column_name 
                               FROM
                                 sys.indexes AS i 
                                 INNER JOIN sys.index_columns AS ic 
                                   ON i.object_id = ic.object_id 
                                   AND i.index_id = ic.index_id 
                                 INNER JOIN sys.tables AS t 
                                   ON t.object_id = i.object_id 
                                 INNER JOIN sys.columns AS c 
                                   ON ic.object_id = c.object_id 
                                   AND ic.column_id = c.column_id 
                               WHERE
                                 i.is_primary_key = 1 
                                 AND t.name = @TableName";

                    Dictionary<string, string> parameterPair = new Dictionary<string, string>()
                {
                    {"@TableName", tableName }
                };

                    try
                    {
                        using (SqlDataReader reader = SetCommand(con, sql, parameterPair).ExecuteReader())
                        {
                            reader.Read();
                            pkColumnName = reader["pk_column_name"]?.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.Message);
                        throw;
                    }
                }
            }

            return pkColumnName;
        }

        /// <summary>
        /// 指定したテーブルの全列名取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tableName">PK取得対象テーブル名</param>
        /// <returns>指定したテーブルの全列名</returns>
        public List<string> GetColumnNames(string connectionString, string tableName)
        {
            List<string> columnNames = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 対象テーブルのPK列名取得
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;

                    string sql = @" SELECT
                                      ORDINAL_POSITION, 
                                      COLUMN_NAME
                                    FROM
                                      INFORMATION_SCHEMA.COLUMNS 
                                    WHERE
                                      TABLE_NAME = @TableName 
                                    ORDER BY
                                      ORDINAL_POSITION";

                    Dictionary<string, string> parameterPair = new Dictionary<string, string>()
                {
                    {"@TableName", tableName }
                };

                    try
                    {
                        using (SqlDataReader reader = SetCommand(con, sql, parameterPair).ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                columnNames.Add(reader["COLUMN_NAME"]?.ToString());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.Message);
                        throw;
                    }
                }
            }

            return columnNames.Where(name => name != null).ToList();
        }

        /// <summary>
        /// SQL実行準備
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="sql">実行SQL</param>
        /// <param name="parameterPair">SQLバインドパラメータ</param>
        /// <returns>実行SqlCommand</returns>
        private SqlCommand SetCommand(SqlConnection con, string sql, Dictionary<string, string> parameterPair)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = con;
            command.CommandText = sql;

            // SQLパラメータ設定
            foreach (KeyValuePair<string, string> pair in parameterPair)
            {
                var value = pair.Value == null ? DBNull.Value : pair.Value as Object;
                command.Parameters.Add(new SqlParameter(pair.Key, value));
            }

            OutputSqlLog(sql, parameterPair);

            return command;
        }

        /// <summary>
        /// 実行SQLログ出力
        /// </summary>
        /// <param name="sql">ログ出力SQL</param>
        /// <param name="parameterPair">SQLバインドパラメータ</param>
        private void OutputSqlLog(string sql, Dictionary<string, string> parameterPair)
        {
            string sqlForLog = sql;

            // SQLパラメータ置換
            foreach (KeyValuePair<string, string> pair in parameterPair)
            {
                sqlForLog = sqlForLog.Replace(pair.Key, pair.Value);
            }

            // 余計な改行、空白を削除してSQLをログ出力
            Logger.LogInformation($"実行SQL={sqlForLog.Replace("  ", "").Replace("\r\n", "")}");
        }
    }
}