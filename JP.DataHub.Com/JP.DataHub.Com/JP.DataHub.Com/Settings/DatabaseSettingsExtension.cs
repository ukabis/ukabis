using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace JP.DataHub.Com.Settings
{
    public static class DatabaseSettingsExtension
    {
        public static TwowaySqlParser.DatabaseType GetDbType(this DatabaseSettings dbSettings)
        {
            return dbSettings.Type switch
            {
                "SQLServer" => TwowaySqlParser.DatabaseType.SqlServer,
                "Oracle" => TwowaySqlParser.DatabaseType.Oracle,
                _ => throw new InvalidCastException($"{dbSettings.Type} を {nameof(TwowaySqlParser.DatabaseType)} に変換できません"),
            };
        }

        public static　IDynamicParameters GetParameters(this DatabaseSettings dbSettings)
        {
            return UnityCore.Resolve<IDynamicParameters>(dbSettings.Type);
        }
    }
}
