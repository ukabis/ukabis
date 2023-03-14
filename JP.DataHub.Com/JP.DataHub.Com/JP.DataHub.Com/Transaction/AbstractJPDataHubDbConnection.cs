using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Reflection;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Dapper;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Profiler;

namespace JP.DataHub.Com.Transaction
{
    public abstract class AbstractJPDataHubDbConnection : IJPDataHubDbConnection, IWantTransaction
    {
        /// <summary>
        /// SingleとしてのQueryに対してデータが取得できなかった場合にNotFoundExceptionをスローするか？
        /// true : QuerySingleで取得できなかったのでNotFoundExceptionをスローする
        /// false : QuerySingleで取得できなくてもNotFoundExceptionをスローしない（nullを返す）
        /// </summary>
        public static bool IsQuerySingleFailThrowNotFoundException { get; set; } = false;

        #region Transaction
        private Lazy<IJPDataHubTransactionManager> _lazyTransactionManager = new Lazy<IJPDataHubTransactionManager>(() => UnityCore.Resolve<IJPDataHubTransactionManager>());
        private Lazy<IJPDataHubTransactionManager> _multiThreadTransactionManager = new Lazy<IJPDataHubTransactionManager>(() => UnityCore.Resolve<IJPDataHubTransactionManager>("Multithread"));
        private IJPDataHubTransactionManager _transactionManager { get => IsMultithread ? _multiThreadTransactionManager.Value : _lazyTransactionManager.Value; }

        protected bool IsMultithread { get; set; } = false;

        /// <summary>
        /// トランザクションの管理をするか
        /// </summary>
        public bool IsTransactionManage { get; protected set; } = false;

        /// <summary>
        /// データベースプロバイダー名
        /// </summary>
        public string ProviderName { get; protected set; }

        /// <summary>
        /// データベースのタイプ名
        /// </summary>
        public string DbType { get; protected set; }

        /// <summary>
        /// トランザクションの状態
        /// </summary>
        public virtual bool IsTransactionStatus { get => dbConnection?.State != ConnectionState.Closed && dbConnection?.State != ConnectionState.Broken; }

        #endregion

        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(AbstractJPDataHubDbConnection));

        protected void NotifyTransaction() => _transactionManager.Connected(this);

        public IDbConnection Connection
        {
            get
            {
                // Transaction処理中のみコネクションをOpenする
                if (IsTransactionScope == true)
                {
                    if (System.Transactions.Transaction.Current?.TransactionInformation.Status == TransactionStatus.Active)
                    {
                        Open();
                    }
                }
                else
                {
                    Open();
                }
                return dbConnection;
            }
        }

        private DbConnection dbConnection;
        private DbTransaction dbTransaction;
        private bool IsTransactionScope = false;
        private readonly object obj = new object();

        public AbstractJPDataHubDbConnection()
        {
        }

        public AbstractJPDataHubDbConnection(JPDataHubDbConnectionParam param)
        {
            Init(param);
        }

        public AbstractJPDataHubDbConnection(string connectionString, string providerName, bool isMultithread = false)
        {
            Init(new JPDataHubDbConnectionParam() { ConnectionString = connectionString, ProviderName = providerName, IsMultithread = isMultithread });
        }

