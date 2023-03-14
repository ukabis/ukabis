using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    public interface ILoggingService
    {
        LoggingQueryModel GetLogging(string logId);

        HttpResponseMessage GetRequestBody(string logId);

        HttpResponseMessage GetResponseBody(string logId);
    }
}
