using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Serializer
{
    public interface ISerializer
    {
        Stream Serialize(object obj);

        byte[] SerializeByte(object obj);

        T Deserialize<T>(byte[] obj);

        object Deserialize(Type type, byte[] obj);
    }
}
