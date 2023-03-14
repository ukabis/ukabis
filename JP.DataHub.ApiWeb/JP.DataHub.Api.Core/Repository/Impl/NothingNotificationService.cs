using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal class NothingNotificationService : AbstractNotificationService, INotificationService
    {
        public NothingNotificationService(string connectionString)
        {
        }

        public override async Task SendAsync(string message)
        {
        }
    }
}
