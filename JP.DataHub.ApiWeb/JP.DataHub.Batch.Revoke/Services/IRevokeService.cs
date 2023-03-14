using JP.DataHub.Batch.Revoke.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.Revoke.Services
{
    public interface IRevokeService
    {
        void Revoke(RevokeNotifyModel model);
    }
}
