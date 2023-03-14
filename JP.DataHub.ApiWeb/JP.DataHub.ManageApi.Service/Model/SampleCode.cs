using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class SampleCode
    {
        public Guid MethodId { get; set; }
        public Guid SampleCodeId { get; set; }
        public string Language { get; set; }
        public int DisplayOrder { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
