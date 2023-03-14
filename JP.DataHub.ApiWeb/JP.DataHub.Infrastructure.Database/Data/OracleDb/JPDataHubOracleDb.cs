using Newtonsoft.Json;
using JP.DataHub.Com.Log;

namespace JP.DataHub.Infrastructure.Database.Data.OracleDb
{
    /// <summary>
    /// OracleDbデータアクセス(DynamicAPIリポジトリ用)
    /// </summary>
    public class JPDataHubOracleDb : AbstractJPDataHubRdbms
    {
        private static readonly object _LockObj = new object();

        protected override JPDataHubLogger Log => new JPDataHubLogger(typeof(JPDataHubOracleDb));
        protected override string ProviderInvariantName => "Oracle.ManagedDataAccess.Client";
        protected override object LockObj => _LockObj;
        protected override IList<JsonConverter> JsonConverters { get; } = new List<JsonConverter>() { new DecimalConverter() };

        public JPDataHubOracleDb() : base()
        {
        }


        /// <summary>
        /// 数値型コンバータ
        /// </summary>
        /// <remarks>
        /// シリアライズ時に整数にも小数桁が付与される(XXX.0)のを防ぐため、整数値はlong、小数値はdecimalとして出力する。
        /// 小数桁末尾の0の復元は不可。
        /// </remarks>
        internal sealed class DecimalConverter : JsonConverter
        {
            public override bool CanRead => false;
            public override bool CanWrite => true;
            public override bool CanConvert(Type type) => type == typeof(decimal);

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var decimalValue = ((decimal)value);
                if (decimalValue % 1 == 0)
                {
                    writer.WriteValue(Decimal.ToInt64(decimalValue));
                }
                else
                {
                    writer.WriteValue(decimalValue);
                }
            }

            public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }
        }
    }
}
