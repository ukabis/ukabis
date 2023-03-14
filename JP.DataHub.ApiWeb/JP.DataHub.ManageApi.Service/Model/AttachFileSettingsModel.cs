using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class AttachFileSettingsModel
    {
        public bool IsEnable { get; set; }

        public string MetaRepositoryId { get; set; }

        public string BlobRepositoryId { get; set; }
    }
}
