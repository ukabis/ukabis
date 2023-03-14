using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class AuthorizeResultModel
    {
        public string OpenId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public bool Result { get; set; }
        public string[] FunctionList { get; set; }
    }
}
