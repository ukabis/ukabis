using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Transaction
{
    public class JPDataHubTransactionManager : List<IWantTransaction>, IJPDataHubTransactionManager
    {
        private object obj = new object();

        public void Connected(IWantTransaction tran)
        {
            lock (obj)
            {
                if (tran?.IsTransactionManage == true)
                {
                    this.Add(tran);
                }
            }
        }

        public void DoCommit()
        {
        }

        public void DoRollback()
        {
        }
    }
}
