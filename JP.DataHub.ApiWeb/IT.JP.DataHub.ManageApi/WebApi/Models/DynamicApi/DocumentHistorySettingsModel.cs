using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class DocumentHistorySettingsModel
    {
        public bool IsEnable { get; set; } = false;

        public string HistoryRepositoryId { get; set; }

    }
}
