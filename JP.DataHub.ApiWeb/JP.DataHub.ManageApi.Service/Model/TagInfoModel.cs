using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class TagInfoModel
    {
        public string TagId { get; set; }

        public string TagCode1 { get; set; }

        public string TagCode2 { get; set; }

        public string TagName { get; set; }

        public string ParentTagId { get; set; }

        public List<TagInfoModel> Children { get; set; } = new();
    }
}
