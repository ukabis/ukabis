using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Converter;

namespace JP.DataHub.Com.Extensions
{
	public static class GuidExtension
	{
		public static string ToBase32String(this Guid guid, bool withPadding = false)
		{
			var result = Base32Converter.ToBase32String(guid.ToByteArray(), withPadding);
			return result;
		}

		public static Guid PaseToBase32String(this Guid guid, string str)
		{
			str = str.ToUpper();
			if (str.Length < 28)
			{
				StringBuilder sb = new StringBuilder();
				var padding = 28 - str.Length;
				sb.Append(str);
				for (int i = 0; i < padding; i++)
				{
					sb.Append("=");
				}
				str = sb.ToString();
			}

			var decoded = Base32Converter.FromBase32String(str);
			byte[] guidarray = new byte[16];
			for (int i = 0; i < 16; i++)
			{
				guidarray[i] = decoded[i];
			}
			return new Guid(guidarray);
		}
	}
}
