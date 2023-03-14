using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Models
{
    public class AuthorizationViewModel
    {
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string OpenId { get; set; }
        public bool Result { get; set; }
        public List<string> FunctionList { get; set; }
    }
}
