using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal static class SqlBuilderUtil
    {
        /// <summary>
        /// パラメータ名をSQLパラメータ用の文字列に変換する。
        /// </summary>
        public static string ToSqlParameter(string parameterName, RepositoryType repositoryType)
        {
            var prefix = GetSqlParameterPrefix(repositoryType);

            if (parameterName.StartsWith(prefix))
            {
                // 既にパラメータ形式なら何もしない
                return parameterName;
            }
            else
            {
                // プレフィックスを付与
                return $"{prefix}{parameterName}";
            }
        }

        /// <summary>
        /// SQLパラメータ用の文字列からパラメータ名に逆変換する。
        /// </summary>
        public static string ToParameterName(string sqlParameter, RepositoryType repositoryType)
        {
            var prefix = GetSqlParameterPrefix(repositoryType);

            if (sqlParameter.StartsWith(prefix))
            {
                // プレフィックスを除去
                return sqlParameter.Substring(prefix.Length);
            }
            else
            {
                // パラメータ形式でなければ何もしない
                return sqlParameter;
            }
        }

        /// <summary>
        /// SQLパラメータかどうか。
        /// </summary>
        public static bool IsSqlParameter(string parameterName, RepositoryType repositoryType)
        {
            var prefix = GetSqlParameterPrefix(repositoryType);
            return parameterName.StartsWith(prefix);
        }


        /// <summary>
        /// リポジトリタイプに応じたSQLパラメータのプレフィクスを取得する。
        /// </summary>
        private static string GetSqlParameterPrefix(RepositoryType repositoryType)
        {
            switch (repositoryType)
            {
                case RepositoryType.SQLServer2: return "@";
                case RepositoryType.CosmosDB: return "@";
                case RepositoryType.OracleDb: return ":";
                default: throw new UnsupportedRepositoryTypeException(repositoryType.ToString());
            }
        }
    }
}
