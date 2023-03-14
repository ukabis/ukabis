using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class DynamicApiTagViewModel
    {
        /// <summary>
        /// タグID
        /// </summary>
        public string TagId { get; set; }

        /// <summary>
        /// タグコード
        /// </summary>
        public string TagCode { get; set; }

        /// <summary>
        /// タグコード2
        /// </summary>
        public string TagCode2 { get; set; }

        /// <summary>
        /// タグ名
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 親のタグID
        /// </summary>
        public string ParentTagId { get; set; }

        /// <summary>
        /// タグタイプID
        /// </summary>
        public string TagTypeId { get; set; }

        /// <summary>
        /// タグタイプ名
        /// </summary>
        public string TagTypeName { get; set; }

        /// <summary>
        /// タグタイプ詳細
        /// </summary>
        public string TagTypeDetail { get; set; }

        /// <summary>
        /// タグ子要素
        /// </summary>
        public List<DynamicApiTagViewModel> Children { get; set; }

        public DynamicApiTagViewModel(string tagId, string tagCode, string tagCode2, string tagName,
                            string parentTagId, string tagTypeId, string tagTypeName, string tagTypeDetail, List<DynamicApiTagViewModel> children)
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
