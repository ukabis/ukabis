using JP.DataHub.Com.Consts;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Logging;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Core.EventNotify;
using JP.DataHub.Infrastructure.Core.Storage;
using JP.DataHub.MVC.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.Interception.Utilities;

namespace JP.DataHub.Infrastructure.Core.Logging
{
    public class LoggingInterceptor : ILoggingInterceptor
    {
        private Lazy<IStorageClient> _loggingTemp = new Lazy<IStorageClient>(() => UnityCore.Resolve<IStorageClient>("LoggingTemp"));
        private IStorageClient loggingTemp => _loggingTemp.Value;

        private Lazy<IStorageClient> _loggingOfficial = new Lazy<IStorageClient>(() => UnityCore.Resolve<IStorageClient>("Logging"));
        private IStorageClient loggingOfficial => _loggingOfficial.Value;

        private Lazy<IDataContainer> _dataContainer = new Lazy<IDataContainer>(() => UnityCore.Resolve<IDataContainer>());
        private IDataContainer dataContainer => _dataContainer.Value;

        private Lazy<JPDataHubLogger> _logger = new Lazy<JPDataHubLogger>(() => new JPDataHubLogger(typeof(LoggingInterceptor)));
        private JPDataHubLogger Logger => _logger.Value;


        private string loggingId;
        private ApiRequestResponseLogModel model;
        private bool isInternalCall = false;
        private bool noLogging = false;
        private bool isOutputLoggingId = false;

        private Lazy<MeteringSettings> _meteringSettings = new Lazy<MeteringSettings>(() => UnityCore.Resolve<MeteringSettings>());
        private MeteringSettings meteringSettings { get => _meteringSettings.Value; }

        private Lazy<VendorAuthenticationDefaultSettings> _vendorSystemAuthenticationDefaultSettings = new Lazy<VendorAuthenticationDefaultSettings>(() => UnityCore.Resolve<VendorAuthenticationDefaultSettings>());
        private VendorAuthenticationDefaultSettings VendorSystemAuthenticationDefaultSettings { get => _vendorSystemAuthenticationDefaultSettings.Value; }

        public Task BeforeTask { get; private set; }
        public Task AfterTask { get; private set; }
        public bool IsNoLogging { get => noLogging; }

        public LoggingInterceptor()
        {
        }

        public Stream Before(string loggingId, DateTime request_time, string http_method_type, string relative_url, Stream request_body, string query_string, Dictionary<string, List<string>> header, string media_type, string content_type, long? content_length)
        {
            noLogging = false;
            isInternalCall = dataContainer.FindObjectPath<bool>("IsInternalCall");
            if (isInternalCall == true/*internal call*/ && meteringSettings.InternalApi == false)
            {
                noLogging = true;
                return request_body;
            }
            if (isInternalCall == false/*external call*/ && meteringSettings.ExternalApi == false)
            {
                noLogging = true;
                return request_body;
            }

            this.loggingId = loggingId;
            string request_body_filename = $"{loggingId}/request.body.bin";
            string request_meta_filename = $"{loggingId}/request.meta.txt";
            isOutputLoggingId = header.ContainsKey(HeaderConst.X_GetInternalAllField);

            // request body
            loggingTemp.CopyTo(request_body_filename, request_body);

            // メタの書き込みから、Logging置き場への配置は別スレッド
            // 勝手に終われば十分（待たない思想）
            // 落ちたら知らないw

            BeforeTask = Task.Run(async () =>
            {
                // request meta
                var meta = new LoggingRequestMeta();
                meta.http_method_type = http_method_type;
                meta.relative_url = relative_url;
                meta.query_string = query_string;
                meta.header = header;
                meta.media_type = media_type;
                meta.content_type = content_type;
                meta.content_length = content_length;
                using (var metastream = meta.ToJson().ToString().ToStream())
                {
                    loggingTemp.CopyTo(request_meta_filename, metastream);

                    // request body local to logging置き場
                    using (var stream = loggingTemp.GetStream(request_body_filename))
                    {
                        loggingOfficial.CopyTo(request_body_filename, stream);
                    }

                    // request meta local to logging置き場
                    metastream.Seek(0, SeekOrigin.Begin);
                    loggingOfficial.CopyTo(request_meta_filename, metastream);

                    // Notify用のモデル作成
                    model = new ApiRequestResponseLogModel();
                    model.LogId = loggingId;
                    model.HttpMethodType = http_method_type;
                    model.RequestContentType = content_type;
                    model.RequestContentLength = content_length;
                    model.RequestBody = $"$file({request_body_filename})";
                    model.RequestHeaders = $"$file({request_meta_filename})";
                    model.VendorId = dataContainer.FindObjectPath<string>("VendorId");
                    model.SystemId = dataContainer.FindObjectPath<string>("SystemId");
                    if (string.IsNullOrEmpty(model.VendorId) && string.IsNullOrEmpty(model.SystemId) ||
                        (model.VendorId == "00000000-0000-0000-0000-000000000000" &&
                         model.SystemId == "00000000-0000-0000-0000-000000000000"))
                    {
                        model.VendorId = VendorSystemAuthenticationDefaultSettings.VendorId;
                        model.SystemId = VendorSystemAuthenticationDefaultSettings.SystemId;
                    }

                    model.OpenId = dataContainer.FindObjectPath<string>("OpenId");
                    model.ClientIpAddress = RequestUtil.GetRequestIP();
                    model.RequestDate = request_time;
                    model.Url = relative_url;
                    model.QueryString = query_string;
                    model.Status = ApiRequestResponseLogModel.LoggingEventStatusEnum.Request;

                    var eventNotify = UnityCore.Resolve<IEventNotifyProvider>("LoggingNotify-Multithread");
                    _ = await eventNotify.NotifyAsync(model.ToJson().ToString());
                    var tranman = UnityCore.Resolve<IJPDataHubTransactionManager>("Multithread");
                    if (tranman?.Any() == true)
                    {
                        tranman.ForEach(x => x.Dispose());
                        tranman.Clear();
                    }
                }
            });
            /* .ContinueWith(t =>
         {
             var logger = new JPDataHubLogger(typeof(LoggingInterceptor));
             logger.Error("Logging Before Error", t.Exception);
         }, TaskContinuationOptions.NotOnRanToCompletion);*/

            // contentsすげ替え用
            return loggingTemp.GetStream(request_body_filename);
        }

