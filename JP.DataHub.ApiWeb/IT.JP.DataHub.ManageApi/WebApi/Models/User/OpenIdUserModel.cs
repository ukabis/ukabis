using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.User
{
    public class OpenIdUserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string OpenId { get; set; }
    }
    public class OpenIdUserRequestModel
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
