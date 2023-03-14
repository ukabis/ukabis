using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http
{
    public class PagingRequest
    {
        /// <summary>
        /// 1ページに表示する件数
        /// </summary>
        public int DisplayCount { get; set; }

        /// <summary>
        /// 表示するページ番号（１オリジン）
        /// </summary>
        public int Page { get; set; }
    }
}
