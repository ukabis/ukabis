using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Serializer
{
    public class JsonSerializer : ISerializer
    {
        public Stream Serialize(object obj)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(obj?.ToJsonString()));
        }

        public byte[] SerializeByte(object obj)
        {
            return Encoding.UTF8.GetBytes(obj?.ToJsonString());
        }

        public T Deserialize<T>(byte[] obj)
        {
            return Encoding.UTF8.GetString(obj).ToObject<T>();
        }

        public object Deserialize(Type type, byte[] obj)
        {
            return Encoding.UTF8.GetString(obj).ToObject(type);
        }
    }
}
