using System;
using Microsoft.Extensions.Configuration;
using Unity;
using Dapper;
using JP.DataHub.Com.Dapper;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;

namespace JP.DataHub.Data.Oracle
{
    [UnityBuildup]
    public class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            container.RegisterType<IDapperTypeMapping, OracleDapperTypeMapping>("Oracle");
            container.RegisterType<IJPDataHubDbConnectionInitialize, OracleDbConnectionInitialize>("Oracle");
        }
    }
}
