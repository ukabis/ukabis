using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Serializer
{
    public class BinaryFormatterSerializer: ISerializer
    {
        private BinaryFormatter _formatter = new BinaryFormatter();

        public Stream Serialize(object targetObject)
        {
            using var serialized = new MemoryStream();
            _formatter.Serialize(serialized, targetObject);
            return serialized;
        }

        public byte[] SerializeByte(object targetObject)
        {
            using var serialized = new MemoryStream();
            _formatter.Serialize(serialized,targetObject);
            return serialized.ToArray();
        }

        public T Deserialize<T>(object targetObject)
        {
            if (targetObject is byte[])
            {
                return Deserialize<T>(targetObject as byte[]);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _formatter.Serialize(ms, targetObject);
                    using (var ms2 = new MemoryStream(ms.ToArray()))
                    {
                        var tmp = _formatter.Deserialize(ms2);
                        return (T)tmp;
                    }
                }
            }
        }

        public T Deserialize<T>(byte[] targetObject)
        {
            using (var ms = new MemoryStream(targetObject))
            {
                return (T)_formatter.Deserialize(ms);
            }
        }

        public object Deserialize(Type type, byte[] obj)
        {
            using (var ms = new MemoryStream(obj))
            {
                return _formatter.Deserialize(ms);
            }
        }
    }
}
