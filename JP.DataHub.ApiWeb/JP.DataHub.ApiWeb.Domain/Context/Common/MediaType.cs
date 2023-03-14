using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record MediaType : IValueObject
    {
        public string Value { get; }

        public bool IsXml { get => Value.ToLower().Contains(MediaTypeConst.ApplicationXml) || Value.ToLower().Contains(MediaTypeConst.TextXml); }
        public bool IsCsv { get => Value.ToLower().Contains(MediaTypeConst.TextCsv); }

        public MediaType(string value)
        {
            Value = value;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(MediaType me, object other) => me?.Equals(other) == true;

        public static bool operator !=(MediaType me, object other) => !me?.Equals(other) == true;
    }

    internal static class MediaTypeExtension
    {
        public static MediaType ToMediaType(this string str) => str == null ? null : new MediaType(str);
    }
}
