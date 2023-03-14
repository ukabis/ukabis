using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Transaction
{
    public interface IJPDataHubTransactionManager : IList<IWantTransaction>
    {
        void Connected(IWantTransaction conn);
    }
}
