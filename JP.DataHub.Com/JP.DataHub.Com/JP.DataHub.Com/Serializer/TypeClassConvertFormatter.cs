using System;
using System.Text;
using MessagePack;
using MessagePack.Formatters;

namespace JP.DataHub.Com.Serializer
{
    public class TypeClassConvertFormatter : IMessagePackFormatter<Type>
    {
        public Type Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            var assemblyQualifiedName = reader.ReadString();
            return Type.GetType(assemblyQualifiedName);
        }

        public void Serialize(ref MessagePackWriter writer, Type value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteString(Encoding.UTF8.GetBytes(value.AssemblyQualifiedName));
            return;
        }
    }
}
