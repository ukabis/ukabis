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
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;
using Unity;

namespace JP.DataHub.Batch.LoggingEventProcess.Repository.Impls
{
    internal class LoggingRepository :  ILoggingRepository
    {
        private readonly IJPDataHubEventHub EventHub;

        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(LoggingRepository));

        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("Logging"));

        private static int commandTimeout;

        private const string MergeSummaryOtherSql = @"
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

        private const string MergeSummaryYmdHmSql = @"
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

        public LoggingRepository(IJPDataHubEventHub eventHub, IConfiguration config)
        {
            EventHub = eventHub;
            commandTimeout = int.Parse(config.GetValue<string>("LoggingSummary:CommandTimeOut") ?? "180");
        }

        public ProviderVendorSystemModel GetProviderVendorSystemId(string apiId)
        {
            var sql = "select provider_vendorid, provider_systemid from ApiInfo a join ControllerInfo c on c.controller_id = a.controller_id where api_id = @api_id";
            var param = new { api_id = apiId };

            dynamic ret = Connection.Query<dynamic>(sql, param);
            if (ret != null && ret.Count > 0)
            {
                return new ProviderVendorSystemModel(ret[0].provider_vendorid.ToString(), ret[0].provider_systemid.ToString());
            }

            return null;
        }

        public void SendSummaryEvent(SendSummaryEventModel summaryEvent)
        {
            EventHub.ConnectionString = UnityCore.Resolve<IConfiguration>().GetValue<string>("ConnectionStrings:LoggingSummaryEvent");
            summaryEvent.RequestDate = System.TimeZoneInfo.ConvertTimeFromUtc(summaryEvent.RequestDate, TZConvert.GetTimeZoneInfo("Tokyo Standard Time"));
            EventHub.SendMessageAsync(JToken.FromObject(summaryEvent), $"{summaryEvent.ControllerId}{summaryEvent.ApiId}{summaryEvent.VendorId}{summaryEvent.SystemId}{summaryEvent.ProviderVendorId}{summaryEvent.ProviderSystemId}{summaryEvent.RequestDate}");
        }

        public void RegistLoggingInfo(LoggingInfoModel loggingInfo)
        {
            var sql = @"
MERGE INTO logging AS target
USING
(
    SELECT @log_id AS log_id
) AS source
ON target.log_id = source.log_id
WHEN MATCHED THEN
    UPDATE SET
        action_name = @action_name,
        api_id = @api_id,
        controller_id = @controller_id,
        controller_name = @controller_name,
        execute_time = @execute_time,
        httpmethodtype = @httpmethodtype,
        httpstatus_code = @httpstatus_code,
        open_id = @open_id,
        client_ipaddress = @client_ipaddress,
        provider_systemid = @provider_systemid,
        provider_vendorid = @provider_vendorid,
        querystring = @querystring,
        requestbody = @requestbody,
        request_contentlength = @request_contentlength,
        request_contenttype = @request_contenttype,
        request_date = @request_date,
        request_headers = @request_headers,
        response_body = @response_body,
        response_contentlength = @response_contentlength,
        response_contenttype = @response_contenttype,
        response_headers = @response_headers,
        system_id = @system_id,
        url  = @url,
        vendor_id = @vendor_id,
        request_date_ymd = @request_date_ymd,
        request_date_ymdh = @request_date_ymdh,
        request_date_ymdhm = @request_date_ymdhm,
        reg_date = @reg_date,
        is_internal_call = @is_internal_call
                
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
        @log_id,
        @action_name,
        @api_id,
        @controller_id,
        @controller_name,
        @execute_time,
        @httpmethodtype,
        @httpstatus_code,
        @open_id,
        @provider_systemid,
        @provider_vendorid,
        @querystring,
        @requestbody,
        @client_ipaddress,
        @request_contentlength,
        @request_contenttype,
        @request_date,
        @request_headers,
        @response_body,
        @response_contentlength,
        @response_contenttype,
        @response_headers,
        @system_id,
        @url,
        @vendor_id,
        @request_date_ymd,
        @request_date_ymdh,
        @request_date_ymdhm,
        @reg_date,
        @is_internal_call
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

        public void RegistBeginLoggingInfo(LoggingInfoModel loggingInfo)
        {
            var sql = @"
MERGE INTO logging AS target
USING
(
    SELECT @log_id AS log_id
) AS source
ON target.log_id = source.log_id
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
        @log_id,
        @action_name,
        @api_id,
        @controller_id,
        @controller_name,
        @execute_time,
        @httpmethodtype,
        @httpstatus_code,
        @open_id,
        @provider_systemid,
        @provider_vendorid,
        @querystring,
        @client_ipaddress,
        @request_contenttype,
        @request_date,
        @request_headers,
        @system_id,
        @url,
        @vendor_id,
        @request_date_ymd,
        @request_date_ymdh,
        @request_date_ymdhm,
        @reg_date,
        0,
        0,
        @is_internal_call
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
MERGE INTO logging AS target
USING
(
    SELECT @log_id AS log_id
) AS source
ON target.log_id = source.log_id
WHEN MATCHED THEN
    UPDATE SET
        requestbody = @requestbody,
        request_contentlength = @request_contentlength,
        reg_date = @reg_date,
        is_internal_call = @is_internal_call

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
        @log_id,
        @action_name,
        @api_id,
        @controller_id,
        @controller_name,
        @execute_time,
        @httpmethodtype,
        @httpstatus_code,
        @open_id,
        @provider_systemid,
        @provider_vendorid,
        @querystring,
        @requestbody,
        @client_ipaddress,
        @request_contentlength,
        @request_contenttype,
        @request_date,
        @request_headers,
        @system_id,
        @url,
        @vendor_id,
        @request_date_ymd,
        @request_date_ymdh,
        @request_date_ymdhm,
        @reg_date,
        0,
        @is_internal_call
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
MERGE INTO logging AS target
USING
(
    SELECT @log_id AS log_id
) AS source
ON target.log_id = source.log_id
WHEN MATCHED THEN
    UPDATE SET
        response_body = @response_body,
        response_contentlength = @response_contentlength,
        response_contenttype = @response_contenttype,
        response_headers = @response_headers,
        httpstatus_code = @httpstatus_code,
        execute_time = @execute_time,
        reg_date = @reg_date,
        is_internal_call = @is_internal_call
                
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
        @log_id,
        @action_name,
        @api_id,
        @controller_id,
        @controller_name,
        @execute_time,
        @httpmethodtype,
        @httpstatus_code,
        @open_id,
        @provider_systemid,
        @provider_vendorid,
        @querystring,
        @client_ipaddress,
        @request_contenttype,
        @request_date,
        @request_headers,
        @response_body,
        @response_contentlength,
        @response_contenttype,
        @response_headers,
        @system_id,
        @url,
        @vendor_id,
        @request_date_ymd,
        @request_date_ymdh,
        @request_date_ymdhm,
        @reg_date,
        0,
        @is_internal_call
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
    }
}