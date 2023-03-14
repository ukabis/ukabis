using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Repository
{
    public interface IServiceBusEventRepository
    {
        Task SendObjectAsync(object obj);
        Task SendAsync(string message);
    }
}
