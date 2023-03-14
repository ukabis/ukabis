using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// リポジトリグループタイプ
    /// </summary>
    public class RepositoryTypeModel
    {
        /// <summary>
        /// リポジトリーグループタイプコード
        /// </summary>
        public string RepositoryTypeCd { get; set; }
        /// <summary>
        /// リポジトリーグループタイプ名
        /// </summary>
        public string RepositoryTypeName { get; set; }
    }
}
