using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Web.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.MVC.Extensions;
using JP.DataHub.MVC.Filters;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Models.Test;
using JP.DataHub.ManageApi.Models.ApiDescription;
using JP.DataHub.ManageApi.Core.DataContainer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using JP.DataHub.ManageApi.Attributes;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace JP.DataHub.ManageApi.Models.ApiDescription
{
    public class ApiDescriptionWrapper
    {
        public ActionDescriptor ActionDescriptor { get => apiDescription.ActionDescriptor; }
        public IList<ApiParameterDescription> ParameterDescriptions { get => apiDescription.ParameterDescriptions; }
        public IDictionary<object, object> Properties { get => apiDescription.Properties; }
        public IList<ApiRequestFormat> SupportedRequestFormats { get => apiDescription.SupportedRequestFormats; }
        public IList<ApiResponseType> SupportedResponseTypes { get => apiDescription.SupportedResponseTypes; }
        public string? GroupName { get => apiDescription.GroupName; }
        public string? HttpMethod { get => apiDescription.HttpMethod; }
        public string? RelativePath { get; }

        private Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription apiDescription;


        public ApiDescriptionWrapper(Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription description)
        {
            apiDescription = description;

            RelativePath = description.RelativePath;

            //RelativePathが、.NETFramworkと違い hoge?fuga={fuga}としてくれず、hoge しか入って来ないため、?fuga={fuga}を付与する
            if (ParameterDescriptions != null && ParameterDescriptions.Count != 0)
            {
                var parametas = ParameterDescriptions.Where(x => x.Source == BindingSource.Query)?.Select(x => $"{x.Name}={{{x.Name}}}").ToList();
                if (parametas != null && parametas.Count != 0)
                    RelativePath = $"{RelativePath}?{string.Join('&', parametas)}";
            }
        }     
    }
}
