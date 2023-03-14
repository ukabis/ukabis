using AutoMapper;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Information;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using Microsoft.VisualBasic;
using System.Text;
using Unity;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class InformationRepository : AbstractRepository, IInformationRepository
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DB_Information, InformationModel>()
                        .ForMember(dst => dst.InformationId, ops => ops.MapFrom(src => src.information_id))
                        .ForMember(dst => dst.Title, ops => ops.MapFrom(src => src.title))
                        .ForMember(dst => dst.Detail, ops => ops.MapFrom(src => src.detail))
                        .ForMember(dst => dst.Date, ops => ops.MapFrom(src => src.date))
                        .ForMember(dst => dst.IsVisibleApi, ops => ops.MapFrom(src => src.is_visible_api))
                        .ForMember(dst => dst.IsVisibleAdmin, ops => ops.MapFrom(src => src.is_visible_admin));
                cfg.CreateMap<InformationModel, DB_Information>()
                        .ForMember(dst => dst.information_id, ops => ops.MapFrom(src => src.InformationId))
                        .ForMember(dst => dst.title, ops => ops.MapFrom(src => src.Title))
                        .ForMember(dst => dst.detail, ops => ops.MapFrom(src => src.Detail))
                        .ForMember(dst => dst.date, ops => ops.MapFrom(src => src.Date))
                        .ForMember(dst => dst.is_visible_api, ops => ops.MapFrom(src => src.IsVisibleApi))
                        .ForMember(dst => dst.is_visible_admin, ops => ops.MapFrom(src => src.IsVisibleAdmin));
            });
            return mappingConfig.CreateMapper();
        });

        private static IMapper _mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Information"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        //[CacheKey(CacheKeyType.Id, "information", "informationlist", "getInformationCount", "is_visible_api", "is_visible_admin")]
        public static string CACHE_KEY_INFORMATION_LIST = "InformationRepository.GetList";

        protected readonly TimeSpan cacheExpireTime = TimeSpan.Parse("24:00:00");


        public InformationModel Get(string informationId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    information_id
    ,title
    ,detail
    ,inf_date as ""DATE""
    ,is_visible_api
    ,is_visible_admin
FROM
    INFORMATION
WHERE
    is_active=1
    AND information_id = /*ds information_id*/'id'
";
            }
            else
            {
                sql = @"
SELECT
    information_id
    ,title
    ,detail
    ,date
    ,is_visible_api
    ,is_visible_admin
FROM
    Information
WHERE
    is_active=1
    AND information_id = @information_id
";

            }
            var param = new { information_id = informationId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var information = _connection.Query<DB_Information>(twowaySql.Sql, dynParams).FirstOrDefault();
            if (information == null)
            {
                throw new NotFoundException($"Not Found Information id={informationId}");
            }
            return _mapper.Map<InformationModel>(information);
        }

        [Cache]
        [CacheArg(allParam: true)]
        [CacheEntity(InformationDatabase.TABLE_INFORMATION)]
        public List<InformationModel> GetList(int? getInformationCount, bool isVisibleApi, bool isVisibleAdmin)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string baseSql;
            if (dbSettings.Type == "Oracle")
            {
                baseSql = @"
SELECT
    information_id
    ,title
    ,detail
    ,inf_date as ""DATE""
    ,is_visible_api
    ,is_visible_admin
FROM
    INFORMATION
WHERE
    is_active=1
/*ds if getInformationCount != null*/
FETCH FIRST /*ds getInformationCount*/1 ROWS ONLY
/*ds end if*/
";
            }
            else
            {
                baseSql = @"
SELECT
/*ds if getInformationCount != null*/
    TOP (@getInformationCount)  
/*ds end if*/
    information_id
    ,title
    ,detail
    ,date
    ,is_visible_api
    ,is_visible_admin
FROM
    Information
WHERE
    is_active=1
";
            }

            var sb = new StringBuilder(baseSql);

            // 表示先のデータのみ取得する場合
            sb.Append(this.CreateInformationVisibleSQL(isVisibleApi, isVisibleAdmin));

            string order = @"
ORDER BY
    date DESC
