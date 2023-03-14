using MessagePack;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    public record DataSchema : IValueObject
    {
        [Key(0)]
        public string Value { get; }

        [IgnoreMember]
        private Lazy<JSchema> _lazySchema;
        [IgnoreMember]
        private JSchemaReaderSettings s_jsonSchemaSettings = new JSchemaReaderSettings { Validators = new List<IJsonValidator> { new NumberCustomValidator(), new JsonFormatCustomValidator() } };

        public DataSchema(string value)
        {
            this.Value = value;
            _lazySchema = new Lazy<JSchema>(() =>
            {
                if (string.IsNullOrEmpty(Value))
                {
                    return null;
                }
                else
                {
                    return Value == null ? null : JSchema.Parse(Value, s_jsonSchemaSettings);
                }
            });
        }

        public JSchema ToJSchema() => _lazySchema.Value;

        public static bool operator ==(DataSchema me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DataSchema me, object other) => !me?.Equals(other) == true;
    }

    internal static class DataSchemaExtension
    {
        public static DataSchema ToDataSchema(this string str) => str == null ? null : new DataSchema(str);
    }
}
