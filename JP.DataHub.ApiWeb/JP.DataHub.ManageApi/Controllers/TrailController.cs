using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.MVC.Extensions;
using JP.DataHub.MVC.Filters;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Models.Trail;

namespace JP.DataHub.ManageApi.Controllers
{
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("55E5DC83-32FC-421D-9139-6706D424B319")]
    public class TrailController : AbstractController
    {
#if Oracle
        private Lazy<IStreamingServiceEventService> _lazyStreamingServiceService = new Lazy<IStreamingServiceEventService>(() => UnityCore.Resolve<IStreamingServiceEventService>("TrailOci"));
        private IStreamingServiceEventService _streamingService { get => _lazyStreamingServiceService.Value; }
#else
        private Lazy<IServiceBusEventService> _lazyServiceBusService = new Lazy<IServiceBusEventService>(() => UnityCore.Resolve<IServiceBusEventService>("Trail"));
        private IServiceBusEventService _serviceBusService { get => _lazyServiceBusService.Value; }
#endif

        [HttpPost]
        [ManageAction("19AF78DE-789E-4299-8714-855F3AC552AC")]
        public ActionResult Register(TrailRegisterViewModel model)
        {
#if Oracle
            _streamingService.SendAsync(model.ToJsonString()).Wait();
#else
            _serviceBusService.SendAsync(model.ToJsonString()).Wait();
#endif
            return Ok();
        }
    }
}
