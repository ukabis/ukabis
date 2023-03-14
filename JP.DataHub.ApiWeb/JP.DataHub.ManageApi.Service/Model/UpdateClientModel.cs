using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class UpdateClientModel
    {
        public string ClientId { get; set; }
        public string SystemId { get; set; }

        public string ClientSecret { get; set; }

        public TimeSpan AccessTokenExpirationTimeSpan { get; set; }
    }
}
