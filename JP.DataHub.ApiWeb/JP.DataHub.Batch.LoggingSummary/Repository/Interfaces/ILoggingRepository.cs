using JP.DataHub.Batch.LoggingSummary.Models;

namespace JP.DataHub.Batch.LoggingSummary.Repository.Interfaces
{
    public interface ILoggingRepository
    {
        void SummaryVendorSystemYmdHm(SummaryCommandModel summaryCommand);

        void SummaryProviderVendorSystemYmdHm(SummaryCommandModel summaryCommand);

        void SummaryVendorSystemYmdH(SummaryCommandModel summaryCommand);

        void SummaryProviderVendorSystemYmdH(SummaryCommandModel summaryCommand);

        void SummaryVendorSystemYmd(SummaryCommandModel summaryCommand);

        void SummaryProviderVendorSystemYmd(SummaryCommandModel summaryCommand);
    }
}