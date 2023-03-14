using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class FieldQueryViewModel
    {
        public string FieldId { get; set; }

        public string ParentFieldId { get; set; }

        public string FieldName { get; set; }

        public List<FieldQueryViewModel> Children { get; set; } = new();

    }
}
