using System.Text;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Transaction;
using Oci.Common.Auth;
using Oci.StreamingService;
using Oci.StreamingService.Models;
using Oci.StreamingService.Requests;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal class StreamingServiceNotificationService : AbstractNotificationService, INotificationService
    {
        private static readonly JPDataHubLogger s_logger = new JPDataHubLogger(typeof(StreamingServiceNotificationService));

        private string _connectionString;

        public StreamingServiceNotificationService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override async Task SendAsync(string message)
        {
            var pcs = new ParseConnectionString(_connectionString);
            var configurationFilePath = pcs.GetValue("ConfigurationFilePath");
            var profile = pcs.GetValue("Profile");
            var pemFilePath = pcs.GetValue("PemFilePath");
            var ocid = pcs.GetValue("Ocid");
            var endPoint = pcs.GetValue("EndPoint");

            try
            {
                var pemFile = new FilePrivateKeySupplier(pemFilePath, null);
                var provider = new ConfigFileAuthenticationDetailsProvider(configurationFilePath, profile, pemFile);
                using var streamClient = new StreamClient(provider);
                streamClient.SetEndpoint(endPoint);


                var messages = new List<PutMessagesDetailsEntry>();
                var detailsEntry = new PutMessagesDetailsEntry
                {
                    Key = Encoding.UTF8.GetBytes($"NotificationService"),
                    Value = Encoding.UTF8.GetBytes(message)
                };
                messages.Add(detailsEntry);

                var putRequest = new PutMessagesRequest
                {
                    StreamId = ocid,
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
    }
}
