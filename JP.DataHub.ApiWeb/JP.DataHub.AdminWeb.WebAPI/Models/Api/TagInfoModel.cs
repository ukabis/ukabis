using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class TagInfoModel
    {
        public string TagId { get; set; }

        public string TagCode1 { get; set; }

        public string TagCode2 { get; set; }

        public string TagName { get; set; }

        public string ParentTagId { get; set; }

        public List<TagInfoModel> Children { get; set; }

        public bool IsActive { get; set; } = false;

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public TagInfoModel Clone()
        {
            var clone = (TagInfoModel)MemberwiseClone();
            clone.Children = Children?.Select(x => x.Clone()).ToList();
            return clone;
        }
    }
}
