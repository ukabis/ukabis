using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.MVC.Unity;
using JP.DataHub.ApiWeb.Domain.Interface;

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("Token")]
    public class TokenController : UnityAutoInjectionController
    {
        [Dependency]
        public IVendorSystemAuthenticationInterface VendorSystemAuthenticationInterface { get; set; }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult Post([FromForm] TokeRequestModel model)
        {
            if (model.grant_type == null || model.grant_type != "client_credentials")
            {
                return BadRequest(new Dictionary<string, string> { { "erros", "unsupported_grant_type" } });
            }
            if (!Guid.TryParse(model.client_id, out var guid) || string.IsNullOrWhiteSpace(model.client_secret))
            {
                return BadRequest(new Dictionary<string, string> { { "erros", "client_id,client_secret" } });
            }
            var result = VendorSystemAuthenticationInterface.AuthenticateClientId(model.client_id, model.client_secret);
            if (result == null)
            {
                return BadRequest(new Dictionary<string, string> { { "erros", "client_id,client_secret" } });
            }
            return Content(JsonConvert.SerializeObject(result, Formatting.Indented), "application/json");
        }
    }

    public class TokeRequestModel
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
    }
}
