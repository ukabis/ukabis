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
    public class TagModel
    {
        public Guid ApiId { get; set; }
        public Guid TagId { get; set; }
        public Guid ParentTagId { get; set; }
        public Guid TagTypeId { get; set; }
        public string TagTypeName { get; set; }
        public string TagName { get; set; }
        public string Code { get; set; }
        public string Code2 { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
