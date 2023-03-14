using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Oci.Common;
using Oci.Common.Auth;
using Oci.Common.Waiters;
using Oci.StreamingService;
using Oci.StreamingService.Models;
using Oci.StreamingService.Requests;
using Oci.StreamingService.Responses;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using AutoMapper;
using System.Diagnostics.Eventing.Reader;
using System.Data;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    public class StreamingServiceEventRepository : IStreamingServiceEventRepository
    {
        /// <summary>
        /// ログ出力する何か
        /// </summary>
        private static readonly JPDataHubLogger s_logger = new JPDataHubLogger(typeof(StreamingServiceEventRepository));

        private bool _isEnable;

        private string _confiFilePath;
        private string _profile;
        private string _pemFilePath;
        private string _streamOcid;
        private string _streamEndPoint;

        private string _key;

        public StreamingServiceEventRepository(string configFilePath, string profile, string pemFilePath, string streamOcid, string streamEndPoint, bool isEnable = true, string key = null)
        {
            _confiFilePath = ExpandExeHome(configFilePath);
            _profile = profile;
            _pemFilePath = ExpandExeHome(pemFilePath);
            _streamOcid = streamOcid;
            _streamEndPoint = streamEndPoint;
            _isEnable = isEnable;
            _key = key;
        }

        public async Task SendObjectAsync(object obj)
            => SendAsync(obj.ToJsonString());

        public async Task SendAsync(string message)
        {
            if (_isEnable == false)
            {
                return;
            }

            try
            {
                var pemFile = new FilePrivateKeySupplier(_pemFilePath, null);
                var provider = new ConfigFileAuthenticationDetailsProvider(_confiFilePath, _profile, pemFile);
                using var streamClient = new StreamClient(provider);
                streamClient.SetEndpoint(_streamEndPoint);


                var messages = new List<PutMessagesDetailsEntry>();
                var detailsEntry = new PutMessagesDetailsEntry
                {
                    Key = Encoding.UTF8.GetBytes(_key != null ? _key : $"StreamingService"),
                    Value = Encoding.UTF8.GetBytes(message)
                };
                messages.Add(detailsEntry);

                var putRequest = new PutMessagesRequest
                {
                    StreamId = _streamOcid,
                    PutMessagesDetails = new PutMessagesDetails { Messages = messages }
                };

                var putResponse = await streamClient.PutMessages(putRequest);

                
                foreach (PutMessagesResultEntry entry in putResponse.PutMessagesResult.Entries)
                {
                    if (!string.IsNullOrEmpty(entry.Error))
                    {
                        s_logger.Error($"Error({entry.Error}): {entry.ErrorMessage}");
                        throw new Exception();
                    }
                }

            }
            catch (Exception e)
            {
                s_logger.Error(e.Message);
                s_logger.Error(e.StackTrace);
            }

        }


        private string ExpandExeHome(string path)
        {
            // "~/"、"~\\" で始まるPathの場合はそのままとする。
            // （OCI.DotNetSDK 側で ユーザーのホームディレクトリのパスのものとしているため）
            if (!(path.StartsWith("~/") || path.StartsWith("~\\")))
            {
                var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                var entryAssemblyDir = Path.GetDirectoryName(entryAssembly.Location);
                path = Path.Combine(entryAssemblyDir, path);
            }
            return path;
        }
    }
}
