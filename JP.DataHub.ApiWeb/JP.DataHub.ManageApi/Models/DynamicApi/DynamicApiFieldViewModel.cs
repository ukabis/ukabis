using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class DynamicApiFieldViewModel
    {
        /// <summary>分野ID</summary>
        public string FieldId { get; }

        /// <summary>親の分野ID</summary>
        public string ParentFieldId { get; }

        /// <summary>分野名</summary>
        public string FieldName { get; }

        public List<DynamicApiFieldViewModel> Children { get; }

        public DynamicApiFieldViewModel(string fieldId, string fieldName, string parentFieldId, List<DynamicApiFieldViewModel> children)
        {
            FieldId = fieldId;
            FieldName = fieldName;
            ParentFieldId = parentFieldId;
            Children = children;
        }
    }
}
