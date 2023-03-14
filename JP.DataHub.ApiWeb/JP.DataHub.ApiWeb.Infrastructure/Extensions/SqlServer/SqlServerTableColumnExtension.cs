using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.Infrastructure.Database.Consts;

namespace JP.DataHub.ApiWeb.Infrastructure.Extensions.SqlServer
{
    // .NET6
    /// <summary>
    /// テーブル列定義文字列を取得する。
    /// </summary>
    internal static class SqlServerTableColumnExtension
    {
        public static string GetDefaultPhrase(this RdbmsTableColumn column) =>
            column.DefaultValue == null ? "" : $"DEFAULT '{column.DefaultValue}'";

        public static string GetNullablePhrase(this RdbmsTableColumn column) =>
            column.Nullable ? "NULL" : "NOT NULL";

        public static string GetCollatePhrase(this RdbmsTableColumn column) =>
            $"{(string.IsNullOrWhiteSpace(column.Collate) ? "" : $"COLLATE {column.Collate}")}";

        public static string GetColumnDeclaration(this RdbmsTableColumn column) =>
            string.Join(" ", new string[] { $"\"{column.Name}\"", column.DataType, column.GetCollatePhrase(), column.GetDefaultPhrase(), column.GetNullablePhrase() }.Where(x => !string.IsNullOrEmpty(x)));


        /// <summary>
        /// JsonSchemaの型をSQLServerの型にマッピングする。
        /// </summary>
        public static string GetSqlServerDataType(this JSchema schema, bool withoutLength = false)
        {
            // オブジェクト・配列を含む場合
            if (!schema.Type.IsFlatType())
            {
                return $"nvarchar(MAX)";
            }

            // DateTime
            if (schema.IsDateTimeFormat())
            {
                return "datetime";
            }

            // その他
            switch (schema.Type & ~JSchemaType.Null)
            {
                case JSchemaType.Boolean:
                    return "bit";
                case JSchemaType.Number:
                case JSchemaType.Integer:
                    return withoutLength ? "numeric" : $"numeric({SqlServerConsts.NumericPrecision},{SqlServerConsts.NumericScale})";
                default:
                    var isIndexable = schema.MaximumLength.HasValue && schema.MaximumLength <= SqlServerConsts.NCharTableColumnMaxLength;
                    if (isIndexable)
                    {
                        return withoutLength ? "nvarchar" : $"nvarchar({SqlServerConsts.NCharTableColumnMaxLength})";
                    }
                    else
                    {
                        return $"nvarchar(MAX)";
                    }
            }
        }
    }
}
