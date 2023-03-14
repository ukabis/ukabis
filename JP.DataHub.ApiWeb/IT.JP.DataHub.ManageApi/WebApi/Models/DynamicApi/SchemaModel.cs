using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class RegisterSchemaResponseModel
    {
        public string SchemaId { get; set; }
    }

    public class RegisterSchemaRequestModel
    {
        public string SchemaId { get; set; }
        public string SchemaName { get; set; }
        public string JsonSchema { get; set; }
        public string Description { get; set; }
        public string VendorId { get; set; }
    }

    public class SchemaModel
    {
        /// <summary>スキーマID</summary>
        public string SchemaId { get; set; }

        /// <summary>スキーマ名</summary>
        public string SchemaName { get; set; }

        /// <summary>JSONスキーマ</summary>
        public string JsonSchema { get; set; }

        /// <summary>説明</summary>
        public string Description { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }

    }
}