        private void Init(JPDataHubDbConnectionParam param)
        {
            if (param == null)
            {
                throw new NullReferenceException("JPDataHubDbConnectionParam");
            }
            if (string.IsNullOrEmpty(param.ConnectionString))
            {
                throw new NullReferenceException("connectionString");
            }
            if (string.IsNullOrEmpty(param.ProviderName))
            {
                throw new NullReferenceException("providerName");
            }

            var configs = UnityCore.ResolveOrDefault<IOptions<List<DbProviderFactoriesConfig>>>();
            var config = configs?.Value?.Where(x => x.Invariant == param.ProviderName)?.FirstOrDefault();
            DbType = config?.DbType;

            var dbpf = DbProviderFactories.GetFactory(param.ProviderName);
            ProviderName = param.ProviderName;
            dbConnection = dbpf.CreateConnection();
            dbConnection.ConnectionString = param.ConnectionString;
            IsMultithread = param.IsMultithread;

            UnityCore.ResolveOrDefault<IJPDataHubDbConnectionInitialize>(DbType)?.Init(this, param, dbConnection, config);

            // トランザクションの設定
            if (param.IsTransactionManage == true)
            {
                IsTransactionManage = true;
            }
            if (param.IsTransactionScope == true)
            {
                IsTransactionScope = true;
            }

            var configuration = UnityCore.ResolveOrDefault<IConfiguration>();
            var profilerParam = new List<IDbProfiler>();
            if (configuration?.GetSection("Profiler")?.GetValue<bool>("UseProfiler") == true)
            {
                profilerParam.Add(new MiniProfilerDbProfilerWrapper());
            }
            if (configuration?.GetSection("Profiler")?.GetValue<bool>("OutputSqlLog") == true)
            {
                profilerParam.Add(new TraceDbLogger());
            }
            if (profilerParam.Count > 0)
            {
                var profiler = new CompositeDbProfiler(profilerParam.ToArray());
                dbConnection = new ProfiledDbConnection(Connection as DbConnection, profiler);
            }
        }

        public void Open()
        {
            lock (obj)
            {
                if (dbConnection.State == ConnectionState.Closed || dbConnection.State == ConnectionState.Broken)
                {
                    dbConnection.Open();
                    if (IsTransactionScope == false)
                    {
                        dbTransaction = dbConnection.BeginTransaction();
                    }
                    NotifyTransaction();
                }
            }
        }

        public void Close()
        {
            if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
            {
                if (dbTransaction != null)
                {
                    dbTransaction.Commit();
                }
                dbConnection.Close();
            }
        }

        public void Rollback()
        {
            if (dbTransaction != null)
            {
                dbTransaction.Rollback();
                // ロールバックした後にもトランザクションを開始する
                // この理由は、Rollbackした後に、再度DB操作を行われる可能性を考えて（通常ではありえないが、念のため）
                // もしトランザクションを開始していないと、そのDB操作はトランザクションがかからず、即座にコミットされたことと同じになってしまうため
                dbTransaction = dbConnection.BeginTransaction();
            }
        }

        #region Query

        public IEnumerable<T> Query<T>(string sql) => Connection.Query<T>(sql);

