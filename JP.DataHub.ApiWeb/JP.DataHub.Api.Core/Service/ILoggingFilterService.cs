using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;

namespace JP.DataHub.Api.Core.Service
{
    [TransactionScope]
    internal interface ILoggingFilterService
    {
        void Write(ApiRequestResponseLogModel model);
    }
}
