using JP.DataHub.Com.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JP.DataHub.Com.Extensions
{
    public static class JSchemaTypeExtensions
    {
        public static Type ToType(this List<JSchema> schemas, string key)
        {
            foreach (JSchema schema in schemas)
            {
                var prop = schema?.Properties.SingleOrDefault(s => s.Key == key);
                if (prop != null && prop.HasValue && prop.Value.Value != null)
                {
                    return prop.Value.Value.Type.ToType();
                }
            }
            return typeof(string);
        }

        public static Type ToType(this JSchemaType? type)
        {
            if (type == null)
            {
                return typeof(string);
            }
            switch (type.Value)
            {
                case JSchemaType.None:
                case JSchemaType.Array:
                case JSchemaType.Null:
                case JSchemaType.String:
                    return typeof(string);
                case JSchemaType.Number:
                    return typeof(double);
                case JSchemaType.Boolean:
                    return typeof(bool);
                case JSchemaType.Integer:
                    return typeof(int);
                default:
                    return typeof(string);
            }
        }

        public static bool IsDateTimeFormat(this JSchema schema)
        {
            return ((schema.Type & JSchemaType.String) == JSchemaType.String && schema.Format == "date-time");
        }

        public static bool IsFlatType(this JSchemaType? type)
        {
            return ((type & JSchemaType.Object) != JSchemaType.Object &&
                    (type & JSchemaType.Array) != JSchemaType.Array);
        }

        public static bool IsSameOrNullable(this JSchemaType? myType, JSchemaType comparisonType)
        {
            return (myType & ~JSchemaType.Null) == comparisonType;
        }
    }
}