        public IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) => Connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);

        public IEnumerable<T> Query<T>(string sql, object param = null) => Connection.Query<T>(sql, param);

        public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) => Connection.Query<TFirst, TSecond, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        public T QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var result = Connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
            if (IsQuerySingleFailThrowNotFoundException == true)
            {
                if (result == null)
                {
                    throw new NotFoundException();
                }
            }
            return result;
        }

        public T QueryPrimaryKey<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var result = Connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
            if (result == null)
            {
                throw new NotFoundException();
            }
            return result;
        }

        public T QuerySingleOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => Connection.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType);

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) => Connection.Query<TFirst, TSecond, TThird, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        public T Get<T>(object id, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.Get<T>(Connection, id, transaction, commandTimeout);

        public IEnumerable<T> GetList<T>(object whereConditions, string additionalCondition, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var whereprops = whereConditions != null ?
                whereConditions.GetType().GetProperties() : new PropertyInfo[0];

            // SQL文の組み立て
            var sb = new StringBuilder("select ");
            BuildSelect(sb, typeof(T).GetProperties().Where(p => p.PropertyType.IsSimpleType()));
            sb.Append(" from ").Append(GetTableName(typeof(T)));
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops, (T)Activator.CreateInstance(typeof(T)), whereConditions);
            }
            if (!string.IsNullOrEmpty(additionalCondition)) sb.Append(" ").Append(additionalCondition);
            // 検索実行
            return Connection.Query<T>(sb.ToString(), whereConditions, transaction, buffered, commandTimeout, commandType);
        }

        #endregion

        public Guid Insert(object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.Insert<Guid>(this.Connection, entityToInsert, transaction, commandTimeout);

        public T Insert<T>(object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.Insert<T>(this.Connection, entityToInsert, transaction, commandTimeout);

        public int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => Connection.Execute(sql, param, transaction, commandTimeout, commandType);

        public int ExecutePrimaryKey(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            int result = Connection.Execute(sql, param, transaction, commandTimeout, commandType);
            if (result == 0)
            {
                throw new NotFoundException();
            }
            else if (result != 1)
            {
                throw new MultipleUpdateException();
            }
            return result;
        }

        public int ExecutePrimaryKeyList(string sql, IList<object> paramList, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            int result = 0;
            foreach (var param in paramList)
            {
                result += Connection.Execute(sql, param, transaction, commandTimeout, commandType);
            }
            if (result < paramList.Count)
            {
                throw new NotFoundException();
            }
            else if (result > paramList.Count)
            {
                throw new MultipleUpdateException();
            }
            return result;
        }

        public int Update(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.Update(Connection, entityToUpdate, transaction, commandTimeout);

        public int UpdatePrimaryKey(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int result = SimpleCRUD.Update(Connection, entityToUpdate, transaction, commandTimeout);
            if (result == 0)
            {
                throw new NotFoundException();
            }
            else if (result != 1)
            {
                throw new MultipleUpdateException();
            }
            return result;
        }

        public int OptimisticUpdate(string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.OptimisticUpdate(this.Connection, sql, param, transaction, commandTimeout);

        public int Delete(object entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.Delete(Connection, entityToDelete, transaction, commandTimeout);

        public int LogicalDelete(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
           const string sql = @"UPDATE {0} SET
upd_date = {1}upd_date,
upd_username = {1}upd_username,
is_active = 0
WHERE is_active = 1
AND ";

            Type type = entityToUpdate.GetType();
            var idProps = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType() == (typeof(System.ComponentModel.DataAnnotations.KeyAttribute)))).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(type);
            var prameterPrefix = GetParameterPrefix();
            var sb = new StringBuilder();
            sb.AppendFormat(sql, name, prameterPrefix);

            BuildWhere(sb, idProps, entityToUpdate);

            return Connection.Execute(sb.ToString(), entityToUpdate, transaction, commandTimeout);
        }

        #region QueryAsync

        public Task<IEnumerable<T>> QueryAsync<T>(string sql) => Connection.QueryAsync<T>(sql);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => Connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null) => Connection.QueryAsync<T>(sql, param);

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) => Connection.QueryAsync<TFirst, TSecond, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) => Connection.QueryAsync<TFirst, TSecond, TThird, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        public Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => Connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType);

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => Connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);

        public Task<T> GetAsync<T>(object id, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.GetAsync<T>(Connection, id, transaction, commandTimeout);

        public Task<IEnumerable<T>> GetListAsync<T>(object whereConditions, string additionalCondition, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var whereprops = whereConditions != null ?
                whereConditions.GetType().GetProperties() : new PropertyInfo[0];

            // SQL文の組み立て
            var sb = new StringBuilder("select ");
            BuildSelect(sb, typeof(T).GetProperties().Where(p => p.PropertyType.IsSimpleType()));
            sb.Append(" from ").Append(GetTableName(typeof(T)));
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops, (T)Activator.CreateInstance(typeof(T)), whereConditions);
            }
            if (!string.IsNullOrEmpty(additionalCondition)) sb.Append(" ").Append(additionalCondition);
            // 検索実行
            return Connection.QueryAsync<T>(sb.ToString(), whereConditions, transaction, commandTimeout, commandType);
        }

        #endregion

        public Task<Guid> InsertAsync(object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.InsertAsync<Guid>(Connection, entityToInsert, transaction, commandTimeout);

        public Task<T> InsertAsync<T>(object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.InsertAsync<T>(Connection, entityToInsert, transaction, commandTimeout);

        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);

        public Task<int> UpdateAsync(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.UpdateAsync(Connection, entityToUpdate, transaction, commandTimeout);

        public Task<int> OptimisticUpdateAsync(string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.OptimisticUpdateAsync(Connection, sql, param, transaction, commandTimeout);

        public Task<int> DeleteAsync(object entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) => SimpleCRUD.DeleteAsync(Connection, entityToDelete, transaction, commandTimeout);

        public Task<int> LogicalDeleteAsync(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            const string sql = @"UPDATE {0} SET
upd_date = {1}upd_date,
upd_username = {1}upd_username,
is_active = 0
WHERE is_active = 1
AND ";

            Type type = entityToUpdate.GetType();
            var idProps = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType() == (typeof(System.ComponentModel.DataAnnotations.KeyAttribute)))).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(type);
            var prameterPrefix = GetParameterPrefix();
            var sb = new StringBuilder();
            sb.AppendFormat(sql, name, prameterPrefix);


            BuildWhere(sb, idProps, entityToUpdate);

            return Connection.ExecuteAsync(sb.ToString(), entityToUpdate, transaction, commandTimeout);
        }

        /// <summary>
        /// BulkCopyを行います。
        /// </summary>
        /// <typeparam name="T">出力テーブルクラス</typeparam>
        /// <param name="dataList">登録するデータのリスト</param>
        public void BulkCopy<T>(IEnumerable<T> dataList)
        {
            throw new NotImplementedException("データベース依存のため現在は実装していない。データベースにあった処理が出来るようにするべき");
            //var bulkCopy = new SqlBulkCopy((SqlConnection)dbConnection);
            //Type destTable = typeof(T);
            //bulkCopy.DestinationTableName = GetTableName(destTable);
            //foreach (var p in destTable.GetProperties())
            //{
            //    if (!p.PropertyType.IsSimpleType()
            //        || p.GetCustomAttribute<IgnoreInsertAttribute>() != null
            //        || p.GetCustomAttribute<ReadOnlyAttribute>() != null) continue;
            //    bulkCopy.ColumnMappings.Add(p.Name, p.Name);
            //}

            //Open();
            //using (var dataReader = new SqlBulkCopyDataReader<T>(dataList))
            //{
            //    bulkCopy.WriteToServer(dataReader);
            //}
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Dispose Resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~AbstractJPDataHubDbConnection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose Resources.
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (dbConnection != null)
                {
                    System.Console.WriteLine("DbClose");
                    Close();
                    dbConnection.Dispose();
                    dbConnection = null;
                }
            }
        }

        #endregion

        #region Dapper.SimpleCRUDのprivateメソッド
        /// <summary>
        /// テーブル名をエンティティ型から取得します。
        /// </summary>
        /// <param name="t">エンティティ型</param>
        /// <returns>テーブル名</returns>
        private string GetTableName(Type t)
        {
            var m = typeof(SimpleCRUD).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(Type) }, null);
            return (string)m.Invoke(null, new object[] { t });
        }

        /// <summary>
        /// プロパティ情報からSelect句を作成します。
        /// </summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="props">プロパティ情報</param>
        private void BuildSelect(StringBuilder sb, IEnumerable<PropertyInfo> props)
        {
            var m = typeof(SimpleCRUD).GetMethod("BuildSelect", BindingFlags.Static | BindingFlags.NonPublic);
            m.Invoke(null, new object[] { sb, props });
        }

        /// <summary>
        /// プロパティ情報からSelect句を作成します。
        /// </summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="props">プロパティ情報</param>
        /// <param name="sourceEntity"></param>
        /// <param name="whereConditions"></param>
        private void BuildWhere(StringBuilder sb, IEnumerable<PropertyInfo> props, object sourceEntity, object whereConditions = null)
        {
            var m = typeof(SimpleCRUD).GetMethod("BuildWhere", BindingFlags.Static | BindingFlags.NonPublic);
            m.Invoke(null, new object[] { sb, props, sourceEntity, whereConditions });
        }
        #endregion

        #region Dapper.SimpleCRUDのprivateフィールド
        private string GetParameterPrefix()
        {
            var m = typeof(SimpleCRUD).GetField("_parameterPrefix", BindingFlags.Static | BindingFlags.NonPublic);
            return m.GetValue(typeof(SimpleCRUD)).ToString();
        }
        #endregion
    }
}
