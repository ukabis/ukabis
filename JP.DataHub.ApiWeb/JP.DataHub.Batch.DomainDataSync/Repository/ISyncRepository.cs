using JP.DataHub.Batch.DomainDataSync.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.DomainDataSync.Repository
{
    public interface ISyncRepository
    {
        ISyncEntity Init();
        Dictionary<string, string> GetData(string connectionString, string tableName, List<string> columnNames, string pkValue);
        Dictionary<string, string> GetDataByConfigSql(string connectionString, string select, string where, string pkValue);
        List<string> GetForeignKeyTableData(string connectionString, string tableName, string foreignKeyTableName, string pkValue);
        List<Dictionary<string, string>> GetAllData(string connectionString, string tableName, List<string> columnNames);
        List<Dictionary<string, string>> GetAllDataByConfigSql(string connectionString, string sql);
        string GetPrimaryKeyColumnName(string connectionString, string tableName);
        List<string> GetColumnNames(string connectionString, string tableName);
        int Delete(string connectionString, string tableName, string specificData);
        int Merge(string connectionString, string tableName, Dictionary<string, string> upsertCollection);
    }
}