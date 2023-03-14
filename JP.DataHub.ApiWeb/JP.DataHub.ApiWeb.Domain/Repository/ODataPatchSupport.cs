using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    public enum ODataPatchSupport
    {
        // サポートしない
        None,
        // 一括更新(SQL)
        BulkUpdate,
        // 順次更新
        SequentialUpdate,
    }
}
