using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record RepositoryGroupId : IValueObject
    {
        public string Value { get; }

        public RepositoryGroupId(string reposiitoryGroupId)
        {
            this.Value = reposiitoryGroupId;
        }

        public static bool operator ==(RepositoryGroupId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(RepositoryGroupId me, object other) => !me?.Equals(other) == true;
    }

    internal static class RepositoryGroupIdExtension
    {
        public static RepositoryGroupId ToRepositoryGroupId(this string val) => val == null ? null : new RepositoryGroupId(val);
    }
}
