using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Infrastructure.Database.Consts;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal class SqlServerDeleteSqlBuilder : IDeleteSqlBuilder
    {
        private DeleteParam DeleteParam { get; }
        private RepositoryInfo RepositoryInfo { get; }
        private IContainerDynamicSeparationRepository ContainerDynamicSeparationRepository { get; }

        public string Sql { get; protected set; }
        public IRdbmsSqlParameterList SqlParameterList { get; protected set; }

        public bool HasPreAdditionalSqls { get; private set; } = false;
        public IEnumerable<(string Sql, IRdbmsSqlParameterList SqlParameterList)> PreAdditionalSqls { get; private set; }


        public SqlServerDeleteSqlBuilder(
            DeleteParam deleteParam,
            RepositoryInfo repositoryInfo,
            IContainerDynamicSeparationRepository containerDynamicSeparationRepository)
        {
            DeleteParam = deleteParam;
            RepositoryInfo = repositoryInfo;
            ContainerDynamicSeparationRepository = containerDynamicSeparationRepository;
        }


        public void BuildUp()
        {
            // テーブル取得
            string tableName;
            if (DeleteParam.OperationInfo?.IsAttachFileOperation == true)
            {
                tableName = SqlServerConsts.AttachFileMetaTableName;
            }
            else
            {
                tableName = ContainerDynamicSeparationRepository.GetOrRegisterContainerName(
                    RepositoryInfo.PhysicalRepositoryId,
                    DeleteParam.ControllerId,
                    new VendorId(Guid.Empty.ToString()),
                    new SystemId(Guid.Empty.ToString()));
            }

            // SQL生成
            var value = DeleteParam.Json[JsonPropertyConst.ID].Value<string>();
            var parameters = new SqlServerSqlParameterList();
            parameters.Add(JsonPropertyConst.ID, value);
            var sql = $"DELETE FROM \"{tableName}\" WHERE \"{JsonPropertyConst.ID}\" = {parameters[JsonPropertyConst.ID].SqlParameter}";

            // 結果をプロパティにセット
            Sql = sql;
            SqlParameterList = parameters;

            // 添付ファイルメタの場合は検索用のテーブルの削除SQLを追加
            if (DeleteParam.OperationInfo?.IsAttachFileOperation == true)
            {
                SetAttachFileMetaAdditionalSql();
            }
        }


        /// <summary>
        /// 添付ファイルメタ用の削除SQLを設定する。
        /// </summary>
        private void SetAttachFileMetaAdditionalSql()
        {
            var attachFileMetaId = SqlParameterList.FirstOrDefault(x => x.Key == JsonPropertyConst.ID).Value.ParameterValue;
            var parameters = new SqlServerSqlParameterList();
            parameters.Add("AttachFileMetaId", attachFileMetaId, autoParameterName: false);

            HasPreAdditionalSqls = true;
            PreAdditionalSqls = new List<(string, IRdbmsSqlParameterList)>()
            {
                new ($"DELETE FROM {SqlServerConsts.AttachFileMetaSearchTableName} WHERE AttachFileMetaId = @AttachFileMetaId", parameters)
            };
        }
    }
}
