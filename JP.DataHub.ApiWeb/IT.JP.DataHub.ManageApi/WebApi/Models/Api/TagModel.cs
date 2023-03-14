using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Api
{
    public class TagModel
    {
        public string ApiId { get; set; }
        public string TagId { get; set; }
        public string ParentTagId { get; set; }
        public string TagTypeId { get; set; }
        public string TagTypeName { get; set; }
        public string TagName { get; set; }
        public string Code { get; set; }
        public string Code2 { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
