using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Service
{
    public interface IServiceBusEventService
    {
        Task SendObjectAsync(object obj);
        Task SendAsync(string message);
    }
}
