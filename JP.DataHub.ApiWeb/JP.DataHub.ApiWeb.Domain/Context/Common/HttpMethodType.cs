using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record HttpMethodType : IValueObject
    {
        public enum MethodTypeEnum
        {
            UNKNOWN,
            GET,
            POST,
            PUT,
            DELETE,
            HEAD,
            METHOD,
            OPTIONS,
            TRACE,
            PATCH,
        }

        [Required]
        public string Code { get; }
        public MethodTypeEnum Value { get; }

        public bool IsGet { get => Value == MethodTypeEnum.GET; }
        public bool IsPost {  get => Value == MethodTypeEnum.POST; }
        public bool IsPut { get => Value == MethodTypeEnum.PUT; }
        public bool IsPatch { get => Value == MethodTypeEnum.PATCH; }
        public bool IsDelete { get => Value == MethodTypeEnum.DELETE; }

        public HttpMethodType(MethodTypeEnum value)
        {
            Value = value;
            Code = value.ToString();
        }

        public HttpMethodType(string value)
        {
            Code = value;
            Value = value.ToUpper().ToEnum<MethodTypeEnum>();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(HttpMethodType me, object other) => me?.Equals(other) == true;

        public static bool operator !=(HttpMethodType me, object other) => !me?.Equals(other) == true;
    }

    internal static class HttpMethodTypeExtension
    {
        public static HttpMethodType ToHttpMethodType(this string str) => str == null ? null : new HttpMethodType(str);
    }
}
