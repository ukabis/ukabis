using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.EventNotify
{
    public interface IEventNotify
    {
        Task<string> NotifyAsync(string eventName, string massage);
        Task<string> NotifyAsync<T>(T massage);

    }
}
