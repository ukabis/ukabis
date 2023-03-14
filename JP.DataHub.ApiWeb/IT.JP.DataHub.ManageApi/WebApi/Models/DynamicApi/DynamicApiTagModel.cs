using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class DynamicApiTagModel
    {
        /// <summary>タグID</summary>
        public string TagId { get; set; }

        /// <summary>タグコード</summary>
        public string TagCode { get; set; }

        /// <summary>タグコード2</summary>
        public string TagCode2 { get; set; }

        /// <summary>タグ名</summary>
        public string TagName { get; set; }

        /// <summary>親のタグID</summary>
        public string ParentTagId { get; set; }

        /// <summary>タグタイプID</summary>
        public string TagTypeId { get; set; }

        public string TagTypeName { get; set; }

        public string TagTypeDetail { get; set; }

        public List<DynamicApiTagModel> Children { get; set; }

        public DynamicApiTagModel(string tagId, string tagCode, string tagCode2, string tagName,
                            string parentTagId, string tagTypeId, string tagTypeName, string tagTypeDetail, List<DynamicApiTagModel> children)
        {
            TagId = tagId;
            TagCode = tagCode;
            TagCode2 = tagCode2;
            TagName = tagName;
            ParentTagId = parentTagId;
            TagTypeId = tagTypeId;
            TagTypeName = tagTypeName;
            TagTypeDetail = tagTypeDetail;
            Children = children;
        }
    }
}
