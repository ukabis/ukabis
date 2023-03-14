using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// リポジトリグループ
    /// </summary>
    public class RepositoryGroupModel
    {
        /// <summary>
        /// リポジトリグループID
        /// </summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>
        /// リポジトリグループ名
        /// </summary>
        [Required(ErrorMessage = "リポジトリグループ名は必須項目です。")]
        [MaxLengthEx(MaxLength = 256, ErrorMessageFormat = "リポジトリグループ名は{0}文字以内で入力して下さい。")]
        public string RepositoryGroupName { get; set; }

        /// <summary>
        /// リポジトリグループタイプ
        /// </summary>
        [Required(ErrorMessage = "リポジトリグループタイプは必須項目です。")]
        public string RepositoryTypeCd { get; set; }

        /// <summary>
        /// 表示順
        /// </summary>
        public int SortNo { get; set; }

        /// <summary>
        /// デフォルトかどうか
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 物理リポジトリリスト
        /// </summary>
        [ValidateComplexType]
        public List<PhysicalRepositoryModel> PhysicalRepositoryList { get; set; }
    }
}
