using JP.DataHub.Batch.LoggingSummary.Models;
using JP.DataHub.Batch.LoggingSummary.Repository.Interfaces;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingSummary.Repository.Impls
{
    /// <summary>
    /// Enumに文字列を付加するためのAttributeクラス
    /// </summary>
    public class OracleNameAttribute : Attribute
    {
        public string OracleName { get; set; }

        public OracleNameAttribute(string oracleName)
        {
            this.OracleName = oracleName;
        }
    }

    public static class EnumToOracleName
    {
        public static string ToOracleName(this LoggingRepository.SummaryTable value)
        {
            Type enumType = value.GetType();
            FieldInfo fieldInfo = enumType.GetField(value.ToString());
            OracleNameAttribute[] attribute = fieldInfo.GetCustomAttributes(typeof(OracleNameAttribute), false) as OracleNameAttribute[];
            return attribute[0].OracleName;
        }

        public static string GetTableName(this LoggingRepository.SummaryTable value, TwowaySqlParser.DatabaseType dbType = TwowaySqlParser.DatabaseType.Oracle)
        {
            var tableName = value.ToString();
            if (dbType == TwowaySqlParser.DatabaseType.Oracle)
            {   // Oracle
                Type enumType = value.GetType();
                FieldInfo fieldInfo = enumType.GetField(value.ToString());
                OracleNameAttribute[] attribute = fieldInfo.GetCustomAttributes(typeof(OracleNameAttribute), false) as OracleNameAttribute[];
                tableName = attribute[0].OracleName;
            }
            else
            {   // SQL server
                tableName = value.ToString();
            }
            return tableName;
        }
    }

    public class LoggingRepository : ILoggingRepository
    {
#if Oracle
        private readonly IEventHubStreamingService EventHub;
#else
        private readonly IJPDataHubEventHub EventHub;
#endif

        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(LoggingRepository));

        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("Logging"));

        private static int commandTimeout;

        private string MergeSummaryOtherSql
        {
            get
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
MERGE INTO {0} target USING (
select DISTINCT
       /*ds date_start*/'' as request_date,
       {2},
       {3},
       controller_id,
       api_id,
       FIRST_VALUE(TO_NCHAR(url)) OVER (PARTITION BY {2},{3},controller_id,api_id ORDER by request_date DESC) as url,
       FIRST_VALUE(TO_NCHAR(httpmethodtype)) OVER (PARTITION BY {2},{3},controller_id,api_id ORDER by request_date DESC) as httpmethodtype,
       FIRST_VALUE(vendor_name) OVER (PARTITION BY {2},{3},controller_id,api_id ORDER by request_date DESC) as vendor_name,
       FIRST_VALUE(system_name) OVER (PARTITION BY {2},{3},controller_id,api_id ORDER by request_date DESC) as system_name,
       sum(execute_time) OVER (PARTITION BY {2},{3},controller_id,api_id ) as execute_time,
       sum(execute_count) OVER (PARTITION BY {2},{3},controller_id,api_id ) as execute_count, 
       sum(successes) OVER (PARTITION BY {2},{3},controller_id,api_id ) as successes,
       sum(failure) OVER (PARTITION BY {2},{3},controller_id,api_id ) as failure,
       sum(running) OVER (PARTITION BY {2},{3},controller_id,api_id ) as running,
       sum(request_contentlength) OVER (PARTITION BY {2},{3},controller_id,api_id ) as request_contentlength, 
       sum(response_contentlength) OVER (PARTITION BY {2},{3},controller_id,api_id ) as response_contentlength
from {1}
where api_id = /*ds api_id*/'' and {2} = /*ds vendor_id*/'' and {3} = /*ds system_id*/'' and
    Request_Date >= /*ds date_start*/'' and
    Request_Date < /*ds date_end*/'' 
                ) source
            ON
            (target.{2} = source.{2} and 
            target.{3} = source.{3} and 
            target.api_id = source.api_id and 
            target.request_date = source.request_date)  
            WHEN MATCHED THEN
                UPDATE
            SET
              url = source.url,
              httpmethodtype = source.httpmethodtype,
              vendor_name = source.vendor_name,
              system_name =source.system_name,
              execute_time = source.execute_time,
              execute_count = source.execute_count,
              successes = source.successes,
              failure = source.failure,
              running = source.running,
              request_contentlength = source.request_contentlength,
              response_contentlength = source.response_contentlength,
              reg_date = SYS_EXTRACT_UTC(SYSTIMESTAMP)

            WHEN NOT MATCHED THEN
            INSERT
            (
                summary_id,
                {2},
                {3},
                controller_id,
                api_id,
                request_date,
                url,
                httpmethodtype,
                vendor_name,
                system_name,
                execute_time,
                execute_count,
                successes,
                failure,
                running,
                request_contentlength,
                response_contentlength,
                reg_date 

            )
            VALUES
                (
                NEWID(),
                source.{2},
                source.{3},
                source.controller_id,
                source.api_id,
                source.request_date,
                source.url,
                source.httpmethodtype,
                source.vendor_name,
                source.system_name,
                source.execute_time,
                source.execute_count,
                source.successes,
                source.failure,
                source.running,
                source.request_contentlength,
                source.response_contentlength,
                SYS_EXTRACT_UTC(SYSTIMESTAMP)  
                )";
                }
                else
                {
                    sql = @"
MERGE INTO {0} AS target USING (
select DISTINCT
       @date_start as request_date,
       {2},
       {3},
       controller_id,
       api_id,
       FIRST_VALUE(url) OVER (PARTITION BY {2},{3},controller_id,api_id ORDER by request_date DESC) as url,
       FIRST_VALUE(httpmethodtype) OVER (PARTITION BY {2},{3},controller_id,api_id ORDER by request_date DESC) as httpmethodtype,
       FIRST_VALUE(vendor_name) OVER (PARTITION BY {2},{3},controller_id,api_id ORDER by request_date DESC) as vendor_name,
       FIRST_VALUE(system_name) OVER (PARTITION BY {2},{3},controller_id,api_id ORDER by request_date DESC) as system_name,
       sum(execute_time) OVER (PARTITION BY {2},{3},controller_id,api_id ) as execute_time,
       sum(execute_count) OVER (PARTITION BY {2},{3},controller_id,api_id ) as execute_count, 
       sum(successes) OVER (PARTITION BY {2},{3},controller_id,api_id ) as successes,
       sum(failure) OVER (PARTITION BY {2},{3},controller_id,api_id ) as failure,
       sum(running) OVER (PARTITION BY {2},{3},controller_id,api_id ) as running,
       sum(request_contentlength) OVER (PARTITION BY {2},{3},controller_id,api_id ) as request_contentlength, 
       sum(response_contentlength) OVER (PARTITION BY {2},{3},controller_id,api_id ) as response_contentlength
from {1}
where api_id = @api_id and {2} = @vendor_id and {3} = @system_id and
    Request_Date >= @date_start and
    Request_Date < @date_end
                ) AS source
            ON
            target.{2} = source.{2} and 
            target.{3} = source.{3} and 
            target.api_id = source.api_id and 
            target.request_date = source.request_date  
            WHEN MATCHED THEN
                UPDATE
            SET
              url = source.url,
              httpmethodtype = source.httpmethodtype,
              vendor_name = source.vendor_name,
              system_name =source.system_name,
              execute_time = source.execute_time,
              execute_count = source.execute_count,
              successes = source.successes,
              failure = source.failure,
              running = source.running,
              request_contentlength = source.request_contentlength,
              response_contentlength = source.response_contentlength,
              reg_date = GETUTCDATE()

            WHEN NOT MATCHED THEN
            INSERT
            (
                summaryId,
                {2},
                {3},
                controller_id,
                api_id,
                request_date,
                url,
                httpmethodtype,
                vendor_name,
                system_name,
                execute_time,
                execute_count,
                successes,
                failure,
                running,
                request_contentlength,
                response_contentlength,
                reg_date 

            )
            VALUES
                (
                NEWID(),
                source.{2},
                source.{3},
                source.controller_id,
                source.api_id,
                source.request_date,
                source.url,
                source.httpmethodtype,
                source.vendor_name,
                source.system_name,
                source.execute_time,
                source.execute_count,
                source.successes,
                source.failure,
                source.running,
                source.request_contentlength,
                source.response_contentlength,
                GETUTCDATE()  
                );";
                }
                return sql;
            }
        }

        private string MergeSummaryYmdHmSql
        {
            get
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
            MERGE INTO {0} target
                USING
            (
                SELECT
            /*ds {1}*/'' AS system_id,
            /*ds {2}*/'' AS vendor_id,
            /*ds api_id*/'' as api_id,
            /*ds request_date*/'' as request_date
            from dual    ) source
            ON
            (target.{1} = source.system_id and 
            target.{2} = source.vendor_id and 
            target.api_id = source.api_id and 
            target.request_date = source.request_date)  
            WHEN MATCHED THEN
                UPDATE
            SET
              url = /*ds url*/'' ,
              httpmethodtype = /*ds httpmethodtype*/'' ,
              vendor_name = /*ds vendor_name*/'' ,
              system_name =/*ds system_name*/'' ,
              execute_time = /*ds execute_time*/'' ,
              execute_count = /*ds execute_count*/'' ,
              successes = /*ds successes*/'' ,
              failure = /*ds failure*/'' ,
              running = /*ds running*/'' ,
              request_contentlength = /*ds request_contentlength*/'' ,
              response_contentlength = /*ds response_contentlength*/'' ,
              reg_date=SYS_EXTRACT_UTC(SYSTIMESTAMP)  

            WHEN NOT MATCHED THEN
            INSERT
            (
                summary_id,
                {2},
                {1},
                controller_id,
                api_id,
                request_date,
                url,
                httpmethodtype,
                vendor_name,
                system_name,
                execute_time,
                execute_count,
                successes,
                failure,
                running,
                request_contentlength,
                response_contentlength,
                reg_date

            )
            VALUES
                (
                NEWID(),
                /*ds {2}*/'' ,
                /*ds {1}*/'' ,
                /*ds controller_id*/'' ,
                /*ds api_id*/'' ,
                /*ds request_date*/'' ,
                /*ds url*/'' ,
                /*ds httpmethodtype*/'' ,
                /*ds vendor_name*/'' ,
                /*ds system_name*/'' ,
                /*ds execute_time*/'' ,
                /*ds execute_count*/'' ,
                /*ds successes*/'' ,
                /*ds failure*/'' ,
                /*ds running*/'' ,
                /*ds request_contentlength*/'' ,
                /*ds response_contentlength*/'' ,
                SYS_EXTRACT_UTC(SYSTIMESTAMP)
                )";
                }
                else
                {
                    sql = @"
            MERGE INTO {0} AS target
                USING
            (
                SELECT
            @{1} AS system_id,
            @{2} AS vendor_id,
            @api_id as api_id,
            @request_date as request_date
                ) AS source
            ON
            target.{1} = source.system_id and 
            target.{2} = source.vendor_id and 
            target.api_id = source.api_id and 
            target.request_date = source.request_date  
            WHEN MATCHED THEN
                UPDATE
            SET
              url = @url,
              httpmethodtype = @httpmethodtype,
              vendor_name = @vendor_name,
              system_name =@system_name,
              execute_time = @execute_time,
              execute_count = @execute_count,
              successes = @successes,
              failure = @failure,
              running = @running,
              request_contentlength = @request_contentlength,
              response_contentlength = @response_contentlength,
              reg_date=GETUTCDATE()  

            WHEN NOT MATCHED THEN
            INSERT
            (
                summaryId,
                {2},
                {1},
                controller_id,
                api_id,
                request_date,
                url,
                httpmethodtype,
                vendor_name,
                system_name,
                execute_time,
                execute_count,
                successes,
                failure,
                running,
                request_contentlength,
                response_contentlength,
                reg_date

            )
            VALUES
                (
                NEWID(),
                @{2},
                @{1},
                @controller_id,
                @api_id,
                @request_date,
                @url,
                @httpmethodtype,
                @vendor_name,
                @system_name,
                @execute_time,
                @execute_count,
                @successes,
                @failure,
                @running,
                @request_contentlength,
                @response_contentlength,
                GETUTCDATE()
                );";
                }

                return sql;
            }
        }

