using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    [Log]
    [TransactionScope]
    interface ICacheApplicationService
    {
        bool IsStaticCache { get; }

        void RefreshStaticCache();
        void CheckStaticCacheTime();
    }
}