";

            sb.Append(order);
            var sql = sb.ToString();
            var param = new { getInformationCount = getInformationCount };
            var dict = new Dictionary<string, object>();
            dict.Add("getInformationCount", getInformationCount);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var list = _connection.Query<DB_Information>(twowaySql.Sql, dynParams).ToList();
            if (list == null || !list.Any())
            {
                throw new NotFoundException();
            }
            return _mapper.Map<List<InformationModel>>(list);
        }

        [CacheEntityFire(InformationDatabase.TABLE_INFORMATION)]
        public InformationModel Registration(InformationModel information)
        {
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);
            if(Guid.Empty == information.InformationId)
            {
                information.InformationId =  Guid.NewGuid();
            }

            DB_Information dbInformation = new DB_Information
            {
                information_id = information.InformationId,
                title = information.Title,
                detail = information.Detail,
                date = information.Date,
                is_visible_api = information.IsVisibleApi,
                is_visible_admin = information.IsVisibleAdmin,
                reg_username = updUserId,
                reg_date = UtcNow,
                upd_username = updUserId,
                upd_date = UtcNow,
                is_active = true,
            };
            _connection.Insert(dbInformation);

            return information;
        }

        [CacheEntityFire(InformationDatabase.TABLE_INFORMATION)]
        public void Delete(string informationId)
        {
            var dateTimeUtil = this.PerRequestDataContainer.GetDateTimeUtil();
            var now = dateTimeUtil.GetUtc(dateTimeUtil.LocalNow);
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    INFORMATION
SET
    is_active = 0
    ,upd_date = /*ds UpdDateTime*/'2000-01-01'
    ,upd_username = /*ds UpdUserId*/'id'
WHERE
    information_id = /*ds InformationId*/'id' and is_active = 1
";
            }
            else
            {
                sql = @"
UPDATE
    Information
SET
    is_active = 0
    ,upd_date = @UpdDateTime
    ,upd_username = @UpdUserId
WHERE
    information_id = @InformationId and is_active = 1
";
            }
            var param = new { InformationId = informationId, UpdDateTime = now, UpdUserId = updUserId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                UpdDateTime = true,
                UpdUserId = true,
                InformationId = true,
            });
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            if (_connection.Execute(twowaySql.Sql, dynParams) <= 0)
            {
                throw new NotFoundException($"Not Found Information id={informationId}");
            };
        }

        [CacheEntityFire(InformationDatabase.TABLE_INFORMATION)]
        public void Update(InformationModel information)
        {
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    INFORMATION
SET
    title = /*ds Title*/'t'
    ,detail = /*ds Detail*/'d'
    ,inf_date = /*ds Date*/'2000-01-01'
    ,upd_date = /*ds UpdDateTime*/'2000-01-01'
    ,upd_username = /*ds UpdUserId*/'id'
    ,is_visible_api = /*ds IsVisibleApi*/1
    ,is_visible_admin = /*ds IsVisibleAdmin*/1
WHERE
    information_id = /*ds InformationId*/'id'
";
            }
            else
            {
                sql = @"
UPDATE
    Information
SET
    title = @Title
    ,detail = @Detail
    ,[date] = @Date
    ,upd_date = @UpdDateTime
    ,upd_username = @UpdUserId
    ,is_visible_api = @IsVisibleApi
    ,is_visible_admin = @IsVisibleAdmin
WHERE
    information_id = @InformationId
";
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                Title = true,
                Detail = true,
                Date = true,
                UpdDateTime = true,
                UpdUserId = true,
                IsVisibleApi = true,
                IsVisibleAdmin = true,
                InformationId = true,
            });

            var param = new
            {
                InformationId = information.InformationId,
                Title = information.Title,
                Detail = information.Detail,
                Date = information.Date,
                IsVisibleApi = information.IsVisibleApi,
                IsVisibleAdmin = information.IsVisibleAdmin,
                UpdDateTime = UtcNow,
                UpdUserId = updUserId
            };
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param).SetNClob(nameof(param.Detail));

            if (_connection.Execute(twowaySql.Sql, dynParams) <= 0)
            {
                throw new NotFoundException($"Not Found Information id={information.InformationId}");
            }
        }
        private string CreateInformationVisibleSQL(bool isVisibleApi, bool isVisibleAdmin)
        {
            var visibleSql = new StringBuilder();
            if (isVisibleApi)
            {
                visibleSql.Append(@" and is_visible_api = 1 ");
            }

            if (isVisibleAdmin)
            {
                visibleSql.Append(@" and is_visible_admin = 1 ");
            }
            return visibleSql.ToString();
        }
    }
}