using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using JP.DataHub.Com.Dapper;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;

namespace Dapper
{
    public static class DapperApplicationBuilder
    {
        public static void UseDapperTypeMapping(string dbType = null)
        {
            SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime);
            SqlMapper.AddTypeMap(typeof(DateTime?), DbType.DateTime);

            // DB型の型=GUID(identifier）と、C#の型string間で変換するように定義
            SqlMapper.AddTypeHandler(new StringToGuidHandler());

            SimpleCRUD.SetDialect(dbType.ToEnum<SimpleCRUD.Dialect>());

            // DB依存部分のTypeMappingを行う
            UnityCore.ResolveOrDefault<IDapperTypeMapping>(dbType)?.UseDapperTypeMapping();
        }
        
        public static void UseDapperTypeMapping(this IApplicationBuilder app, string dbType = null)
        {
            UseDapperTypeMapping(dbType);
        }
    }
}
