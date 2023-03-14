using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class RegisterClientModel
    {
        public Guid SystemId { get; set; }

        public string ClientSecret { get; set; }

        public TimeSpan AccessTokenExpirationTimeSpan { get; set; }
    }
}
