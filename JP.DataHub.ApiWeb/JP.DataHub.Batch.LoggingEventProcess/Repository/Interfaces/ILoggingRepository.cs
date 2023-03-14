using JP.DataHub.Batch.LoggingEventProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingEventProcess.Repository.Interfaces
{
    public interface ILoggingRepository
    {
        ProviderVendorSystemModel GetProviderVendorSystemId(string apiId);

        void SendSummaryEvent(SendSummaryEventModel summaryEvent);

        void RegistLoggingInfo(LoggingInfoModel loggingInfo);

        void RegistBeginLoggingInfo(LoggingInfoModel loggingInfo);

        void RegistRequestLoggingInfo(LoggingInfoModel loggingInfo);

        void RegistResponseLoggingInfo(LoggingInfoModel loggingInfo);
    }
}
