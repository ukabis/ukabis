using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ApiMultiLanguageViewModel
    {
        public string ApiLangId { get; set; }

        public string LocaleCode { get; set; }

        public string ApiDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
