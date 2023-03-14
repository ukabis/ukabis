using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Formatters;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    [MessagePackFormatter(typeof(IpAddressForamatter))]
    internal record IpAddress : IValueObject
    {
        [Key(0)]
        public string Value { get; }

        public IpAddress(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(IpAddress me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IpAddress me, object other) => !me?.Equals(other) == true;
    }

    internal static class IpAddressExtension
    {
        public static IpAddress ToIpAddress(this string? val) => val == null ? null : new IpAddress(val);
        public static IpAddress ToIpAddress(this Guid? val) => val == null ? null : new IpAddress(val.ToString());
    }

    internal class IpAddressForamatter : IMessagePackFormatter<IpAddress>
    {
        public IpAddress Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            var ipAddress = reader.ReadString();
            if (ipAddress == "null")
            {
                return new IpAddress(null);
            }
            return new IpAddress(ipAddress);
        }

        public void Serialize(ref MessagePackWriter writer, IpAddress value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteString(System.Text.Encoding.UTF8.GetBytes(value.Value?.ToString() ?? "null"));
            return;
        }
    }
}