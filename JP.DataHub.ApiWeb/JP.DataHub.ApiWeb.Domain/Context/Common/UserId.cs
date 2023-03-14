using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
	internal record UserId : IValueObject
	{
		public string Value { get; }

		public UserId(string userId)
		{
			this.Value = userId;
		}

		public static bool operator ==(UserId me, object other) => me?.Equals(other) == true;

		public static bool operator !=(UserId me, object other) => !me?.Equals(other) == true;
	}


	internal static class UserIdExtension
	{
		public static UserId ToUserId(this string val) => val == null ? null : new UserId(val);
	}
}
