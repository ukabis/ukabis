using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Profiling.Data;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Profiler;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Infrastructure.Database.Data
{
    /// <summary>
    /// RDBMSデータアクセス(DynamicAPIリポジトリ用)
    /// </summary>
    public abstract class AbstractJPDataHubRdbms : IJPDataHubRdbms
    {
        protected abstract JPDataHubLogger Log { get; }
        protected abstract string ProviderInvariantName { get; }
        protected abstract object LockObj { get; }
        protected abstract IList<JsonConverter> JsonConverters { get; }

        protected DbConnection DbConnection;
        protected string _ConnectionString;


        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {

                lock (LockObj)
                {
                    if (_ConnectionString != value)
                    {
                        _ConnectionString = value;
                        InitConnection();
                    }
                }
            }
        }


        public AbstractJPDataHubRdbms()
        {
        }


        #region CRUD

        public virtual int UpsertDocument(string sql, object param)
        {
            return DbConnection.Execute(sql, param);
        }

        public virtual IEnumerable<JToken> QueryDocument(string sql, object param)
        {
            foreach (var record in DbConnection.Query(sql, param))
            {
                var settings = new JsonSerializerSettings
                {
                    FloatParseHandling = FloatParseHandling.Decimal,
                    Converters = JsonConverters
                };
                var jsonStr = JsonConvert.SerializeObject(record, settings);
                var json = JToken.Parse(jsonStr);
                yield return json;
            }

            yield break;
        }

        public virtual int DeleteDocument(string sql, object param)
        {
            return DbConnection.Execute(sql, param);
        }

        #endregion

        #region DDL

        public virtual void Execute(string sql)
        {
            DbConnection.Execute(sql);
        }

        #endregion

        #region Connection

        protected virtual void InitConnection()
        {
            Close();
            var dbpf = DbProviderFactories.GetFactory(ProviderInvariantName);
            DbConnection = dbpf.CreateConnection();
            DbConnection.ConnectionString = _ConnectionString;
            Open();

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
                DbConnection = new ProfiledDbConnection(DbConnection, profiler);
            }
        }

        public virtual void Open()
        {
            lock (LockObj)
            {
                Log.Trace($"DbConneciton Open() Executing");
                if (DbConnection.State == ConnectionState.Closed || DbConnection.State == ConnectionState.Broken)
                {
                    DbConnection.Open();
                    Log.Trace($"DbConneciton Open() Executed");
                }
            }
        }

        protected virtual void Close()
        {
            Log.Trace($"DbConneciton Close() Executing");
            if (DbConnection != null && DbConnection.State != ConnectionState.Closed)
            {
                DbConnection.Close();
                Log.Trace($"DbConneciton Close() Executed");
            }
        }

        #endregion

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
        ~AbstractJPDataHubRdbms()
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

                if (DbConnection != null)
                {
                    Close();
                    DbConnection.Dispose();
                    DbConnection = null;
                }
            }
        }

        #endregion
    }
}
