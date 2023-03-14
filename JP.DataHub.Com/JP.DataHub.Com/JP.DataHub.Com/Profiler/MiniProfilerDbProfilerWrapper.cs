using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.DataContainer;

namespace JP.DataHub.Com.Profiler
{
    public class MiniProfilerDbProfilerWrapper : IDbProfiler
    {
        private readonly IDbProfiler Profiler = (IDbProfiler)MiniProfiler.Current;


        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive => Profiler.IsActive && !IsDisabled();


        /// <summary>
        /// SQLExecuteFinish
        /// </summary>
        public void ExecuteFinish(IDbCommand profiledDbCommand, SqlExecuteType executeType, DbDataReader reader)
        {
            Profiler.ExecuteFinish(profiledDbCommand, executeType, reader);
        }

        /// <summary>
        /// SQL Execute Start
        /// </summary>
        public void ExecuteStart(IDbCommand profiledDbCommand, SqlExecuteType executeType)
        {
            Profiler.ExecuteStart(profiledDbCommand, executeType);
        }

        /// <summary>
        /// OnError
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        /// <param name="exception"></param>
        public void OnError(IDbCommand profiledDbCommand, SqlExecuteType executeType, Exception exception)
        {
            Profiler.OnError(profiledDbCommand, executeType, exception);
        }

        /// <summary>
        /// SQLREADERFINISH
        /// </summary>
        /// <param name="reader"></param>
        public void ReaderFinish(IDataReader reader)
        {
            Profiler.ReaderFinish(reader);
        }


        private bool IsDisabled()
        {
            IDataContainer dataContainer;
            try
            {
                dataContainer = DataContainerUtil.ResolveDataContainer();
                //dataContainer = UnityCore.Resolve<IDataContainer>();
                return dataContainer.ProfilerDisabled;
            }
            catch
            {
                // 何もしない
            }

            return false;
        }
    }
}
