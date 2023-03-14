using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class DynamicApiSampleCodeModel
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
        /// Language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
    }
}
