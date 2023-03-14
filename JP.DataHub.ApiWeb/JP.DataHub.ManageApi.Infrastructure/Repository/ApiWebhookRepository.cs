using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using AutoMapper;
using System.Data.SqlClient;
using System.Text;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class ApiWebhookRepository : AbstractRepository, IApiWebhookRepository
    {
        private static readonly JPDataHubLogger s_logger = new(typeof(RepositoryGroupRepository));

        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DB_ControllerWebhook, ApiWebhookModel>()
                    .ForMember(dst => dst.ApiWebhookId, ops => ops.MapFrom(src => src.controller_webhook_id))
                    .ForMember(dst => dst.VendorId, ops => ops.MapFrom(src => src.vendor_id))
                    .ForMember(dst => dst.ApiId, ops => ops.MapFrom(src => src.controller_id))
                    .ForMember(dst => dst.Headers, ops => ops.MapFrom(src => ToHeaderList(src.headers)))
                    .ForMember(dst => dst.ApiId, ops => ops.MapFrom(src => src.controller_id))
                    .ForMember(dst => dst.NotifyRegister, ops => ops.MapFrom(src => src.notify_register))
                    .ForMember(dst => dst.NotifyUpdate, ops => ops.MapFrom(src => src.notify_update))
                    .ForMember(dst => dst.NotifyDelete, ops => ops.MapFrom(src => src.notify_delete));
                cfg.CreateMap<ApiWebhookModel, DB_ControllerWebhook>()
                    .ForMember(dst => dst.controller_webhook_id, ops => ops.MapFrom(src => src.ApiWebhookId))
                    .ForMember(dst => dst.vendor_id, ops => ops.MapFrom(src => src.VendorId))
                    .ForMember(dst => dst.controller_id, ops => ops.MapFrom(src => src.ApiId))
                    .ForMember(dst => dst.headers, o => o.MapFrom(src => ToHeaderString(src.Headers)))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get => s_lazyMapper.Value; }

        /// <summary>
        /// 指定されたApiWebhookIdのApiWebhookを取得します。
        /// </summary>
        /// <param name="apiWebhookId">ApiWebhookId</param>
        /// <returns>Webhook</returns>
        public ApiWebhookModel Get(string apiWebhookId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
   controller_webhook_id,
   controller_id,
   vendor_id,
   url,
   headers,
   notify_register,
   notify_update,
   notify_delete 
FROM
   CONTROLLER_WEBHOOK 
WHERE
   controller_webhook_id = /*ds api_webhook_id*/'00000000-0000-0000-0000-000000000000' 
   AND is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
   controller_webhook_id,
   controller_id,
   vendor_id,
   url,
   headers,
   notify_register,
   notify_update,
   notify_delete
FROM
   ControllerWebhook
WHERE
   controller_webhook_id = @api_webhook_id
   AND is_active = 1
";
            }
            var param = new { api_webhook_id = apiWebhookId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<DB_ControllerWebhook>(twowaySql.Sql, dynParams);
            return s_mapper.Map<ApiWebhookModel>(result);
        }

        /// <summary>
        /// 指定されたAPI ID、ベンダーIDのApiWebhookの存在確認をします。
        /// </summary>
        /// <param name="apiId">API ID</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>bool</returns>
        public bool IsExists(string apiId, string vendorId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
   COUNT(controller_webhook_id) AS Count 
FROM
   CONTROLLER_WEBHOOK 
WHERE
   controller_id = /*ds controller_id*/'00000000-0000-0000-0000-000000000000' 
   AND vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' 
   AND is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
   COUNT(controller_webhook_id) AS Count
FROM
   ControllerWebhook
WHERE
   controller_id = @controller_id
   AND vendor_id = @vendor_id
   AND is_active = 1
";
            }
            var param = new { controller_id = apiId, vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            if (Connection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 指定されたベンダーIDのApiWebhook一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ApiWebhookId</param>
        /// <returns>Webhook一覧</returns>
        public IList<ApiWebhookModel> GetList(string vendorId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
   controller_webhook_id,
   controller_id,
   vendor_id,
   url,
   headers,
   notify_register,
   notify_update,
   notify_delete 
FROM
   CONTROLLER_WEBHOOK
WHERE
   vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' 
   AND is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
   controller_webhook_id,
   controller_id,
   vendor_id,
   url,
   headers,
   notify_register,
   notify_update,
   notify_delete
FROM
   ControllerWebhook
WHERE
   vendor_id = @vendor_id
   AND is_active = 1
";
            }
            var param = new { vendor_id = vendorId, is_active = 1 };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            // 検索実行
            var result = Connection.Query<DB_ControllerWebhook>(twowaySql.Sql, dynParams).ToList();
            return s_mapper.Map<IList<ApiWebhookModel>>(result);
        }

        /// <summary>
        /// Webhookを登録します。
        /// </summary>
        /// <param name="model">Webhook</param>
        [CacheIdFire("controller_webhook_id", "model.ApiWebhookId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_CONTROLLERWEBHOOK)]
        public void Register(ApiWebhookModel model)
        {
            // データベース登録用のモデルを生成
            var db_model = s_mapper.Map<DB_ControllerWebhook>(model);

            // 登録実行
            Connection.Insert(db_model);
        }

        /// <summary>
        /// Webhookを更新します。
        /// </summary>
        /// <param name="model">Webhook</param>
        [CacheIdFire("controller_webhook_id", "model.ApiWebhookId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_CONTROLLERWEBHOOK)]
        public void Update(ApiWebhookModel model)
        {
            // データベース登録用のモデルを生成
            var db_model = s_mapper.Map<DB_ControllerWebhook>(model);
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE
   CONTROLLER_WEBHOOK 
SET 
   controller_id = /*ds controller_id*/'00000000-0000-0000-0000-000000000000' ,
   vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' ,
   url = /*ds url*/'a' ,
   headers = /*ds headers*/'a' ,
   notify_register = /*ds notify_register*/1 ,
   notify_update = /*ds notify_update*/1 ,
   notify_delete = /*ds notify_delete*/1 ,
   upd_date = /*ds upd_date*/SYSTIMESTAMP ,
   upd_username = /*ds upd_username*/'a' ,
   is_active = /*ds is_active*/1 
WHERE 
   controller_webhook_id = /*ds controller_webhook_id*/'00000000-0000-0000-0000-000000000000' 
";
            }
            else
            {
                sql = @"
UPDATE
   ControllerWebhook
SET 
   controller_id = @controller_id,
   vendor_id = @vendor_id,
   url = @url,
   headers = @headers,
   notify_register = @notify_register,
   notify_update = @notify_update,
   notify_delete = @notify_delete,
   upd_date = @upd_date,
   upd_username = @upd_username,
   is_active = @is_active
WHERE 
   controller_webhook_id = @controller_webhook_id
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, db_model);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(db_model);
            // 更新実行
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// Webhookを削除します。
        /// </summary>
        /// <param name="apiWebhookId">ApiWebhookId</param>
        [CacheIdFire("apiWebhookId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_CONTROLLERWEBHOOK)]
        public void Delete(string apiWebhookId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE
   CONTROLLER_WEBHOOK
SET 
   is_active = 0
WHERE 
   controller_webhook_id = /*ds api_webhook_id*/'00000000-0000-0000-0000-000000000000' 
";
            }
            else
            {
                sql = @"
UPDATE
   ControllerWebhook
SET 
   is_active = 0
WHERE 
   controller_webhook_id = @api_webhook_id
";
            }
            var param = new { api_webhook_id = apiWebhookId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// HttpHeaderのリストを文字列に変換します。
        /// </summary>
        /// <param name="headers">HttpHeaderのリスト</param>
        /// <returns>ヘッダー文字列</returns>
        private static string ToHeaderString(IList<HttpHeaderModel> headers)
        {
            if (headers == null) return null;

            var sb = new StringBuilder();
            foreach (var header in headers)
            {
                sb.AppendLine(header.FieldName + ":" + header.Value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// ヘッダー文字列をHttpHeaderのリストに変換します。
        /// </summary>
        /// <param name="headers">ヘッダー文字列</param>
        /// <returns>HttpHeaderのリスト</returns>
        private static IList<HttpHeaderModel> ToHeaderList(string headers)
        {
            if (string.IsNullOrEmpty(headers)) return null;

            var result = new List<HttpHeaderModel>();
            foreach (string line in headers.Split('\r', '\n'))
            {
                int idx = line.IndexOf(':');
                if (idx > 0) result.Add(new HttpHeaderModel(line.Substring(0, idx), line.Substring(idx + 1)));
            }

            return result;
        }
    }
}