using System.Net;
using Microsoft.AspNetCore.Mvc;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Interface;

namespace JP.DataHub.ApiWeb.Controllers
{
    [ApiController]
    [Route("API/[controller]/[action]")]
    [ManageApi("72833834-6339-42BB-9625-99578D410C67")]
    [AuthorizeUsingOpenIdConnect]
    public class AsyncApiController : AbstractController
    {
        [HttpGet]
        [ManageAction("B7FAECC0-E3E4-44FF-8E16-6566760F3EB7")]
        public IActionResult GetStatus(string requestId)
        {
            try
            {
                return new OkObjectResult(Api.GetStatus(requestId));
            }
            catch (AsyncApiStatusNotFoundException)
            {
                return new NotFoundObjectResult(new { Message = $"RequestId={requestId} Not Found" });
            }
        }

        [HttpGet]
        [ManageAction("663C4C2E-0821-45AE-A71C-1C5C114FF6EE")]
        public IActionResult GetResult(string requestId)
        {
            try
            {
                return ToActionResult(Api.GetResult(requestId));
            }
            catch (AsyncApiStatusNotFoundException)
            {
                return new BadRequestObjectResult(new { Message = $"RequestId={requestId} Not Found" });
            }
            catch (AsyncApiNotEndException)
            {
                return new BadRequestObjectResult(new { Message = $"RequestId={requestId} Not Found" });

            }
            catch (AsyncApiResultNotFoundException)
            {
                return new ObjectResult(new { Message = $"RequestId={requestId} Result File Not Found" }) { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
    }
}
