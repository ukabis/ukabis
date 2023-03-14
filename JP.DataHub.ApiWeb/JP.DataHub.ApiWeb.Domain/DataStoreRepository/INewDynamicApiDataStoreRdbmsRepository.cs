using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    [Log]
    interface INewDynamicApiDataStoreRdbmsRepository : INewDynamicApiDataStoreRepository
    {
        /// <summary>
        /// マルチバイト文字列の最大文字数
        /// </summary>
        int? NCharTableColumnMaxLength { get; }

        /// <summary>
        /// テーブル列名の最大バイト数
        /// </summary>
        int? TableColumnNameMaxBytes { get; }

        /// <summary>
        /// テーブル最大列数
        /// </summary>
        int? TableMaxColumns { get; }

        /// <summary>
        /// テーブル列名パターン
        /// </summary>
        string TableColumnNamePattern { get; }


        IEnumerable<RdbmsTableColumn> GetTableColumns(string tableName);

        void CreateTable(string tableName, IEnumerable<RdbmsTableColumn> columns, IEnumerable<string> primaryKeyColumns = null, string primaryKeyName = null);

        void AddTableColumns(string tableName, IEnumerable<RdbmsTableColumn> columns);

        void CreateIndex(string tableName, string columnName, string indexName = null);
    }
}
