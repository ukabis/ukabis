using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class OpenIdUserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string OpenId { get; set; }
    }
}
