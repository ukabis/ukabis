using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    public interface INewDocumentHistoryDataStoreRepository
    {
        Func<string> DefaultFileName { get; set; }
    }
}
