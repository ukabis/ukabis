using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Database
{
    // .NET6
    /// <summary>
    /// GetAllApi用
    /// </summary>
    [MessagePackObject]
    public class AllApiEntityIdentifier : IAllApiEntityIdentifier
    {
        // Controller単位でAllAPIEntityを取得するために必要
        [Key(0)]
        public Guid controller_id { get; set; }

        // 以下はAPIの検索に必要
        [Key(1)]
        public string controller_relative_url { get; set; }

        [Key(2)]
        public Guid api_id { get; set; }
        [Key(3)]
        public string method_name { get; set; }
        [Key(4)]
        public string method_type { get; set; }
        [Key(5)]
        public string action_type_cd { get; set; }

        [Key(6)]
        public string alias_method_name { get; set; }
        [Key(7)]
        public bool is_nomatch_querystring { get; set; }
    }
}
