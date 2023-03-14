using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiResourceFieldQueryModel
    {
        public string FieldId { get; set; }

        public string ParentFieldId { get; set; }

        public string FieldName { get; set; }

        public bool IsActive { get; set; } = false;

        public List<ApiResourceFieldQueryModel> Children { get; set; } = new();

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public ApiResourceFieldQueryModel Clone()
        {
            var clone = (ApiResourceFieldQueryModel)MemberwiseClone();
            clone.Children = Children?.Select(x => x.Clone()).ToList();
            return clone;
        }
    }
}
