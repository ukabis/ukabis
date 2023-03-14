using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using System.Data.SqlClient;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using System.IO;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class ResourceSharingPersonRepository : AbstractRepository, IResourceSharingPersonRepository
    {
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ResourceSharingPersonRuleModel, DB_ResourceSharingPersonRule>().ReverseMap()
                    .ForMember(dst => dst.ResourceSharingPersonRuleId, ops => ops.MapFrom(src => src.resource_sharing_person_rule_id))
                    .ForMember(dst => dst.ResourceSharingRuleName, ops => ops.MapFrom(src => src.resource_sharing_rule_name))
                    .ForMember(dst => dst.ResourcePath, ops => ops.MapFrom(src => src.resource_path))
                    .ForMember(dst => dst.SharingFromUserId, ops => ops.MapFrom(src => src.sharing_from_user_id))
                    .ForMember(dst => dst.SharingFromMailAddress, ops => ops.MapFrom(src => src.sharing_from_mail_address))
                    .ForMember(dst => dst.SharingToUserId, ops => ops.MapFrom(src => src.sharing_to_user_id))
                    .ForMember(dst => dst.SharingToMailAddress, ops => ops.MapFrom(src => src.sharing_to_mail_address))
                    .ForMember(dst => dst.Query, ops => ops.MapFrom(src => src.query))
                    .ForMember(dst => dst.Script, ops => ops.MapFrom(src => src.script))
                    .ForMember(dst => dst.IsEnable, ops => ops.MapFrom(src => src.is_enable))
                    .ForMember(dst => dst.SharingToVendorId, ops => ops.MapFrom(src => src.sharing_to_vendor_id))
                    .ForMember(dst => dst.SharingToSystemId, ops => ops.MapFrom(src => src.sharing_to_system_id));
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get => s_lazyMapper.Value; }

        /// <summary>
        /// 指定されたリソースパスの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">リソースパス</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public IEnumerable<ResourceSharingPersonRuleModel> GetList(string path)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
select 
    resource_sharing_person_rule_id as ResourceSharingPersonRuleId,
    resource_sharing_rule_name as ResourceSharingRuleName,
    resource_path as ResourcePath,
    sharing_from_user_id as SharingFromUserId,
    sharing_from_mail_address as SharingFromMailAddress,
    sharing_to_user_id as SharingToUserId,
    sharing_to_mail_address as SharingToMailAddress,
    query Query,
    script Script,
    is_enable as IsEnable,
    sharing_to_vendor_id as SharingToVendorId,
    sharing_to_system_id as SharingToSystemId
