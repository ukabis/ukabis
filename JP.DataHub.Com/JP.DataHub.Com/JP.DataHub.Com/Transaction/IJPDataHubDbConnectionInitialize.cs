using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Configuration;

namespace JP.DataHub.Com.Transaction
{
    public interface IJPDataHubDbConnectionInitialize
    {
        void Init(IJPDataHubDbConnection connection, JPDataHubDbConnectionParam param, DbConnection dbConnection, IDbProviderFactoriesConfig config);
    }
}
