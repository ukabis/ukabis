using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Configuration
{
    public class EventNotiryConfig
    {
        public const string SectionName = "EventNotify";
        public enum Types
        {
            //OracleRDBMS
            OracleDb,
            //OracleAdvanceQueing
            OracleAq
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public EventNotiryConfigOracleDBSetting OracleDbSettings { get; set; }
        public EventNotiryConfigOracleAqSetting OracleAqSettings { get; set; }

        public void Verify()
        {
            if (string.IsNullOrEmpty(Name)) throw new Exception($"{nameof(EventNotiryConfig)} Name is required.");
            if (string.IsNullOrEmpty(Type)) throw new Exception($"{nameof(EventNotiryConfig)} Type is required.");
            if (!Enum.IsDefined(typeof(Types), Type)) throw new Exception($"{nameof(EventNotiryConfig)} This type is not supported.{Type}");
            if(Type == Types.OracleDb.ToString())
            {
                if (OracleDbSettings == null) throw new Exception($"{nameof(EventNotiryConfig)} OracleDbSettings is required.");
                OracleDbSettings.Verify();
            }
            if (Type == Types.OracleAq.ToString())
            {
                if (OracleAqSettings == null) throw new Exception($"{nameof(EventNotiryConfig)} OracleAqSettings is required.");
                OracleAqSettings.Verify();
            }
        }


        public class EventNotiryConfigOracleDBSetting
        {
            public string ConnectionStringName { get; set; }
            public string TableName { get; set; }
            public void Verify()
            {
                if (string.IsNullOrEmpty(ConnectionStringName)) throw new Exception($"{nameof(EventNotiryConfigOracleDBSetting)} ConnectionStringName is required.");
                if (string.IsNullOrEmpty(TableName)) throw new Exception($"{nameof(EventNotiryConfigOracleDBSetting)} TableName is required.");
            }
        }

        public class EventNotiryConfigOracleAqSetting
        {
            public string ConnectionStringName { get; set; }
            public string QueueName { get; set; }
            public string SubscriberName { get; set; }
            public void Verify()
            {
                if (string.IsNullOrEmpty(ConnectionStringName)) throw new Exception($"{nameof(EventNotiryConfigOracleAqSetting)} ConnectionStringName is required.");
                if (string.IsNullOrEmpty(QueueName)) throw new Exception($"{nameof(EventNotiryConfigOracleAqSetting)} QueueName is required.");
                if (string.IsNullOrEmpty(SubscriberName)) throw new Exception($"{nameof(EventNotiryConfigOracleAqSetting)} SubscriberName is required.");
            }
        }
    }
}
