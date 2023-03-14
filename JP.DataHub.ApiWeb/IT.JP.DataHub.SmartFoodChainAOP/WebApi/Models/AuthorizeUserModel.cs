using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class AuthorizeUserModel
    {
        public string ApplicationId { get; set; }
        public string AuthorizeUserId { get; set; }
        public string OpenId { get; set; }
        public string[] PrivateRoleId { get; set; }
        public string[] Functions { get; set; }
    }
}
