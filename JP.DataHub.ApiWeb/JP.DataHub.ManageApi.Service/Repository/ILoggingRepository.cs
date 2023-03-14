using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    public interface ILoggingRepository
    {
        LoggingQueryModel GetLogging(string logId);

        (Stream Stream, string MimeType) GetRequestBody(string logId);

        (Stream Stream, string MimeType) GetResponseBody(string logId);
    }
}
