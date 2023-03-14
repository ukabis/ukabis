using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;

namespace JP.DataHub.Data.Oracle
{
    public class OracleDbConnectionInitialize : IJPDataHubDbConnectionInitialize
    {
        private static bool _isSetFetchSize = false;

        public void Init(IJPDataHubDbConnection connection, JPDataHubDbConnectionParam param, DbConnection dbConnection, IDbProviderFactoriesConfig config)
        {
            SetFetchSize(config.Init.GetValueOrDefault("FetchSize").To<int>());
        }

        private void SetFetchSize(int fetchSize)
        {
            if (_isSetFetchSize == false)
            {
                if (fetchSize != 0)
                {
                    OracleConfiguration.FetchSize = fetchSize;
                }
                _isSetFetchSize = true;
            }
        }
    }
}
