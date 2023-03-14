using System;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class RegisterSampleCodeModel
    {
        /// <summary>
        /// SampleCodeId
        /// </summary>
        public string SampleCodeId { get; set; }

        /// <summary>
        /// LanguageId
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
