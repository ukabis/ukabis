using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Document;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.Com.Sql;
using Org.BouncyCastle.Crypto.Agreement;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class AgreementRepository : AbstractRepository, IAgreementRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Document"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        public AgreementModel GetAgreement(string agreementId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    agreement_id AS AgreementId,
    vendor_id AS VendorId,
	title AS Title,
	detail AS Detail 
FROM
    AGREEMENT 
WHERE
    is_active=1 
AND agreement_id = /*ds agreement_id*/'00000000-0000-0000-0000-000000000000' 
";
            }
            else
            {
                sql = @"
SELECT
    agreement_id AS AgreementId,
    vendor_id AS VendorId,
	title AS Title,
	detail AS Detail
FROM
    Agreement
WHERE
    is_active=1
AND agreement_id = @agreement_id
";
            }
            var param = new { agreement_id = agreementId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingleOrDefault<AgreementModel>(twowaySql.Sql, dynParams);

            if (result == null)
            {
                throw new NotFoundException($"Not Found AgreementId={agreementId}");
            }
            return result;
        }

        public List<AgreementModel> GetAgreementList(string? vendorId = null)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    agreement_id AS AgreementId
    ,vendor_id AS VendorId
	,title AS Title
	,detail AS Detail 
FROM
    AGREEMENT 
WHERE
/*ds if VendorId != null*/
vendor_id =/*ds VendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
is_active=1 
ORDER BY
    title
";
            }
            else
            {
                sql = @"
SELECT
    agreement_id AS AgreementId
    ,vendor_id AS VendorId
	,title AS Title
	,detail AS Detail
FROM
    Agreement
WHERE
/*ds if VendorId != null*/
vendor_id =/*ds VendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
is_active=1
ORDER BY
    title
";
            }
            var param = new { VendorId = vendorId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<AgreementModel>(twowaySql.Sql, dynParams).ToList();
            return result;
        }

        [DomainDataSync(DocumentDatabase.TABLE_AGREEMENT, "AgreementId")]
        public AgreementModel RegistAgreement(AgreementModel agreement)
        {
            // Vendorが存在するか
            if (!ExistVendor(agreement.VendorId))
            {
                throw new NotFoundException();
            }

            var now = this.PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow);
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);

            DB_Agreement dbAgreement = new DB_Agreement()
            {
                agreement_id = Guid.Parse(agreement.AgreementId),
                vendor_id = Guid.Parse(agreement.VendorId),
                title = agreement.Title,
                detail = agreement.Detail,
                reg_date = now,
                reg_username = updUserId,
                upd_date = now,
                upd_username = updUserId,
                is_active = true
            };

            this.Connection.Insert(dbAgreement);

            return agreement;
        }

        [DomainDataSync(DocumentDatabase.TABLE_AGREEMENT, "AgreementId")]
        public AgreementModel UpdateAgreement(AgreementModel agreement)
        {
            var now = this.PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow);
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);

            DB_Agreement dbAgreement = new DB_Agreement()
            {
                agreement_id = Guid.Parse(agreement.AgreementId),
                title = agreement.Title,
                detail = agreement.Detail,
                upd_date = now,
                upd_username = updUserId
            };


            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    AGREEMENT 
SET
    title = /*ds title*/'a' 
    ,detail = /*ds detail*/'a' 
    ,upd_date = /*ds upd_date*/SYSTIMESTAMP 
    ,upd_username = /*ds upd_username*/'a' 
WHERE
    agreement_id = /*ds agreement_id*/'00000000-0000-0000-0000-000000000000' 
";
            }
            else
            {
                sql = @"
UPDATE
    agreement
SET
    title = @title
    ,detail = @detail
    ,upd_date = @upd_date
    ,upd_username = @upd_username
WHERE
    agreement_id = @agreement_id
";
            }

            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, dbAgreement);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(dbAgreement).SetNClob(nameof(dbAgreement.detail));
            this.Connection.Execute(twowaySql.Sql, dbAgreement);

            return agreement;
        }

        [DomainDataSync(DocumentDatabase.TABLE_AGREEMENT, "agreementId")]
        public void DeleteAgreement(string agreementId)
        {

            // 規約が使用済みの場合は削除不可
            if (!this.CheckUsedAgreement(agreementId))
            {
                throw new InUseException("指定された規約は使用されている為削除できません。");
            }

            var now = this.PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow);
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);

            DB_Agreement dbAgreement = new DB_Agreement()
            {
                agreement_id = Guid.Parse(agreementId),
                upd_date = now,
                upd_username = updUserId,
                is_active = false
            };

            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    AGREEMENT 
SET
    upd_date = /*ds upd_date*/SYSTIMESTAMP 
    ,upd_username = /*ds upd_username*/'a' 
    ,is_active = /*ds is_active*/1 
WHERE
    agreement_id = /*ds agreement_id*/'00000000-0000-0000-0000-000000000000' 
";
            }
            else
            {
                sql = @"
UPDATE
    agreement
SET
    upd_date = @upd_date
    ,upd_username = @upd_username
    ,is_active = @is_active
WHERE
    agreement_id = @agreement_id
";
            }

            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, dbAgreement);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(dbAgreement);
            this.Connection.Execute(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// 規約が既に使用されているか
        /// </summary>
        /// <param name="agreementId">agreementId</param>
        /// <returns></returns>
        private bool CheckUsedAgreement(string agreementId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = "SELECT COUNT(document_id) FROM DOCUMENT WHERE agreement_id = /*ds agreement_id*/'00000000-0000-0000-0000-000000000000' AND is_active = 1";
            }
            else
            {
                sql = "SELECT COUNT(document_id) FROM Document WHERE agreement_id = @agreement_id AND is_active = 1";
            }
            var param = new { agreement_id = agreementId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var ret = this.Connection.QuerySingle<int>(twowaySql.Sql, dynParams);

            return (ret == 0);
        }

        /// <summary>
        /// Vendorが存在しているか
        /// </summary>
        /// <param name="vendorId">vendor_id</param>
        /// <returns>存在する場合はtrue、しない場合はfalseを返す</returns>
        private bool ExistVendor(string vendorId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = "SELECT COUNT(vendor_id) FROM VENDOR WHERE vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' AND is_active = 1";
            }
                else
            {
                sql = "SELECT COUNT(vendor_id) FROM Vendor WHERE vendor_id = @vendor_id AND is_active = 1";
            }
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var ret = this.Connection.QuerySingle<int>(twowaySql.Sql, dynParams);

            // Vendorが存在していればtrue
            return ret > 0;
        }
    }
}