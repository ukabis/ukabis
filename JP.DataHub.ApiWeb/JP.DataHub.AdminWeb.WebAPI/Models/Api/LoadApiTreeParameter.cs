using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    /// <summary>
    /// APIツリー更新時の引数用クラス
    /// 使いたいパラメタが増えたら随時追加する
    /// </summary>
    public class LoadApiTreeParameter
    {
        public string? ResourceId { get; set; } = null;

        public bool? IsResourceRemoved { get; set; } = false;
    }
}
