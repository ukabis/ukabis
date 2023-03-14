using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record XResourceSharingWith : IValueObject
    {
        public IReadOnlyDictionary<string, string> Value { get; }

        public string this[string key] { get => Value == null ? null : Value?[key]; }

        public XResourceSharingWith(IDictionary<string, string> resourceSharingWith)
        {
            if (resourceSharingWith == null)
            {
                resourceSharingWith = new Dictionary<string, string>();
            }
            Value = new ReadOnlyDictionary<string,string>(resourceSharingWith);
        }

        public static bool operator ==(XResourceSharingWith me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XResourceSharingWith me, object other) => !me?.Equals(other) == true;
    }
}
