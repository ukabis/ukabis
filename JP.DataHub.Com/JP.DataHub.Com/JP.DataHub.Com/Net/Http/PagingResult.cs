using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http
{
    public class PagingResult<T>
    {
        /// <summary>
        /// 表示するページ番号（１オリジン）
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 1ページに表示する件数
        /// </summary>
        public int DisplayCount { get; set; }
        /// <summary>
        /// レコード数
        /// </summary>
        public int RecordCount { get; set; }
        /// <summary>
        /// 最大ページ数（1オリジン）
        /// </summary>
        public int MaxPageCount { get; set; }
        /// <summary>
        /// 結果
        /// </summary>
        public T Result { get;set;}
    }
}
