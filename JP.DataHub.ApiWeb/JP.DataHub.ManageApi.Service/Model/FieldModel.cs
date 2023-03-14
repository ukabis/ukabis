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
    public class FieldModel
    {
        public Guid ApiId { get; set; }
        public Guid FieldId { get; set; }
        public Guid ParentFieldId { get; set; }
        public string FieldName { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
