using JP.DataHub.Com.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi
{
    public class DataSchema
    {
        public string Value { get; }

        private Lazy<JSchema> _schema;

        private JSchemaReaderSettings jsonSchemaSettings = new JSchemaReaderSettings { Validators = new List<IJsonValidator> { new JsonFormatCustomValidator(), new NumberCustomValidator() } };

        public DataSchema(string value)
        {
            this.Value = value;
            _schema = new Lazy<JSchema>(() =>
            {
                if (string.IsNullOrEmpty(Value))
                {
                    return null;
                }
                else
                {
                    return Value == null ? null : JSchema.Parse(Value, jsonSchemaSettings);
                }
            });
        }

        public JSchema GetJSchema() => _schema.Value;
        public JSchema ToJSchema() => _schema.Value;
    }
}
