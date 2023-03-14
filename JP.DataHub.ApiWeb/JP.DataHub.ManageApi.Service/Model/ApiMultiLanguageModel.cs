using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ApiMultiLanguageModel
    {
        public string ApiLangId { get; set; } = Guid.NewGuid().ToString();

        public string LocaleCode { get; set; }

        public string ApiDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
