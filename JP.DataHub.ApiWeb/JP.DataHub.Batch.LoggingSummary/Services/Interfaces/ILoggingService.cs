using JP.DataHub.Batch.LoggingSummary.Models;

namespace JP.DataHub.Batch.LoggingSummary.Services.Interfaces
{
    public interface ILoggingService
    {
        void SummaryVendorSystemYmdHm(SummaryCommandModel commandModel);

        void SummaryProviderVendorSystemYmdHm(SummaryCommandModel commandModel);

        void SummaryVendorSystemYmdH(SummaryCommandModel commandModel);

        void SummaryProviderVendorSystemYmdH(SummaryCommandModel commandModel);

        void SummaryVendorSystemYmd(SummaryCommandModel commandModel);

        void SummaryProviderVendorSystemYmd(SummaryCommandModel commandModel);
    }
}