#if Oracle
        public LoggingRepository(IEventHubStreamingService eventHub, IConfiguration config)
        {
            EventHub = eventHub;
            commandTimeout = int.Parse(config.GetValue<string>("LoggingSummary:CommandTimeOut") ?? "180");
        }
#else
        public LoggingRepository(IJPDataHubEventHub eventHub, IConfiguration config)
        {
            EventHub = eventHub;
            commandTimeout = int.Parse(config.GetValue<string>("LoggingSummary:CommandTimeOut") ?? "180");
        }
#endif

        public void SummaryVendorSystemYmdHm(SummaryCommandModel summaryCommand)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var selectSummary = CreateSummarySQL(TargetDate.request_date_ymdhm, TargetVendor.vendor_id, TargetSystem.system_id, dbSettings);
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("requestdate", summaryCommand.RequestDate);
            twowaySqlParam.Add("apiid", summaryCommand.ApiId);
            twowaySqlParam.Add("vendorid", summaryCommand.VendorId);
            twowaySqlParam.Add("systemid", summaryCommand.SystemId);
            var twowaySelectSummary = new TwowaySqlParser(dbSettings.GetDbType(), selectSummary, twowaySqlParam);
            var param = new
            {
                requestdate = summaryCommand.RequestDate,
                apiid = summaryCommand.ApiId,
                vendorid = summaryCommand.VendorId,
                systemid = summaryCommand.SystemId
            };
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var summaryResult = Connection.Query<SummaryResultModel>(twowaySelectSummary.Sql, dynParams, commandTimeout: commandTimeout).FirstOrDefault();
            if (summaryResult == null) return;

            var upsertParam = new
            {
                api_id = summaryResult.api_id,
                controller_id = summaryResult.controller_id,
                system_id = summaryResult.system_id,
                vendor_id = summaryResult.vendor_id,
                vendor_name = summaryResult.vendor_name,
                system_name = summaryResult.system_name,
                url = summaryResult.url,
                httpmethodtype = summaryResult.httpmethodtype,
                request_date = summaryResult.requestdate,
                execute_count = summaryResult.execute_count,
                successes = summaryResult.successes,
                failure = summaryResult.failure,
                running = summaryResult.running,
                request_contentlength = summaryResult.request_contentlength,
                response_contentlength = summaryResult.response_contentlength,
                execute_time = summaryResult.ConvertExecuteTime,
                reg_date = DateTime.UtcNow

            };

            var sql = string.Format(MergeSummaryYmdHmSql, SummaryTable.VendorSystemSummaryYmdHm.GetTableName(dbSettings.GetDbType()),
                TargetSystem.system_id.ToString(), TargetVendor.vendor_id.ToString());
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                api_id = true,
                controller_id = true,
                system_id = true,
                vendor_id = true,
                vendor_name = true,
                system_name = true,
                url = true,
                httpmethodtype = true,
                request_date = true,
                execute_count = true,
                successes = true,
                failure = true,
                running = true,
                request_contentlength = true,
                response_contentlength = true,
                execute_time = true,
                reg_date = true
            });
            dynParams = dbSettings.GetParameters().AddDynamicParams(upsertParam);
            try
            {
                Connection.Execute(twowaySql.Sql, dynParams, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(upsertParam);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        public void SummaryVendorSystemYmdH(SummaryCommandModel summaryCommand)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();

            var sql = string.Format(MergeSummaryOtherSql, SummaryTable.VendorSystemSummaryYmdH.GetTableName(dbSettings.GetDbType()),
                SummaryTable.VendorSystemSummaryYmdHm.GetTableName(dbSettings.GetDbType()), TargetVendor.vendor_id, TargetSystem.system_id);
            var parameter = new
            {
                api_id = summaryCommand.ApiId,
                vendor_id = summaryCommand.VendorId,
                system_id = summaryCommand.SystemId,
                date_start = summaryCommand.RequestDate,
                date_end = summaryCommand.RequestDate.AddHours(1)
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                api_id = true,
                vendor_id = true,
                system_id = true,
                date_start = true,
                date_end = true,
            });
            var dynParams = dbSettings.GetParameters().AddDynamicParams(parameter);
            try
            {
                Connection.Execute(twowaySql.Sql, dynParams, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(parameter);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        public void SummaryVendorSystemYmd(SummaryCommandModel summaryCommand)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = string.Format(MergeSummaryOtherSql, SummaryTable.VendorSystemSummaryYmd.GetTableName(dbSettings.GetDbType()),
                SummaryTable.VendorSystemSummaryYmdH.GetTableName(dbSettings.GetDbType()), TargetVendor.vendor_id, TargetSystem.system_id);
            var parameter = new
            {
                api_id = summaryCommand.ApiId,
                vendor_id = summaryCommand.VendorId,
                system_id = summaryCommand.SystemId,
                date_start = summaryCommand.RequestDate,
                date_end = summaryCommand.RequestDate.AddDays(1)
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                api_id = true,
                vendor_id = true,
                system_id = true,
                date_start = true,
                date_end = true,
            });
            var dynParams = dbSettings.GetParameters().AddDynamicParams(parameter);
            try
            {
                Connection.Execute(twowaySql.Sql, dynParams, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(parameter);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }


        public void SummaryProviderVendorSystemYmdHm(SummaryCommandModel summaryCommand)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var selectSummary = CreateSummarySQL(TargetDate.request_date_ymdhm, TargetVendor.provider_vendorid, TargetSystem.provider_systemid, dbSettings);
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("requestdate", summaryCommand.RequestDate);
            twowaySqlParam.Add("apiid", summaryCommand.ApiId);
            twowaySqlParam.Add("vendorid", summaryCommand.ProviderVendorId);
            twowaySqlParam.Add("systemid", summaryCommand.ProviderSystemId);
            var twowaySelectSummary = new TwowaySqlParser(dbSettings.GetDbType(), selectSummary, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var summaryResult = Connection.Query<SummaryResultModel>(twowaySelectSummary.Sql, dynParams, commandTimeout: commandTimeout).FirstOrDefault();
            if (summaryResult == null)
            {
                return;
            }

            var upsertParam = new
            {
                api_id = summaryResult.api_id,
                controller_id = summaryResult.controller_id,
                provider_systemid = summaryResult.system_id,
                provider_vendorid = summaryResult.vendor_id,
                vendor_name = summaryResult.vendor_name,
                system_name = summaryResult.system_name,
                url = summaryResult.url,
                httpmethodtype = summaryResult.httpmethodtype,
                request_date = summaryResult.requestdate,
                execute_count = summaryResult.execute_count,
                successes = summaryResult.successes,
                failure = summaryResult.failure,
                running = summaryResult.running,
                request_contentlength = summaryResult.request_contentlength,
                response_contentlength = summaryResult.response_contentlength,
                execute_time = summaryResult.ConvertExecuteTime,
                reg_date = DateTime.UtcNow


            };
            var sql = string.Format(MergeSummaryYmdHmSql, SummaryTable.ProviderVendorSystemSummaryYmdHm.GetTableName(dbSettings.GetDbType()),
                TargetSystem.provider_systemid.ToString(), TargetVendor.provider_vendorid.ToString());

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                api_id = true,
                controller_id = true,
                provider_systemid = true,
                provider_vendorid = true,
                vendor_name = true,
                system_name = true,
                url = true,
                httpmethodtype = true,
                request_date = true,
                execute_count = true,
                successes = true,
                failure = true,
                running = true,
                request_contentlength = true,
                response_contentlength = true,
                execute_time = true,
                reg_date = true
            });
            dynParams = dbSettings.GetParameters().AddDynamicParams(upsertParam);
            try
            {
                Connection.Execute(twowaySql.Sql, dynParams, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(upsertParam);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }
        public void SummaryProviderVendorSystemYmdH(SummaryCommandModel summaryCommand)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = string.Format(MergeSummaryOtherSql, SummaryTable.ProviderVendorSystemSummaryYmdH.GetTableName(dbSettings.GetDbType()),
                SummaryTable.ProviderVendorSystemSummaryYmdHm.GetTableName(dbSettings.GetDbType()), TargetVendor.provider_vendorid,
                TargetSystem.provider_systemid);
            var parameter = new
            {
                api_id = summaryCommand.ApiId,
                vendor_id = summaryCommand.ProviderVendorId,
                system_id = summaryCommand.ProviderSystemId,
                date_start = summaryCommand.RequestDate,
                date_end = summaryCommand.RequestDate.AddHours(1)
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                api_id = true,
                vendor_id = true,
                system_id = true,
                date_start = true,
                date_end = true,
            });
            var dynParams = dbSettings.GetParameters().AddDynamicParams(parameter);
            try
            {
                Connection.Execute(twowaySql.Sql, dynParams, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(parameter);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        public void SummaryProviderVendorSystemYmd(SummaryCommandModel summaryCommand)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = string.Format(MergeSummaryOtherSql, SummaryTable.ProviderVendorSystemSummaryYmd.GetTableName(dbSettings.GetDbType()),
                SummaryTable.ProviderVendorSystemSummaryYmdH.GetTableName(dbSettings.GetDbType()), TargetVendor.provider_vendorid,
                TargetSystem.provider_systemid);
            var parameter = new
            {
                api_id = summaryCommand.ApiId,
                vendor_id = summaryCommand.ProviderVendorId,
                system_id = summaryCommand.ProviderSystemId,
                date_start = summaryCommand.RequestDate,
                date_end = summaryCommand.RequestDate.AddDays(1)
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                api_id = true,
                vendor_id = true,
                system_id = true,
                date_start = true,
                date_end = true,
            });
            var dynParams = dbSettings.GetParameters().AddDynamicParams(parameter);
            try
            {
                Connection.Execute(twowaySql.Sql, dynParams, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(parameter);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        private enum TargetDate
        {
            request_date_ymd,
            request_date_ymdh,
            request_date_ymdhm,
        }

        private enum TargetVendor
        {
            vendor_id,
            provider_vendorid
        }

        private enum TargetSystem
        {
            system_id,
            provider_systemid
        }

        public enum SummaryTable
        {
            [OracleName("Vendor_System_Summary_YmdHm")]
            VendorSystemSummaryYmdHm,
            [OracleName("Provider_Vendor_System_Summary_YmdHm")]
            ProviderVendorSystemSummaryYmdHm,
            [OracleName("Vendor_System_Summary_YmdH")]
            VendorSystemSummaryYmdH,
            [OracleName("Provider_Vendor_System_Summary_YmdH")]
            ProviderVendorSystemSummaryYmdH,
            [OracleName("Vendor_System_Summary_Ymd")]
            VendorSystemSummaryYmd,
            [OracleName("Provider_Vendor_System_Summary_Ymd")]
            ProviderVendorSystemSummaryYmd
        }

        private dynamic CreateDBLogging(LoggingInfoModel loggingInfo)
        {
            return new
            {
                log_id = loggingInfo.LogId,
                action_name = loggingInfo.ActionName,
                api_id = Guid.Parse(loggingInfo.ApiId),
                client_ipaddress = loggingInfo.ClientIpAddress,
                controller_id = Guid.Parse(loggingInfo.ControllerId),
                controller_name = loggingInfo.ControllerName,
                execute_time = loggingInfo.ExecuteTime.Ticks,
                httpmethodtype = loggingInfo.HttpMethodType,
                httpstatus_code = loggingInfo.HttpStatusCode,
                open_id = string.IsNullOrEmpty(loggingInfo.OpenId) ? null : (Guid?)Guid.Parse(loggingInfo.OpenId),
                provider_systemid = string.IsNullOrEmpty(loggingInfo.ProviderSystemId) ? null : (Guid?)Guid.Parse(loggingInfo.ProviderSystemId),
                provider_vendorid = string.IsNullOrEmpty(loggingInfo.ProviderVendorId) ? null : (Guid?)Guid.Parse(loggingInfo.ProviderVendorId),
                querystring = loggingInfo.QueryString,
                requestbody = loggingInfo.RequestBody,
                request_contentlength = loggingInfo.RequestContentLength,
                request_contenttype = loggingInfo.RequestContentType,
                request_date = loggingInfo.RequestDate,
                request_headers = loggingInfo.RequestHeaders,
                response_body = loggingInfo.ResponseBody,
                response_contentlength = loggingInfo.ResponseContentLength,
                response_contenttype = loggingInfo.ResponseContentType,
                response_headers = loggingInfo.ResponseHeaders,
                system_id = string.IsNullOrEmpty(loggingInfo.SystemId) ? null : (Guid?)Guid.Parse(loggingInfo.SystemId),
                url = loggingInfo.Url,
                vendor_id = string.IsNullOrEmpty(loggingInfo.VendorId) ? null : (Guid?)Guid.Parse(loggingInfo.VendorId),
                request_date_ymd = loggingInfo.RequestDateYmd,
                request_date_ymdh = loggingInfo.RequestDateYmdH,
                request_date_ymdhm = loggingInfo.RequestDateYmdHM,
                reg_date = DateTime.UtcNow,
                is_internal_call = loggingInfo.IsInternalCall
            };
        }

        private string CreateSummarySQL(TargetDate targetDate, TargetVendor targetVendor, TargetSystem targetSystem, Com.Settings.DatabaseSettings dbSettings)
        {
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"SELECT 
                 log.{0} as requestdate
                ,log.controller_id
                ,log.api_id
                ,(case when api.api_url IS NOT NULL then controller_api.controller_url || '/' || api.api_url ELSE controller_api.controller_url END) as url
                ,TO_CHAR(httpmethodtype) AS httpmethodtype
                ,log.{1} as system_id
                ,log.{2} as vendor_id
                ,sys.system_name
                ,vendor.vendor_name
                ,count(log.api_id) as execute_count ,
            SUM(case when httpstatus_code >= '200' and httpstatus_code < '300'
            then 1
            when httpstatus_code = '404'
            then 1
            ELSE 0 END) as successes ,
            SUM(case
            when httpstatus_code != '404' and httpstatus_code != '0' and NOT(httpstatus_code >= '200' and httpstatus_code < '300')
            then 1
            ELSE 0 END) as failure,
            SUM(case
            when httpstatus_code = '0'
            then 1
            ELSE 0 END) as running,
            sum(execute_time) as execute_time,
            SUM(request_contentlength) as request_contentlength,
            SUM(response_contentlength) as response_contentlength
            FROM logging  log /*with(nolock)*/
            LEFT JOIN api_info api /*with(nolock)*/ ON log.api_id = api.api_id
            LEFT JOIN controller_info controller_api /*with(nolock)*/ ON log.controller_id = controller_api.controller_id
            INNER JOIN system sys /*with(nolock)*/ ON log.{1} = sys.system_id
            INNER JOIN vendor  vendor /*with(nolock)*/ ON log.{2} = vendor.vendor_id
            where log.api_id = /*ds apiid*/'' and log.{2} = /*ds vendorid*/'' and log.{1} = /*ds systemid*/'' and
                {0} = /*ds requestdate*/'' 
            group by
                 log.{0}
                ,log.controller_id
                ,log.api_id
                ,(case when api.api_url IS NOT NULL then controller_api.controller_url || '/' || api.api_url ELSE controller_api.controller_url END)
                ,TO_CHAR(httpmethodtype)
                ,log.{2}
                ,log.{1}
                ,sys.system_name
                ,vendor.vendor_name";
            }
            else
            {
                sql = @"SELECT 
                 log.{0} as requestdate
                ,log.controller_id
                ,log.api_id
                ,(case when api.api_url IS NOT NULL then controller_api.controller_url + '/' + api.api_url ELSE controller_api.controller_url END) as url
                ,httpmethodtype
                ,log.{1} as system_id
                ,log.{2} as vendor_id
                ,sys.system_name
                ,vendor.vendor_name
                ,count(log.api_id) as execute_count ,
            SUM(case when httpstatus_code >= '200' and httpstatus_code < '300'
            then 1
            when httpstatus_code = '404'
            then 1
            ELSE 0 END) as successes ,
            SUM(case
            when httpstatus_code != '404' and httpstatus_code != '0' and NOT(httpstatus_code >= '200' and httpstatus_code < '300')
            then 1
            ELSE 0 END) as failure,
            SUM(case
            when httpstatus_code = '0'
            then 1
            ELSE 0 END) as running,
            sum(execute_time) as execute_time,
            SUM(request_contentlength) as request_contentlength,
            SUM(response_contentlength) as response_contentlength
            FROM Logging  log with(nolock)
            LEFT JOIN ApiInfo api with(nolock) ON log.api_id = api.api_id
            LEFT JOIN ControllerInfo controller_api with(nolock) ON log.controller_id = controller_api.controller_id
            INNER JOIN System sys with(nolock) ON log.{1} = sys.system_id
            INNER JOIN vendor  vendor with(nolock) ON log.{2} = vendor.vendor_id
            where log.api_id = @apiid and log.{2} = @vendorid and log.{1} = @systemid and
                {0} = @requestdate
            group by
                 log.{0}
                ,log.controller_id
                ,log.api_id
                ,(case when api.api_url IS NOT NULL then controller_api.controller_url + '/' + api.api_url ELSE controller_api.controller_url END)
                ,httpmethodtype
                ,log.{2}
                ,log.{1}
                ,sys.system_name
                ,vendor.vendor_name";
            }
            return string.Format(sql, targetDate.ToString(), targetSystem.ToString(), targetVendor.ToString());
        }
    }
}