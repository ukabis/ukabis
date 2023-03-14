using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Profiling.Data;

namespace JP.DataHub.Com.Profiler
{
    public class CompositeDbProfiler : IDbProfiler
    {
        readonly IDbProfiler[] profilers;

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="dbProfilers"></param>
        public CompositeDbProfiler(params IDbProfiler[] dbProfilers)
        {
            this.profilers = dbProfilers;
        }

        /// <summary>
        /// SQLExecuteFinish
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        /// <param name="reader"></param>
        public void ExecuteFinish(IDbCommand profiledDbCommand, SqlExecuteType executeType, DbDataReader reader)
        {
            foreach (var item in profilers)
            {
                if (item != null && item.IsActive)
                {
                    item.ExecuteFinish(profiledDbCommand, executeType, reader);
                }
            }
        }

        /// <summary>
        /// SQL Execute Start
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        public void ExecuteStart(IDbCommand profiledDbCommand, SqlExecuteType executeType)
        {
            foreach (var item in profilers)
            {
                if (item != null && item.IsActive)
                {
                    item.ExecuteStart(profiledDbCommand, executeType);
                }
            }
        }

        /// <summary>
        /// Isactive
        /// </summary>
        public bool IsActive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// OnError
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        /// <param name="exception"></param>
        public void OnError(IDbCommand profiledDbCommand, SqlExecuteType executeType, Exception exception)
        {
            foreach (var item in profilers)
            {
                if (item != null && item.IsActive)
                {
                    item.OnError(profiledDbCommand, executeType, exception);
                }
            }
        }

        /// <summary>
        /// SQLREADERFINISH
        /// </summary>
        /// <param name="reader"></param>
        public void ReaderFinish(IDataReader reader)
        {
            foreach (var item in profilers)
            {
                if (item != null && item.IsActive)
                {
                    item.ReaderFinish(reader);
                }
            }
        }
    }
}
