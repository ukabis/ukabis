using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.EventNotify
{
    public interface IEventNotifyProvider
    {
        Task<string> NotifyAsync(string message, string category = null);
    }
}
