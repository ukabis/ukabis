using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ApiOpenIdCAModel
    {
        /// <summary>
        /// ApiOpenIdCaId
        /// </summary>
        public string ApiOpenIdCaId { get; set; }

        /// <summary>
        /// ApplicationId
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Application名
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// アクセスコントロール
        /// </summary>
        public string AccessControl { get; set; }
    }
}
