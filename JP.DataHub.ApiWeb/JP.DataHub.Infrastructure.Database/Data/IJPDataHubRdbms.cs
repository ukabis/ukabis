using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.Infrastructure.Database.Data
{
    // .NET6
    /// <summary>
    /// RDBMSデータアクセス(DynamicAPIリポジトリ用)
    /// </summary>
    public interface IJPDataHubRdbms : IDisposable
    {
        string ConnectionString { get; set; }

        int UpsertDocument(string sql, object param);

        IEnumerable<JToken> QueryDocument(string sql, object param);

        int DeleteDocument(string sql, object param);

        void Execute(string sql);

        void Open();
    }
}
