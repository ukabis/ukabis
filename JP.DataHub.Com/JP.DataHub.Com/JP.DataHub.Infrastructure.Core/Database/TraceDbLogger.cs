using System;
using System.Data;
using System.Data.Common;
using System.Text;
using StackExchange.Profiling.Data;
using System.Diagnostics;
using JP.DataHub.Com.Log;

namespace JP.DataHub.Infrastructure.Core.Database
{
    /// <summary>
    /// TraceDbLogger
    /// </summary>
    public class TraceDbLogger : IDbProfiler
    {
        private Stopwatch stopwatch;
        private string commandText;

        private JPDataHubLogger logger = new JPDataHubLogger(typeof(TraceDbLogger));
        private StringBuilder parameters = new StringBuilder();

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
            commandText = profiledDbCommand.CommandText.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
            parameters = new StringBuilder();
            for (int ix = 0; ix < profiledDbCommand.Parameters.Count; ix++)
            {
                var parameter = (DbParameter)profiledDbCommand.Parameters[ix];
                parameters.Append($" [ParameterName={parameter.ParameterName} ,Value={parameter.Value.ToString()}]");
            }
            if (executeType != SqlExecuteType.Reader)
            {
                stopwatch.Stop();
                logger.Debug($"SQL = {commandText} {parameters.ToString()} ExecuteTime={stopwatch.Elapsed.ToString()}");
            }


        }

        /// <summary>
        /// SQLExecuteStart Start StopWatch
        /// </summary>
        /// <param name="profiledDbCommand"></param>
        /// <param name="executeType"></param>
        public void ExecuteStart(IDbCommand profiledDbCommand, SqlExecuteType executeType)
        {
            stopwatch = Stopwatch.StartNew();
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
            stopwatch.Stop();
            logger.Debug($"SQL = {commandText} {parameters.ToString()} ExecuteTime={stopwatch.Elapsed.ToString()}");
        }
    }
    
}
