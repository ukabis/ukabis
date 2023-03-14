using System;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Logging
{
    public class ApiInfoModel
    {
        public string ControllerId { get; set; }

        public string ApiId { get; set; }

        public string ApiDescription { get; set; }

        public string ApiUrl { get; set; }

        public ApiInfoModel(string controllerId, string apiId, string apiDescription, string apiUrl)
        {
            ControllerId = controllerId;
            ApiId = apiId;
            ApiDescription = apiDescription;
            ApiUrl = apiUrl;
        }
    }
}
