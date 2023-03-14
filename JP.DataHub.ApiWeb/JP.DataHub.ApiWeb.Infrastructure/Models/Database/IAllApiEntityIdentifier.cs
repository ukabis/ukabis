using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Database
{
    // .NET6
    /// <summary>
    /// GetAllApi用
    /// </summary>
    public interface IAllApiEntityIdentifier
    {
        // Controller単位でAllAPIEntityを取得するために必要
        Guid controller_id { get; set; }

        // 以下はAPIの検索に必要
        string controller_relative_url { get; set; }

        Guid api_id { get; set; }
        string method_name { get; set; }
        string action_type_cd { get; set; }
        string method_type { get; set; }
        string alias_method_name { get; set; }
        bool is_nomatch_querystring { get; set; }
    }
}
