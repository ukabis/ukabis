using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiResourceAttachFileSettingsModel
    {
        public bool IsEnable { get; set; }

        [RequiredMetaWhenAttachFileEnable]
        public string MetaRepositoryId { get; set; }

        [RequiredBlobWhenAttachFileEnable]
        public string BlobRepositoryId { get; set; }

        public ApiResourceAttachFileSettingsModel(bool isEnable, string metaRepositoryId, string blobRepositoryId)
        {
            IsEnable = isEnable;
            MetaRepositoryId = metaRepositoryId?.ToLower();
            BlobRepositoryId = blobRepositoryId?.ToLower();
        }
    }
}
