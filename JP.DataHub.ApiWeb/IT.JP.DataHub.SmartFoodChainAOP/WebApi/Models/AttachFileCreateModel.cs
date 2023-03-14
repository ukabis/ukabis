using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class AttachFileCreateModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int FileLength { get; set; }
        public List<AttachFileCreateMetaModel> MetaList { get; set; }
    }

    public class AttachFileCreateMetaModel
    {
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }
    }

    public class ResponseFileIdModel
    {
        public string FileId { get; set; }
    }
}