        public (int StatusCode, Dictionary<string, List<string>> Header, Stream Stream) After(string loggingId, TimeSpan execute_time, int statusCode, Dictionary<string, List<string>> header, Stream stream)
        {
            string response_body_filename = $"{loggingId}/response.body.bin";
            string response_meta_filename = $"{loggingId}/response.meta.txt";

            // response body
            loggingTemp.CopyTo(response_body_filename, stream);
            var dataContainer = JP.DataHub.Com.Unity.UnityCore.Resolve<IDataContainer>();

            // Notifyは別スレッド
            // 勝手に終われば十分（待たない思想）
            AfterTask = Task.Run(async () =>
            {
                // response meta
                var meta = new LoggingResponseMeta();
                meta.status_code = statusCode;
                meta.header = header;
                using (var metastream = meta.ToJson().ToString().ToStream())
                {
                    loggingTemp.CopyTo(response_meta_filename, metastream);
                    long size = loggingTemp.GetSize(response_meta_filename);
                    using (var loggingStream = loggingTemp.GetStream(response_meta_filename))
                    {
                        loggingOfficial.CopyTo(response_meta_filename, loggingStream);
                    }

                    Task.WaitAll(BeforeTask);   // modelのインスタンスを待つ
                    model.HttpStatusCode = statusCode;
                    model.ResponseContentLength = size;
                    model.ResponseBody = $"$file({response_body_filename})";
                    model.ResponseHeaders = $"$file({response_meta_filename})";
                    model.ResourceId = dataContainer.FindObjectPath<string>("ResourceId");
                    model.ApiId = dataContainer.FindObjectPath<string>("ApiId");
                    model.ControllerName = dataContainer.FindObjectPath<string>("ControllerName");
                    model.ActionName = dataContainer.FindObjectPath<string>("ActionName");
                    model.ExecuteTime = execute_time;
                    model.Status = ApiRequestResponseLogModel.LoggingEventStatusEnum.Response;
                    var eventNotify = UnityCore.Resolve<IEventNotifyProvider>("LoggingNotify-Multithread");
                    _ = await eventNotify.NotifyAsync(model.ToJson().ToString());
                    var tranman = UnityCore.Resolve<IJPDataHubTransactionManager>("Multithread");
                    if (tranman?.Any() == true)
                    {
                        Logger.Info("tranman count: " + tranman.Count);
                        tranman.ForEach(x => x.Dispose());
                        tranman.Clear();
                    }
                }
            }).ContinueWith(t =>
            {
                var logger = new JPDataHubLogger(typeof(LoggingInterceptor));
                logger.Error("Logging After Error", t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);

            // responseすげ替え
            if (isOutputLoggingId)
            {
                header.Add(HeaderConst.LoggingLogId, loggingId);    // LoggingIdをヘッダーに混ぜる
            }
            return ((int)statusCode, header, loggingTemp.GetStream(response_body_filename));
        }
    }
}
