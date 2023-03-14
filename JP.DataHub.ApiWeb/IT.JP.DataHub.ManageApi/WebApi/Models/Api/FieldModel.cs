using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Api
{
    public class FieldModel
    {
        public string ApiId { get; set; }
        public string FieldId { get; set; }
        public string ParentFieldId { get; set; }
        public string FieldName { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
