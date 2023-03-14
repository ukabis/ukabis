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
    /// 物理リポジトリ
    /// </summary>
    public class PhysicalRepositoryModel
    {
        /// <summary>
        /// 物理リポジトリID
        /// </summary>
        public string PhysicalRepositoryId { get; set; }
        /// <summary>
        /// 接続文字列
        /// </summary>
        [Required(ErrorMessage = "接続文字列は必須項目です。")]
        [MaxLengthEx(MaxLength = 1024, ErrorMessageFormat = "接続文字列は{0}文字以内で入力して下さい。")]
        public string ConnectionString { get; set; }
        /// <summary>
        /// Fullかどうか
        /// </summary>
        public bool IsFull { get; set; }
        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public PhysicalRepositoryModel Clone()
        {
            return (PhysicalRepositoryModel)MemberwiseClone();
        }
    }
}
