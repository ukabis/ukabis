using JP.DataHub.Batch.LoggingEventProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingEventProcess.Services.Interfaces
{
    public interface ILoggingService
    {

        ProviderVendorSystemModel GetProviderVendorSystemId(string apiId);

        void SendSummaryEvent(SendSummaryEventModel eventModel);

        void RegistLoggingInfo(LoggingInfoModel loggingInfoModel);
    }
}
