using System;
using JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    public interface ICacheInterface
    {
        bool IsStaticCache { get; }

        void RefreshStaticCache();
        void CheckStaticCacheTime();
    }
}
