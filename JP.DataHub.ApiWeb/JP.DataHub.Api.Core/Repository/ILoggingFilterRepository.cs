using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Api.Core.Service;

namespace JP.DataHub.Api.Core.Repository
{
    internal interface ILoggingFilterRepository
    {
        void Write(ApiRequestResponseLogModel model);
    }
}
