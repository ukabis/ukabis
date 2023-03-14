using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using System.Security.Cryptography.Xml;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Api.Core.Exceptions;
using System.Data.SqlClient;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using AutoMapper;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class RevokeRepository : AbstractRepository, IRevokeRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DB_UserRevoke, UserRevokeModel>()
                    .ForMember(dst => dst.UserRevokeId, ops => ops.MapFrom(src => src.user_revoke_id))
                    .ForMember(dst => dst.UserTermsId, ops => ops.MapFrom(src => src.user_terms_id))
                    .ForMember(dst => dst.TermsId, ops => ops.MapFrom(src => src.terms_id))
                    .ForMember(dst => dst.IsFinish, ops => ops.MapFrom(src => src.is_finished))
                    .ForMember(dst => dst.OpenId, ops => ops.MapFrom(src => src.open_id))
                    .ForMember(dst => dst.Start, ops => ops.MapFrom(src => src.start_date))
                    .ForMember(dst => dst.End, ops => ops.MapFrom(src => src.end_date));
                cfg.CreateMap<DB_RevokeHistory, RemoveHistoryModel>()
                    .ForMember(dst => dst.RevokeHistoryId, ops => ops.MapFrom(src => src.revoke_history_id))
                    .ForMember(dst => dst.ControllerId, ops => ops.MapFrom(src => src.controller_id))
                    .ForMember(dst => dst.Start, ops => ops.MapFrom(src => src.start_date))
                    .ForMember(dst => dst.End, ops => ops.MapFrom(src => src.end_date));
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        public UserRevokeModel Start(string user_terms_id, string open_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT terms_id FROM USER_TERMS WHERE user_terms_id=/*ds user_terms_id*/'1' AND is_active=1";
            }
            else
            {
                sql = "SELECT terms_id FROM UserTerms WHERE user_terms_id=@user_terms_id AND is_active=1";
            }
            var param = new { user_terms_id = user_terms_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var terms_id = Connection.QuerySingleOrDefault<Guid>(twowaySql.Sql, dynParams);
            if (terms_id == null)
            {
                throw new NotFoundException($"not found user_terms_id({user_terms_id}).");
            }
            var now = DateTime.UtcNow;
            var result = new DB_UserRevoke() { user_revoke_id = Guid.NewGuid(), user_terms_id = user_terms_id.To<Guid>(), terms_id = terms_id, is_finished = false, open_id = open_id.To<Guid>(), start_date = now, reg_date = now, reg_username = open_id, upd_date = now, upd_username = open_id, is_active = true  };
            try
            {
                Connection.Insert(result);
            }
            catch (SqlException ex)
            {
                // 外部キー制約違反の場合、独自例外を返す
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                throw new SqlDatabaseException(ex.Message);
            }
            return s_mapper.Map<UserRevokeModel>(result);
        }

        public void Stop(string user_revoke_id, string open_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT user_revoke_id,end_date FROM USER_REVOKE WHERE user_revoke_id=/*ds user_revoke_id*/'1' AND is_active=1";
            }
            else
            {
                sql = "SELECT user_revoke_id,end_date FROM UserRevoke WHERE user_revoke_id=@user_revoke_id AND is_active=1";
            }
            var param = new { user_revoke_id = user_revoke_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var record = Connection.QuerySingleOrDefault<DB_UserRevoke>(twowaySql.Sql, dynParams);
            if (record == null)
            {
                throw new AlreadyExistsException($"有効なUserRevokeが存在しません. user_revoke_id = {user_revoke_id}");
            }
            else if (record.end_date != null)
            {
                throw new AlreadyExistsException($"user_revoke_id({user_revoke_id})はStop処理をしています");
            }
            if (dbSettings.Type == "Oracle")
            {
                sql = "UPDATE USER_REVOKE SET is_finished=1,end_date=SYSTIMESTAMP,upd_date=SYSTIMESTAMP,upd_username=/*ds open_id*/'1' WHERE user_revoke_id=/*ds user_revoke_id*/'1' AND end_date IS NULL AND is_active=1";
            }
            else
            {
                sql = "UPDATE UserRevoke SET is_finished=1,end_date=GETDATE(),upd_date=GETDATE(),upd_username=@open_id WHERE user_revoke_id=@user_revoke_id AND end_date IS NULL AND is_active=1";
            }
            var param2 = new { user_revoke_id = user_revoke_id, open_id = open_id };
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param2);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            if (Connection.Execute(twowaySql.Sql, dynParams) < 1)
            {
                throw new NotFoundException($"not found user_revoke_id({user_revoke_id}).");
            }
        }

        public RemoveHistoryModel RemoveResourceStart(string user_revoke_id, string controller_id, string open_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT COUNT(*) FROM CONTROLLER WHERE controller_id=/*ds controller_id*/'1' AND is_active=1";
            }
            else
            {
                sql = "SELECT COUNT(*) FROM Controller WHERE controller_id=@controller_id AND is_active=1";
            }
            var param = new { controller_id = controller_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            int count = Connection.QuerySingleOrDefault<int>(twowaySql.Sql, dynParams);
            if (count == 0)
            {
                throw new NotFoundException($"有効なリソースが存在しません. controller_id = {controller_id}");
            }
            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT COUNT(*) FROM USER_REVOKE WHERE user_revoke_id=/*ds user_revoke_id*/'1' AND end_date IS NULL AND is_active=1";
            }
            else
            {
                sql = "SELECT COUNT(*) FROM UserRevoke WHERE user_revoke_id=@user_revoke_id AND end_date IS NULL AND is_active=1";
            }
            var param2 = new { user_revoke_id = user_revoke_id };
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param2);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            count = Connection.QuerySingleOrDefault<int>(twowaySql.Sql, dynParams);
            if (count == 0)
            {
                throw new NotFoundException($"有効なUserRevokeが存在しません. user_revoke_id = {user_revoke_id}");
            }

            var now = DateTime.UtcNow;
            var result = new DB_RevokeHistory() { revoke_history_id = Guid.NewGuid(), user_revoke_id = user_revoke_id.To<Guid>(), controller_id = controller_id.To<Guid>(), start_date = now, reg_date = now, reg_username = open_id, upd_date = now, upd_username = open_id, is_active = true };
            try
            {
                Connection.Insert(result);
            }
            catch (SqlException ex)
            {
                // 外部キー制約違反の場合、独自例外を返す
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                throw new SqlDatabaseException(ex.Message);
            }
            return s_mapper.Map<RemoveHistoryModel>(result);
        }

        public void RemoveResourceStop(string revoke_history_id, string open_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT revoke_history_id,end_date FROM REVOKE_HISTORY WHERE revoke_history_id=/*ds revoke_history_id*/'1' AND is_active=1";
            }
            else
            {
                sql = "SELECT revoke_history_id,end_date FROM RevokeHistory WHERE revoke_history_id=@revoke_history_id AND is_active=1";
            }
            var param = new { revoke_history_id = revoke_history_id, open_id = open_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var record = Connection.QuerySingleOrDefault<DB_RevokeHistory>(twowaySql.Sql, dynParams);
            if (record == null)
            {
                throw new NotFoundException($"RevokeHistory. revoke_history_id = {revoke_history_id}");
            }
            if (record.end_date != null)
            {
                throw new AlreadyExistsException($"既にこのrevoke_history_id({revoke_history_id})はRemoveResourceStop処理をしています");
            }
            if (dbSettings.Type == "Oracle")
            {
                sql = "UPDATE REVOKE_HISTORY SET end_date=/*ds now*/SYSTIMESTAMP , upd_date=/*ds now*/SYSTIMESTAMP , upd_username=/*ds open_id*/'1' WHERE revoke_history_id=/*ds revoke_history_id*/'1' AND end_date IS NULL AND is_active=1";
            }
            else
            {
                sql = "UPDATE RevokeHistory SET end_date=@now,upd_date=@now,upd_username=@open_id WHERE revoke_history_id=@revoke_history_id AND end_date IS NULL AND is_active=1";
            }
            var param2 = new { revoke_history_id = revoke_history_id, now = DateTime.UtcNow, open_id = open_id };
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param2);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            if (Connection.Execute(twowaySql.Sql, dynParams) < 1)
            {
                throw new NotFoundException($"not found user_revoke_id({revoke_history_id}).");
            }
        }
    }
}
