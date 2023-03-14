using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class SystemLinkModel : LinkBase
    {
        public string SystemLinkId { get; set; }
        public string SystemId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Url { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDefault { get; set; }
    }
}
