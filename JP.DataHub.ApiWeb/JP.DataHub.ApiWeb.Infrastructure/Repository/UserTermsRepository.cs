using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.Infrastructure.Database.Document;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class UserTermsRepository : AbstractRepository, IUserTermsRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        public IList<UserTermsModel> GetList(string open_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    user_terms_id AS UserTermsId
    ,open_id AS OpenId
    ,terms_id AS TermsId
    ,agreement_date AS AgreementDate
    ,revoke_date AS RevokeDate
FROM
    USER_TERMS
WHERE
    open_id=/*ds open_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    user_terms_id AS UserTermsId
    ,open_id AS OpenId
    ,terms_id AS TermsId
    ,agreement_date AS AgreementDate
    ,revoke_date AS RevokeDate
FROM
    UserTerms
WHERE
    open_id=@open_id
    AND is_active=1
";
            }
            var param = new { open_id = open_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<UserTermsModel>(twowaySql.Sql, dynParams).ToList();
            if (result.Count() == 0)
            {
                throw new NotFoundException($"Not Found TermsGroup.");
            }
            return result;
        }

        public UserTermsModel Get(string open_id, string user_terms_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    user_terms_id AS UserTermsId
    ,open_id AS OpenId
    ,terms_id AS TermsId
    ,agreement_date AS AgreementDate
    ,revoke_date AS RevokeDate
FROM
    USER_TERMS
WHERE
    user_terms_id=/*ds user_terms_id*/'1' 
    AND open_id=/*ds open_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    user_terms_id AS UserTermsId
    ,open_id AS OpenId
    ,terms_id AS TermsId
    ,agreement_date AS AgreementDate
    ,revoke_date AS RevokeDate
FROM
    UserTerms
WHERE
    user_terms_id=@user_terms_id
    AND open_id=@open_id
    AND is_active=1
";
            }
            var param = new { open_id = open_id, user_terms_id = user_terms_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingle<UserTermsModel>(twowaySql.Sql, dynParams);
        }
    }
}
