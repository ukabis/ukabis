using System.Net;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    class ScriptRuntimeLogFileRepository : AbstractRepository, IScriptRuntimeLogFileRepository
    {
        private static readonly string s_containername = UnityCore.Resolve<string>("ScriptRuntimeLogFileBlobContainerName");

        private IBlobStorage _blob => UnityCore.Resolve<IBlobStorage>("ScriptRuntimeFileBlobStorage");


        public ScriptRuntimeLogGetFile Get(Guid logId, Guid vendorId)
        {
            try
            {
                var file = _blob.GetAppendBlobAsync(s_containername, $"{vendorId}/{logId}/Runtime.log").Result;
                return new ScriptRuntimeLogGetFile("Runtime.log", vendorId, logId, file.OpenReadAsync().Result, file.Properties.ContentType);
            }
            catch (AggregateException ae) when (ae.InnerException is StorageException se)
            {
                if (se.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    throw new NotFoundException(se.Message);
                }
                throw;
            }
        }

        public async Task<Uri> AppendAsync(ScriptRuntimeLogAppendFile file)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(file.AppendContent.Value)))
            {
                return await _blob.AppendBlobAsync(s_containername, file.FilePathIncludeName(), ms, file.ContentType.Value);
            }
        }

        public bool Delete(Guid logId, Guid vendorId)
        {
            try
            {
                return _blob.DeleteAppendBlobAsync(s_containername, $"{vendorId.ToString()}/{logId.ToString()}/Runtime.log").Result;
            }
            catch (AggregateException ae) when (ae.InnerException is StorageException se)
            {
                if (se.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    throw new NotFoundException(se.Message);
                }
                throw;
            }
        }
    }
}