from RESOURCE_SHARING_PERSON_RULE
where /*ds Path*/'path' like replace(resource_path, '*', '%')
and is_active = 1
order by resource_sharing_rule_name";
            }
            else
            {
                sql = @"
select 
    resource_sharing_person_rule_id as ResourceSharingPersonRuleId,
    resource_sharing_rule_name as ResourceSharingRuleName,
    resource_path as ResourcePath,
    sharing_from_user_id as SharingFromUserId,
    sharing_from_mail_address as SharingFromMailAddress,
    sharing_to_user_id as SharingToUserId,
    sharing_to_mail_address as SharingToMailAddress,
    query Query,
    script Script,
    is_enable as IsEnable,
    sharing_to_vendor_id as SharingToVendorId,
    sharing_to_system_id as SharingToSystemId
from ResourceSharingPersonRule
where @Path like replace(resource_path, '*', '%')
and is_active = 1
order by resource_sharing_rule_name";
            }

            var param = new { Path = path };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<ResourceSharingPersonRuleModel>(twowaySql.Sql, dynParams);
        }


        /// <summary>
        /// 指定されたIDの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="id">個人リソースシェアリングルールID</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public ResourceSharingPersonRuleModel Get(string id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var query = "";
            if (dbSettings.Type == "Oracle")
            {
                query = @"
select 
    resource_sharing_person_rule_id as ResourceSharingPersonRuleId,
    resource_sharing_rule_name as ResourceSharingRuleName,
    resource_path as ResourcePath,
    sharing_from_user_id as SharingFromUserId,
    sharing_from_mail_address as SharingFromMailAddress,
    sharing_to_user_id as SharingToUserId,
    sharing_to_mail_address as SharingToMailAddress,
    query Query,
    script Script,
    is_enable as IsEnable,
    sharing_to_vendor_id as SharingToVendorId,
    sharing_to_system_id as SharingToSystemId
from RESOURCE_SHARING_PERSON_RULE
where is_active = 1
and resource_sharing_person_rule_id = /*ds sharing_rule_id*/'00000000-0000-0000-0000-000000000000' 
order by resource_sharing_rule_name";
            }
            else
            {
                query = @"
select 
    resource_sharing_person_rule_id as ResourceSharingPersonRuleId,
    resource_sharing_rule_name as ResourceSharingRuleName,
    resource_path as ResourcePath,
    sharing_from_user_id as SharingFromUserId,
    sharing_from_mail_address as SharingFromMailAddress,
    sharing_to_user_id as SharingToUserId,
    sharing_to_mail_address as SharingToMailAddress,
    query Query,
    script Script,
    is_enable as IsEnable,
    sharing_to_vendor_id as SharingToVendorId,
    sharing_to_system_id as SharingToSystemId
from ResourceSharingPersonRule
where is_active = 1
and resource_sharing_person_rule_id = @sharing_rule_id
order by resource_sharing_rule_name";
            }

            var param = new { sharing_rule_id = id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), query, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            // 検索実行
            return _connection.Query<ResourceSharingPersonRuleModel>(twowaySql.Sql, dynParams).FirstOrDefault();
        }

        /// <summary>
        /// 個人リソースシェアリングルールを登録します。
        /// </summary>
        /// <param name="model">個人リソースシェアリングルール</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public ResourceSharingPersonRuleModel Register(ResourceSharingPersonRuleModel model)
        {
            // データベース登録用のモデルを生成
            var db_model = s_mapper.Map<DB_ResourceSharingPersonRule>(model);

            // 同一ルール存在チェック
            if (IsExists(db_model)) throw new AlreadyExistsException($"The same settings exist.: {db_model.sharing_to_mail_address}");

            // 管理情報を設定
            db_model.reg_username = Convert.ToString(PerRequestDataContainer.OpenId);
            db_model.reg_date = UtcNow;
            db_model.upd_username = Convert.ToString(PerRequestDataContainer.OpenId);
            db_model.upd_date = UtcNow;
            db_model.is_active = true;
            try
            {
                // 登録実行
                db_model.resource_sharing_person_rule_id = _connection.Insert(db_model);
                // 結果を返却
                return s_mapper.Map<ResourceSharingPersonRuleModel>(db_model);
            }
            catch (SqlException ex)
            {
                // UNIQUE制約違反の場合、独自例外を返す
                if (ex.Number == 2627) throw new AlreadyExistsException(ex.Message);
                else throw new SqlDatabaseException(ex.Message);
            }
        }

        /// <summary>
        /// 個人リソースシェアリングルールを更新します。
        /// </summary>
        /// <param name="model">個人リソースシェアリングルール</param>
        /// <returns>個人リソースシェアリングルール</returns>
        public ResourceSharingPersonRuleModel Update(ResourceSharingPersonRuleModel model)
        {
            // データベース登録用のモデルを生成
            var db_model = s_mapper.Map<DB_ResourceSharingPersonRule>(model);

            // 同一ルール存在チェック
            if (IsExists(db_model)) throw new AlreadyExistsException($"The same settings exist.: {db_model.sharing_to_mail_address}");

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    RESOURCE_SHARING_PERSON_RULE
SET
    resource_sharing_rule_name = /*ds resource_sharing_rule_name*/'1' 
	,resource_path = /*ds resource_path*/'1' 
	,sharing_from_user_id = /*ds sharing_from_user_id*/'1' 
	,sharing_from_mail_address = /*ds sharing_from_mail_address*/'1' 
	,sharing_to_user_id = /*ds sharing_to_user_id*/'1' 
	,sharing_to_mail_address = /*ds sharing_to_mail_address*/'1' 
	,query = /*ds query*/'1' 
	,script = /*ds script*/'1' 
	,is_enable = /*ds is_enable*/1 
    ,upd_date = SYSTIMESTAMP
    ,upd_username = /*ds upd_username*/'1' 
	,sharing_to_vendor_id = /*ds sharing_to_vendor_id*/'1' 
	,sharing_to_system_id = /*ds sharing_to_system_id*/'1' 
WHERE
    resource_sharing_person_rule_id = /*ds resource_sharing_person_rule_id*/'00000000-0000-0000-0000-000000000000' 
    AND is_active = 1
";
            }
            else
            {
                sql = @"
UPDATE
    ResourceSharingPersonRule
SET
    resource_sharing_rule_name = @resource_sharing_rule_name
	,resource_path = @resource_path
	,sharing_from_user_id = @sharing_from_user_id
	,sharing_from_mail_address = @sharing_from_mail_address
	,sharing_to_user_id = @sharing_to_user_id
	,sharing_to_mail_address = @sharing_to_mail_address
	,query = @query
	,script = @script
	,is_enable = @is_enable
    ,upd_date = GETDATE()
    ,upd_username = @upd_username
	,sharing_to_vendor_id = @sharing_to_vendor_id
	,sharing_to_system_id = @sharing_to_system_id
WHERE
    resource_sharing_person_rule_id = @resource_sharing_person_rule_id 
    AND is_active = 1
";
            }

            var param = new {
                resource_sharing_person_rule_id = model.ResourceSharingPersonRuleId,
                resource_sharing_rule_name = model.ResourceSharingRuleName,
                resource_path = model.ResourcePath,
                sharing_from_user_id = model.SharingFromUserId,
                sharing_from_mail_address = model.SharingFromMailAddress,
                sharing_to_user_id = model.SharingToUserId,
                sharing_to_mail_address = model.SharingToMailAddress,
                query = model.Query,
                script = model.Script,
                is_enable = model.IsEnable,
                sharing_to_vendor_id = model.SharingToVendorId,
                sharing_to_system_id = model.SharingToSystemId,
                upd_username = Convert.ToString(PerRequestDataContainer.OpenId)
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param)
                .SetNClob(nameof(param.query))
                .SetNClob(nameof(param.script));
            try
            {
                // 更新実行
                if (_connection.ExecutePrimaryKey(twowaySql.Sql, dynParams) <= 0)
                {
                    throw new NotFoundException($"Not Found ResourceSharingPersonRuleId ={model.ResourceSharingPersonRuleId}");
                }
                // 結果を返却
                return model;
            }
            catch (SqlException ex)
            {
                // UNIQUE制約違反の場合、独自例外を返す
                if (ex.Number == 2627) throw new AlreadyExistsException(ex.Message);
                else throw new SqlDatabaseException(ex.Message);
            }
        }

        /// <summary>
        /// 同一のルールがあるかチェックします。
        /// </summary>
        /// <param name="model">個人リソースシェアリングルール</param>
        /// <returns>true:存在する、false:存在しない</returns>
        private bool IsExists(DB_ResourceSharingPersonRule model)
        {
            /*
             * 取得する対象は以下2件
             * 1. Person to Person (openidがfrom-toで完全一致)
             * 2. Any to Vendor (openidに関する指定がない AND toベンダーが指定されている)
            */

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
select count(1) from RESOURCE_SHARING_PERSON_RULE
where resource_sharing_person_rule_id != /*ds resource_sharing_person_rule_id*/'1' 
and resource_path = /*ds resource_path*/'1' 
and ( 
  ( sharing_from_user_id = /*ds sharing_from_user_id*/'00000000-0000-0000-0000-000000000000'  AND sharing_to_user_id = /*ds sharing_to_user_id*/'00000000-0000-0000-0000-000000000000' )
  OR ( 
        (sharing_from_user_id is null AND sharing_from_mail_address = '*')
        AND (sharing_to_user_id is null AND sharing_to_mail_address = '*')
        AND sharing_to_vendor_id = /*ds sharing_to_vendor_id*/'1' 
        AND sharing_to_system_id = /*ds sharing_to_system_id*/'1' 
    )
)
and sharing_to_user_id = /*ds sharing_to_user_id*/'1' 
and is_enable = 1
and is_active = 1";
            }
            else
            {
                sql = @"
select count(1) from ResourceSharingPersonRule
where resource_sharing_person_rule_id != @resource_sharing_person_rule_id
and resource_path = @resource_path
and ( 
  ( sharing_from_user_id = @sharing_from_user_id  AND sharing_to_user_id = @sharing_to_user_id )
  OR ( 
        (sharing_from_user_id is null AND sharing_from_mail_address = '*')
        AND (sharing_to_user_id is null AND sharing_to_mail_address = '*')
        AND sharing_to_vendor_id = @sharing_to_vendor_id  
        AND sharing_to_system_id = @sharing_to_system_id 
    )
)
and sharing_to_user_id = @sharing_to_user_id
and is_enable = 1
and is_active = 1";
            }

            var param = new
            {
                resource_sharing_person_rule_id = model.resource_sharing_person_rule_id,
                resource_path = model.resource_path,
                sharing_from_user_id = model.sharing_from_user_id,
                sharing_to_user_id = model.sharing_to_user_id,
                sharing_to_vendor_id = model.sharing_to_vendor_id,
                sharing_to_system_id = model.sharing_to_system_id
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            // 検索実行
            var result = _connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            // 結果を返却
            return result > 0;
        }

        /// <summary>
        /// 指定されたIDの個人リソースシェアリングルールを削除します。
        /// </summary>
        /// <param name="id">個人リソースシェアリングルールID</param>
        public void Delete(string id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    RESOURCE_SHARING_PERSON_RULE
SET 
    is_active = 0
    ,upd_date = SYSTIMESTAMP
    ,upd_username = /*ds staff_id*/'1' 
WHERE
    resource_sharing_person_rule_id = /*ds resource_sharing_person_rule_id*/'1' 
    AND is_active = 1
";
            }
            else
            {
                sql = @"
UPDATE 
    ResourceSharingPersonRule
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@staff_id
WHERE
    resource_sharing_person_rule_id=@resource_sharing_person_rule_id
    AND is_active=1
";
            }
            var param = new { resource_sharing_person_rule_id = id, staff_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }
    }
}
