using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal class NotificationService : INotificationService
    {
        private INotificationService _internal;

        public NotificationService(string connectionString)
        {
            var pcs = new ParseConnectionString(connectionString);
            var type = "Type";
            _internal = NotificationServiceFactory.Create(pcs.GetValue(type), pcs.GetWithoutValue(type));
        }

        public async Task SendObjectAsync(object obj)
            => _internal?.SendObjectAsync(obj);

        public async Task SendAsync(string message)
            => _internal?.SendAsync(message);
    }
}
