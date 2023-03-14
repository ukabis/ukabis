using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Oracle;
using JP.DataHub.Com.Dapper;
using Oracle.ManagedDataAccess.Client;

namespace JP.DataHub.Data.Oracle
{
    internal class OracleDapperTypeMapping : IDapperTypeMapping
    {
        public void UseDapperTypeMapping()
        {
            // Oracleの文字列マッピングのミスを訂正するハック
            FixOdpNetDbTypeStringMapping();

            //OracleではCHAR(1)をboolとして扱う
            // OracleではDateTime型を明示的にDatetimeとしてマッピングしないとエラーが発生する
            // 何もしないとDatetime2としてマッピングされてしまうことがあるため
            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(new OracleBoolHandler());

            // GUIDクラスをVARCHAR2と対応付けるための設定
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.AddTypeHandler(typeof(Guid), new OracleGuidHandler());

            //OracleDynamicParameter指定だとOracleTypeMapperしか動かないため
            OracleTypeMapper.AddTypeHandler(typeof(DateTime), new OracleDatetimeHandler());
            OracleTypeMapper.AddTypeHandler(typeof(DateTime?), new OracleDatetimeHandler());
            OracleTypeMapper.AddTypeHandler(typeof(bool), new OracleBoolHandler());
        }

        private void FixOdpNetDbTypeStringMapping()
        {
            var asm = typeof(OracleConnection).Assembly;
            var dbTypeTable = asm.GetType("Oracle.ManagedDataAccess.Client.OraDb_DbTypeTable");
            if (dbTypeTable == null)
            {
                throw new NullReferenceException(nameof(dbTypeTable));
            }

            var dbTypeMapping = dbTypeTable.GetField("dbTypeToOracleDbTypeMapping", BindingFlags.Static | BindingFlags.NonPublic);
            if (dbTypeMapping == null)
            {
                throw new NullReferenceException(nameof(dbTypeMapping));
            }

            var typeMappings = (int[]?)dbTypeMapping.GetValue(null);
            if (typeMappings == null)
            {
                throw new NullReferenceException(nameof(typeMappings));
            }

            typeMappings[(int)System.Data.DbType.String] = (int)OracleDbType.NVarchar2;
            dbTypeMapping.SetValue(null, typeMappings);
        }
    }
}
