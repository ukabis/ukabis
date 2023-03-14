using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using StackExchange.Profiling.Data;
using JP.DataHub.Com.Log;

namespace JP.DataHub.Com.Profiler
{
    public class TraceDbLogger : IDbProfiler
    {
        private Stopwatch _stopwatch;
        private string _commandText;

        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(TraceDbLogger));
        private StringBuilder _parameters = new StringBuilder();

        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// SQLExeCute Finish output Log Exclude Reader
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        /// <param name="reader"></param>
        public void ExecuteFinish(IDbCommand profiledDbCommand, SqlExecuteType executeType, DbDataReader reader)
        {
            _commandText = profiledDbCommand.CommandText.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
            _parameters = new StringBuilder();
            for (int ix = 0; ix < profiledDbCommand.Parameters.Count; ix++)
            {
                var parameter = (DbParameter)profiledDbCommand.Parameters[ix];
                _parameters.Append($" [ParameterName={parameter.ParameterName} ,Value={parameter.Value.ToString()}]");
            }
            if (executeType != SqlExecuteType.Reader)
            {
                _stopwatch.Stop();
                _logger.Debug($"SQL = {_commandText} {_parameters.ToString()} ExecuteTime={_stopwatch.Elapsed.ToString()}");
            }
        }

        /// <summary>
        /// SQLExecuteStart Start StopWatch
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        public void ExecuteStart(IDbCommand profiledDbCommand, SqlExecuteType executeType)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// OnError
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        /// <param name="exception"></param>
        public void OnError(IDbCommand profiledDbCommand, SqlExecuteType executeType, Exception exception)
        {
        }

        /// <summary>
        /// SQL reader Finish Out Reader log
        /// </summary>
        /// <param name="reader"></param>
        public void ReaderFinish(IDataReader reader)
        {
            _stopwatch.Stop();
            _logger.Debug($"SQL = {_commandText} {_parameters.ToString()} ExecuteTime={_stopwatch.Elapsed.ToString()}");
        }
    }
}
