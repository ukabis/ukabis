using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Transaction
{
    public interface IJPDataHubDbConnection : IDisposable
    {
        /// <summary>
        /// DBのプロバイダー名
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// データベース名
        /// </summary>
        string DbType { get; }

        /// <summary>
        /// DB接続用オブジェクト
        /// </summary>
        IDbConnection Connection { get; }

        void Open();

        void Close();

        #region Query
        IEnumerable<T> Query<T>(string sql);
        IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<T> Query<T>(string sql, object param = null);
        IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        T QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        T QueryPrimaryKey<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        T QuerySingleOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        T Get<T>(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<T> GetList<T>(object whereConditions, string additionalCondition, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        #endregion

        Guid Insert(object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);

        T Insert<T>(object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);

        int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        int ExecutePrimaryKey(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        int ExecutePrimaryKeyList(string sql, IList<object> paramList, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        int Update(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);

        int UpdatePrimaryKey(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);

        int OptimisticUpdate(string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null);

        int LogicalDelete(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);

        #region QueryAsync
        Task<IEnumerable<T>> QueryAsync<T>(string sql);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<T> GetAsync<T>(object id, IDbTransaction transaction = null, int? commandTimeout = null);
        Task<IEnumerable<T>> GetListAsync<T>(object whereConditions, string additionalCondition, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        #endregion

        Task<Guid> InsertAsync(object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<T> InsertAsync<T>(object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<int> UpdateAsync(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int> OptimisticUpdateAsync(string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int> LogicalDeleteAsync(object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// BulkCopyを行います。
        /// </summary>
        /// <typeparam name="T">出力テーブルクラス</typeparam>
        /// <param name="dataList">登録するデータのリスト</param>
        void BulkCopy<T>(IEnumerable<T> dataList);
    }
}
