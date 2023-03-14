using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Exceptions
{
    /// <summary>
    /// データベースに対してSQL実行時にエラーが発生した場合にスローする例外です。
    /// </summary>
    [Serializable]
    public class SqlDatabaseException : Exception
    {
        public SqlDatabaseException()
        {
        }

        public SqlDatabaseException(string message)
            : base("SqlDataBase Error: " + message)
        {
        }

        public SqlDatabaseException(string message, Exception inner)
#if (DEBUG)
            : base(message, inner)
#else
            : base(message)
#endif
        {
        }
    }
}
