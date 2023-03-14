using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.Com.Transaction
{
    public interface IJPDataHubEventHub
    {
        string ConnectionString { get; set; }

        Task<bool> SendMessageAsync(JToken message, string partitionKey = null);
    }
}
