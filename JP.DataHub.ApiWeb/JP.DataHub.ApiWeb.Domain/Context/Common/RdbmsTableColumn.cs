using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal record RdbmsTableColumn : IValueObject
    {
        [Required]
        public string Name { get; }
        [Required]
        public string DataType { get; }
        public bool Nullable { get; }
        public string DefaultValue { get; }
        public string Collate { get; }

        public RdbmsTableColumn(string name, string dataType, bool nullable = false, string defaultValue = null, string collate = null)
        {
            Name = name;
            DataType = dataType;
            Nullable = nullable;
            DefaultValue = defaultValue;
            Collate = collate;
        }

        public static bool operator ==(RdbmsTableColumn me, object other) => me?.Equals(other) == true;

        public static bool operator !=(RdbmsTableColumn me, object other) => !me?.Equals(other) == true;
    }
}
