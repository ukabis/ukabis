using System;
using System.IO;
using MessagePack;

namespace JP.DataHub.Com.Serializer
{
    public class ComMessagePackSerializer : ISerializer
    {
        private MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard.WithResolver(MessagePack.Resolvers.StandardResolverAllowPrivate.Instance);

        public Stream Serialize(object obj)
        {
            return new MemoryStream(MessagePackSerializer.Serialize(obj, _options));
        }

        public byte[] SerializeByte(object obj)
        {
            return MessagePackSerializer.Serialize(obj, _options);
        }

        public T Deserialize<T>(byte[] obj)
        {
            return MessagePackSerializer.Deserialize<T>(obj, _options);
        }

        public object Deserialize(Type type, byte[] obj)
        {
            return MessagePackSerializer.Deserialize(type, obj, _options);
        }
    }
}
