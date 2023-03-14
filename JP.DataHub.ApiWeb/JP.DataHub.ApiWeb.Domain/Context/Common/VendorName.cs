using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
	[MessagePackObject]
	public record VendorName : IValueObject
	{
		[Key(0)]
		public string Value { get; }

		public VendorName(string venderName)
		{
			this.Value = venderName;
		}

		public static bool operator ==(VendorName me, object other) => me?.Equals(other) == true;

		public static bool operator !=(VendorName me, object other) => !me?.Equals(other) == true;
	}

	internal static class VendorNameExtension
	{
		public static VendorName ToVendorName(this string val) => val == null ? null : new VendorName(val);
	}
}
