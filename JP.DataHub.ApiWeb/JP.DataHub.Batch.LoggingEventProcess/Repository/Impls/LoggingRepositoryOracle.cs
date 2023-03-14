using JP.DataHub.Batch.LoggingEventProcess.Models;
using JP.DataHub.Batch.LoggingEventProcess.Repository.Interfaces;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.TimeZone;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Oracle;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using TimeZoneConverter;
using Unity;

namespace JP.DataHub.Batch.LoggingEventProcess.Repository.Impls
{
    public class LoggingRepositoryOracle :  ILoggingRepository
    {
        private readonly IEventHubStreamingService EventHub;

        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(LoggingRepositoryOracle));

        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("Logging"));

        private static int commandTimeout;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="eventHub"></param>
        /// <param name="config"></param>
        public LoggingRepositoryOracle(IEventHubStreamingService eventHub, IConfiguration config)
        {
            EventHub = eventHub;
            commandTimeout = int.Parse(config.GetValue<string>("LoggingSummary:CommandTimeOut") ?? "180");
        }

        /// <summary>
        /// MergeSummaryOtherSql
        /// ※パラメータはUpperCamelCaseなので注意
        /// </summary>
        private const string MergeSummaryOtherSql = @"
MERGE INTO {0} target USING (
select DISTINCT
       :DateStart as request_date,
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
where api_id = :ApiId and {2} = :VendorId and {3} = :SystemId and
    Request_Date >= :DateStart and
    Request_Date < :DateEnd
                ) source
            ON (
            target.{2} = source.{2} and 
            target.{3} = source.{3} and 
            target.api_id = source.api_id and 
            target.request_date = source.request_date ) 
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

        /// <summary>
        /// MergeSummaryYmdHmSql
        /// ※パラメータはUpperCamelCaseなので注意
        /// </summary>
        private const string MergeSummaryYmdHmSql = @"
            MERGE INTO {0} target
                USING
            (
                SELECT
            :{1} AS system_id,
            :{2} AS vendor_id,
            :ApiId as api_id,
            :RequestDate as request_date
            FROM DUAL
                ) source
            ON (
            target.{1} = source.system_id and 
            target.{2} = source.vendor_id and 
            target.api_id = source.api_id and 
            target.request_date = source.request_date ) 
            WHEN MATCHED THEN
                UPDATE
            SET
              url = :Url,
              httpmethodtype = :HttpMethodType,
              vendor_name = :VendorName,
              system_name =:SystemName,
              execute_time = :ExecuteTime,
              execute_count = :ExecuteCount,
              successes = :Successes,
              failure = :Failure,
              running = :Running,
              request_contentlength = :RequestContentLength,
              response_contentlength = :ResponseContentLength,
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
                :{2},
                :{1},
                :ControllerId,
                :ApiId,
                :RequestDate,
                :Url,
                :HttpMethodType,
                :VendorName,
                :SystemName,
                :ExecuteTime,
                :ExecuteCount,
                :Successes,
                :Failure,
                :Running,
                :RequestContentLength,
                :ResponseContentLength,
                GETUTCDATE()
                );";

        public ProviderVendorSystemModel GetProviderVendorSystemId(string apiId)
        {
            var sql = "select provider_vendorid, provider_systemid from Api_Info a join Controller_Info c on c.controller_id = a.controller_id where api_id = :ApiId";
            var param = new { ApiId = apiId };

            dynamic ret = Connection.Query<dynamic>(sql, param);
            if (ret != null && ret.Count > 0)
            {
                return new ProviderVendorSystemModel(ret[0].PROVIDER_VENDORID.ToString(), ret[0].PROVIDER_SYSTEMID.ToString());
            }

            return null;
        }

        public void SendSummaryEvent(SendSummaryEventModel summaryEvent)
        {
            summaryEvent.RequestDate = System.TimeZoneInfo.ConvertTimeFromUtc(summaryEvent.RequestDate, TZConvert.GetTimeZoneInfo("Tokyo Standard Time"));
            EventHub.SendMessageAsync(JToken.FromObject(summaryEvent), $"{summaryEvent.ControllerId}{summaryEvent.ApiId}{summaryEvent.VendorId}{summaryEvent.SystemId}{summaryEvent.ProviderVendorId}{summaryEvent.ProviderSystemId}{summaryEvent.RequestDate}");
        }

        public void RegistLoggingInfo(LoggingInfoModel loggingInfo)
        {
            var sql = @"
MERGE INTO logging target
USING
(
    SELECT :LogId AS log_id
    FROM DUAL
) source
ON (target.log_id = source.log_id)
WHEN MATCHED THEN
    UPDATE SET
        action_name = :ActionName,
        api_id = :ApiId,
        controller_id = :ControllerId,
        controller_name = :ControllerName,
        execute_time = :ExecuteTime,
        httpmethodtype = :HttpMethodType,
        httpstatus_code = :HttpStatusCode,
        open_id = :OpenId,
        client_ipaddress = :ClientIpAddress,
        provider_systemid = :ProviderSystemId,
        provider_vendorid = :ProviderVendorId,
        querystring = :QueryString,
        requestbody = :RequestBody,
        request_contentlength = :RequestContentLength,
        request_contenttype = :RequestContentType,
        request_date = :RequestDate,
        request_headers = :RequestHeaders,
        response_body = :ResponseBody,
        response_contentlength = :ResponseContentLength,
        response_contenttype = :ResponseContentType,
        response_headers = :ResponseHeaders,
        system_id = :SystemId,
        url  = :Url,
        vendor_id = :VendorId,
        request_date_ymd = :RequestDateYmd,
        request_date_ymdh = :RequestDateYmdH,
        request_date_ymdhm = :RequestDateYmdHM,
        reg_date = :RegDate,
        is_internal_call = :IsInternalCall
                
WHEN NOT MATCHED THEN
    INSERT
    (
        log_id,
        action_name,
        api_id,
        controller_id,
        controller_name,
        execute_time,
        httpmethodtype,
        httpstatus_code,
        open_id,
        provider_systemid,
        provider_vendorid,
        querystring,
        requestbody,
        client_ipaddress,
        request_contentlength,
        request_contenttype,
        request_date,
        request_headers,
        response_body,
        response_contentlength,
        response_contenttype,
        response_headers,
        system_id,
        url,
        vendor_id,
        request_date_ymd,
        request_date_ymdh,
        request_date_ymdhm,
        reg_date,
        is_internal_call
    )
    VALUES
        (
        :LogId,
        :ActionName,
        :ApiId,
        :ControllerId,
        :ControllerName,
        :ExecuteTime,
        :HttpMethodType,
        :HttpStatusCode,
        :OpenId,
        :ProviderSystemId,
        :ProviderVendorId,
        :QueryString,
        :RequestBody,
        :ClientIpAddress,
        :RequestContentLength,
        :RequestContentType,
        :RequestDate,
        :RequestHeaders,
        :ResponseBody,
        :ResponseContentLength,
        :ResponseContentType,
        :ResponseHeaders,
        :SystemId,
        :Url,
        :VendorId,
        :RequestDateYmd,
        :RequestDateYmdH,
        :RequestDateYmdHM,
        :RegDate,
        :IsInternalCall
    ) ";

            var dbLogging = CreateDBLogging(loggingInfo);
            try
            {
                Connection.Execute(sql, dbLogging, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(dbLogging);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        public void RegistBeginLoggingInfo(LoggingInfoModel loggingInfo)
        {
            var sql = @"
MERGE INTO logging target
USING
(
    SELECT :LogId AS log_id
    FROM DUAL
) source
ON (target.log_id = source.log_id)
WHEN NOT MATCHED THEN
    INSERT
    (
        log_id,
        action_name,
        api_id,
        controller_id,
        controller_name,
        execute_time,
        httpmethodtype,
        httpstatus_code,
        open_id,
        provider_systemid,
        provider_vendorid,
        querystring,
        client_ipaddress,
        request_contenttype,
        request_date,
        request_headers,
        system_id,
        url,
        vendor_id,
        request_date_ymd,
        request_date_ymdh,
        request_date_ymdhm,
        reg_date,
        request_contentlength,
        response_contentlength,
        is_internal_call
    )
    VALUES
        (
        :LogId,
        :ActionName,
        :ApiId,
        :ControllerId,
        :ControllerName,
        :ExecuteTime,
        :HttpMethodType,
        :HttpStatusCode,
        :OpenId,
        :ProviderSystemId,
        :ProviderVendorId,
        :QueryString,
        :ClientIpAddress,
        :RequestContentType,
        :RequestDate,
        :RequestHeaders,
        :SystemId,
        :Url,
        :VendorId,
        :RequestDateYmd,
        :RequestDateYmdH,
        :RequestDateYmdHM,
        :RegDate,
        0,
        0,
        :IsInternalCall
    ); ";

            var dbLogging = CreateDBLogging(loggingInfo);
            try
            {
                Connection.Execute(sql, dbLogging, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(dbLogging);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        public void RegistRequestLoggingInfo(LoggingInfoModel loggingInfo)
        {
            var sql = @"
MERGE INTO logging target
USING
(
    SELECT :LogId AS log_id
    FROM DUAL
) source
ON (target.log_id = source.log_id)
WHEN MATCHED THEN
    UPDATE SET
        requestbody = :RequestBody,
        request_contentlength = :RequestContentLength,
        reg_date = :RegDate,
        is_internal_call = :IsInternalCall

WHEN NOT MATCHED THEN
    INSERT
    (
        log_id,
        action_name,
        api_id,
        controller_id,
        controller_name,
        execute_time,
        httpmethodtype,
        httpstatus_code,
        open_id,
        provider_systemid,
        provider_vendorid,
        querystring,
        requestbody,
        client_ipaddress,
        request_contentlength,
        request_contenttype,
        request_date,
        request_headers,
        system_id,
        url,
        vendor_id,
        request_date_ymd,
        request_date_ymdh,
        request_date_ymdhm,
        reg_date,
        response_contentlength,
        is_internal_call
    )
    VALUES
        (
        :LogId,
        :ActionName,
        :ApiId,
        :ControllerId,
        :ControllerName,
        :ExecuteTime,
        :HttpMethodType,
        :HttpStatusCode,
        :OpenId,
        :ProviderSystemId,
        :ProviderVendorId,
        :QueryString,
        :RequestBody,
        :ClientIpAddress,
        :RequestContentLength,
        :RequestContentType,
        :RequestDate,
        :RequestHeaders,
        :SystemId,
        :Url,
        :VendorId,
        :RequestDateYmd,
        :RequestDateYmdH,
        :RequestDateYmdHM,
        :RegDate,
        0,
        :IsInternalCall
    ); ";

            var dbLogging = CreateDBLogging(loggingInfo);

            try
            {
                Connection.Execute(sql, dbLogging, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(dbLogging);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        public void RegistResponseLoggingInfo(LoggingInfoModel loggingInfo)
        {
            var sql = @"
MERGE INTO logging target
USING
(
    SELECT :LogId AS log_id
    FROM DUAL
) source
ON (target.log_id = source.log_id)
WHEN MATCHED THEN
    UPDATE SET
        response_body = :ResponseBody,
        response_contentlength = :ResponseContentLength,
        response_contenttype = :ResponseContentType,
        response_headers = :ResponseHeaders,
        httpstatus_code = :HttpStatusCode,
        execute_time = :ExecuteTime,
        reg_date = :RegDate,
        is_internal_call = :IsInternalCall
                
WHEN NOT MATCHED THEN
    INSERT
    (
        log_id,
        action_name,
        api_id,
        controller_id,
        controller_name,
        execute_time,
        httpmethodtype,
        httpstatus_code,
        open_id,
        provider_systemid,
        provider_vendorid,
        querystring,
        client_ipaddress,
        request_contenttype,
        request_date,
        request_headers,
        response_body,
        response_contentlength,
        response_contenttype,
        response_headers,
        system_id,
        url,
        vendor_id,
        request_date_ymd,
        request_date_ymdh,
        request_date_ymdhm,
        reg_date,
        request_contentlength,
        is_internal_call
    )
    VALUES
        (
        :LogId,
        :ActionName,
        :ApiId,
        :ControllerId,
        :ControllerName,
        :ExecuteTime,
        :HttpMethodType,
        :HttpStatusCode,
        :OpenId,
        :ProviderSystemId,
        :ProviderVendorId,
        :QueryString,
        :ClientIpAddress,
        :RequestContentType,
        :RequestDate,
        :RequestHeaders,
        :ResponseBody,
        :ResponseContentLength,
        :ResponseContentType,
        :ResponseHeaders,
        :SystemId,
        :Url,
        :VendorId,
        :RequestDateYmd,
        :RequestDateYmdH,
        :RequestDateYmdHM,
        :RegDate,
        0,
        :IsInternalCall
    ); ";

            var dbLogging = CreateDBLogging(loggingInfo);
            try
            {
                Connection.Execute(sql, dbLogging, commandTimeout: commandTimeout);
            }
            catch (SqlException ex)
            {
                string parameters = JsonConvert.SerializeObject(dbLogging);
                logger.Error($"Sql Error: {ex.Number} {ex.Message}\nSql: {sql}\nParameter: {parameters}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        private dynamic CreateDBLoggingOld(LoggingInfoModel loggingInfo)
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

        private dynamic CreateDBLogging(LoggingInfoModel m)
        {
            var p = new OracleDynamicParameters();
            p.Add(nameof(m.LogId), m.LogId, OracleMappingType.Char);
            p.Add(nameof(m.ActionName), m.ActionName, OracleMappingType.NClob);
            p.Add(nameof(m.ApiId), m.ApiId, OracleMappingType.Char);
            p.Add(nameof(m.ClientIpAddress), m.ClientIpAddress, OracleMappingType.NClob);
            p.Add(nameof(m.ControllerId), Guid.Parse(m.ControllerId), OracleMappingType.Char);
            p.Add(nameof(m.ControllerName), m.ControllerName, OracleMappingType.NClob);
            p.Add(nameof(m.ExecuteTime), m.ExecuteTime.Ticks, OracleMappingType.Long);
            p.Add(nameof(m.HttpMethodType), m.HttpMethodType, OracleMappingType.NClob);
            p.Add(nameof(m.HttpStatusCode), m.HttpStatusCode, OracleMappingType.NVarchar2);
            p.Add(nameof(m.OpenId), string.IsNullOrEmpty(m.OpenId) ? null : (Guid?)Guid.Parse(m.OpenId), OracleMappingType.Char);
            p.Add(nameof(m.ProviderSystemId), string.IsNullOrEmpty(m.ProviderSystemId) ? null : (Guid?)Guid.Parse(m.ProviderSystemId), OracleMappingType.Char);
            p.Add(nameof(m.ProviderVendorId), string.IsNullOrEmpty(m.ProviderVendorId) ? null : (Guid?)Guid.Parse(m.ProviderVendorId), OracleMappingType.Char);
            p.Add(nameof(m.QueryString), m.QueryString, OracleMappingType.NClob);
            p.Add(nameof(m.RequestBody), m.RequestBody, OracleMappingType.NClob);
            p.Add(nameof(m.RequestContentLength), m.RequestContentLength, OracleMappingType.Long);
            p.Add(nameof(m.RequestContentType), m.RequestContentType, OracleMappingType.NClob);
            p.Add(nameof(m.RequestDate), m.RequestDate, OracleMappingType.TimeStamp);
            p.Add(nameof(m.RequestHeaders), m.RequestHeaders, OracleMappingType.NClob);
            p.Add(nameof(m.ResponseBody), m.ResponseBody, OracleMappingType.NClob);
            p.Add(nameof(m.ResponseContentLength), m.ResponseContentLength, OracleMappingType.Long);
            p.Add(nameof(m.ResponseContentType), m.ResponseContentType, OracleMappingType.NClob);
            p.Add(nameof(m.ResponseHeaders), m.ResponseHeaders, OracleMappingType.NClob);
            p.Add(nameof(m.SystemId), string.IsNullOrEmpty(m.SystemId) ? null : (Guid?)Guid.Parse(m.SystemId), OracleMappingType.Char);
            p.Add(nameof(m.Url), m.Url, OracleMappingType.NClob);
            p.Add(nameof(m.VendorId), string.IsNullOrEmpty(m.VendorId) ? null : (Guid?)Guid.Parse(m.VendorId), OracleMappingType.Char);
            p.Add(nameof(m.RequestDateYmd), m.RequestDateYmd, OracleMappingType.TimeStamp);
            p.Add(nameof(m.RequestDateYmdH), m.RequestDateYmdH, OracleMappingType.TimeStamp);
            p.Add(nameof(m.RequestDateYmdHM), m.RequestDateYmdHM, OracleMappingType.TimeStamp);
            p.Add("RegDate", DateTime.UtcNow, OracleMappingType.TimeStamp);
            p.Add(nameof(m.IsInternalCall), m.IsInternalCall, OracleMappingType.Int16);

            return p;
        }
    }
}