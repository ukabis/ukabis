using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    internal class User : IEntity
	{
		public Vendor Vendor { get; set; }

		public SystemEntity System { get; set; }

		private UserId UserId { get; set; }

		public User(Vendor vendor, SystemEntity system, UserId userId)
		{
			Vendor = vendor;
			System = system;
			UserId = userId;
		}
	}
}
