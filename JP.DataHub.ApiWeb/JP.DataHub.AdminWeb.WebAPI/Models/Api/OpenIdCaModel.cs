using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class OpenIdCaModel
    {
        /// <summary>
        /// ApplicationId
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Application名
        /// </summary>
        public string ApplicationName { get; set; }
    }
}
