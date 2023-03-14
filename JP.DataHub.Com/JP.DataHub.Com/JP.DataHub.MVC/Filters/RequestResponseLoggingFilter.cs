using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.DataContainer;

namespace JP.DataHub.MVC.Filters
{
    public class RequestResponseLoggingFilter : ActionFilterAttribute
    {
        private Lazy<IDataContainer> _lazyDataContainer = new Lazy<IDataContainer>(() => DataContainerUtil.ResolveDataContainer());
        private IDataContainer _dataContainer => _lazyDataContainer.Value;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor cad)
            {
                _dataContainer.ActionName = cad.ActionName;
                _dataContainer.ControllerName = cad.ControllerName;
                _dataContainer.Argument = cad.Parameters;
            }
        }
    }
}