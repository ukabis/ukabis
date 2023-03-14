using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    // .NET6
    /// <summary>
    /// SqlServerを使用してデータを永続化するためのリポジトリです。
    /// </summary>
    [Obsolete("互換性のために残している旧版です。")]
    internal class SqlServerDataStoreRepositoryQueryOnly : NewAbstractDynamicApiDataStoreRepository
    {
        public override bool CanQuery => true;
        public override string RepositoryName => "SqlServerQueryOnly";

        private readonly Func<string, IJPDataHubDbConnection> _factory;
        private readonly IPerRequestDataContainer _perRequestDataContainer;
        private readonly IContainerDynamicSeparationRepository _repository;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="factory">IJPDataHubDbConnectionのインスタンスを生成するFactory</param>
        /// <param name="perRequestDataContainer">リクエスト毎のコンテキスト</param>
        /// <param name="repository">テーブル名を取得するためのリポジトリ</param>
        public SqlServerDataStoreRepositoryQueryOnly(Func<string, IJPDataHubDbConnection> factory,
            IPerRequestDataContainer perRequestDataContainer,
            IContainerDynamicSeparationRepository repository)
        {
            _factory = factory;
            _perRequestDataContainer = perRequestDataContainer;
            _repository = repository;
        }

        /// <summary>
        /// 検索パラメータで指定されたODataパラメータで検索を行います。
        /// </summary>
        /// <param name="param">検索パラメータ</param>
        /// <param name="responseContinuation"></param>
        /// <returns>検索結果</returns>
        [DataStoreRepositoryParamODataConvert]
        public override IList<JsonDocument> Query(QueryParam param, out XResponseContinuation responseContinuation)
        {
            responseContinuation = null;
            string sql = param.NativeQuery?.Sql;
            if (!string.IsNullOrEmpty(sql))
            {
                // テーブル名を取得
                string tableName = _repository.GetOrRegisterContainerName(RepositoryInfo.PhysicalRepositoryId,
                    param.ControllerId,
                    new VendorId(_perRequestDataContainer.VendorId), new SystemId(_perRequestDataContainer.SystemId));

                // テーブル名を置換
                sql = sql.Replace("@tableName", $"[{tableName}]");

                using (var dbConnection = _factory(RepositoryInfo.ConnectionString))
                {
                    // 検索実行
                    var queryResult = dbConnection.Query<dynamic>(sql);
                    // 結果返却
                    if (queryResult.Any())
                        return queryResult.Select(row => new JsonDocument(JToken.FromObject(row))).ToList();
                }
            }
            else if (!string.IsNullOrEmpty(param.ApiQuery?.Value))
            {
                return QueryEnumerable(param).ToList();
            }

            return null;
        }

        /// <summary>
        /// 検索パラメータで指定されたSQL文とクエリパラメータで検索を行います。
        /// </summary>
        /// <param name="param">検索パラメータ</param>
        /// <returns>検索結果</returns>
        public override IEnumerable<JsonDocument> QueryEnumerable(QueryParam param)
        {
            if (string.IsNullOrEmpty(param.ApiQuery?.Value)) return null;

            // 検索パラメータ作成
            var queryParams = new DynamicParameters();
            if (param.QueryString != null)
            {
                foreach (var qs in param.QueryString.Dic)
                    queryParams.Add(qs.Key.Value, qs.Value.Value);
            }

            using (var dbConnection = _factory(RepositoryInfo.ConnectionString))
            {
                // 検索実行
                var queryResult = dbConnection.Query<dynamic>(param.ApiQuery.Value, queryParams);
                // 結果返却
                if (queryResult.Any())
                    return queryResult.Select(row => new JsonDocument(JToken.FromObject(row)));
            }

            return null;
        }

        /// <summary>
        /// 内部で追加するWhere句を作成します。
        /// </summary>
        /// <param name="param">検索パラメータ</param>
        /// <returns>追加のWhere句</returns>
        public override string GetInternalAddWhereString(QueryParam param, out IDictionary<string, object> additionalParameters)
        {
            additionalParameters = new Dictionary<string, object>();
            return null;
        }

        public override JsonDocument QueryOnce(QueryParam param) => throw new NotImplementedException();

        public override RegisterOnceResult RegisterOnce(RegisterParam param) => throw new NotImplementedException();

        public override IEnumerable<string> Delete(DeleteParam param) => throw new NotImplementedException();

        public override void DeleteOnce(DeleteParam param) => throw new NotImplementedException();
    }
}
