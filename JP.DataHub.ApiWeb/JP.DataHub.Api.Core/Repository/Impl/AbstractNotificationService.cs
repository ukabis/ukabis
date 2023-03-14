using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal abstract class AbstractNotificationService : INotificationService
    {
        public Task SendObjectAsync(object obj)
            => SendAsync(obj.ToJsonString());

        public abstract Task SendAsync(string message);
    }
}
