using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class FieldQueryModel
    {
        public FieldQueryModel()
        {
            Children = new List<FieldQueryModel>();
        }

        public string FieldId { get; }

        public string ParentFieldId { get; }

        public string FieldName { get; }

        public IList<FieldQueryModel> Children { get; internal set; }

    }
}
