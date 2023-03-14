using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class DataSchemaInformationModel
    {
        public string DataSchemaId { get; set; }

        public string SchemaName { get; set; }

        public string VendorId { get; set; }

        public string DataSchema { get; set; }

        public bool IsActive { get; set; }

        public string DataSchemaDescription { get; set; }
    }
}
