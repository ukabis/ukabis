using System.Collections.Generic;

namespace JP.DataHub.Com.Net.Http.Models
{
    public class ExternalAttachFileModel
    {
        public string DataSourceType { get; set; }
        public List<string> Credentials { get; set; }
        public string FilePath { get; set; }
    }
}
