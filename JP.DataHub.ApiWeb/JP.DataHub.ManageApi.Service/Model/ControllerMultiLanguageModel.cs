using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ControllerMultiLanguageModel
    {
        public string ControllerLangId { get; set; }

        public string LocaleCode { get; set; }

        public string ControllerDescription { get; set; }

        public string ControllerName { get; set; }

        public string FeeDescription { get; set; }

        public string ResourceCreateUser { get; set; }

        public string ResourceMaintainer { get; set; }

        public string UpdateFrequency { get; set; }

        public string ContactInformation { get; set; }

        public string Version { get; set; }

        public string AgreeDescription { get; set; }

        public bool IsActive { get; set; }
    }
}
