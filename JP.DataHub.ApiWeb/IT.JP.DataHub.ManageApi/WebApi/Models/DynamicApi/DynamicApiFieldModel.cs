using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class DynamicApiFieldModel
    {
        /// <summary>分野ID</summary>
        public string FieldId { get; }

        /// <summary>親の分野ID</summary>
        public string ParentFieldId { get; }

        /// <summary>分野名</summary>
        public string FieldName { get; }

        public List<DynamicApiFieldModel> Children { get; }

        public DynamicApiFieldModel(string fieldId, string fieldName, string parentFieldId, List<DynamicApiFieldModel> children)
        {
            FieldId = fieldId;
            FieldName = fieldName;
            ParentFieldId = parentFieldId;
            Children = children;
        }
    }
}
