using Dapper;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    public class StringToGuidHandler : SqlMapper.TypeHandler<string>
    {
        public override void SetValue(IDbDataParameter parameter, string value)
        {
            parameter.DbType = DbType.Guid;
            parameter.Value = Guid.Parse(value);
        }

        public override string Parse(object value)
        {
            return value?.ToString();
        }
    }
}
