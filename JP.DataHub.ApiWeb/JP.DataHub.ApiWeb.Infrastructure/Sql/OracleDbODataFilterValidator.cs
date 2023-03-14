using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.OData.Interface;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    // .NET6
    internal class OracleDbODataFilterValidator : IODataFilterValidator
    {
        private static Lazy<IConfigurationSection> _lasyAppConfig = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("AppConfig"));
        protected static IConfigurationSection AppConfig { get => _lasyAppConfig.Value; }

        public bool IsFilterValueUnescapeEnabled => _IsFilterValueUnescapeEnabled;
        private static readonly bool _IsFilterValueUnescapeEnabled = AppConfig.GetValue<bool>("IsFilterValueUnescapeEnabledOnSqlServerOData");

        private JSchema JSchema { get; set; }
        private RelativeUri RelativeUri { get; set; }


        public OracleDbODataFilterValidator(JSchema jSchema)
        {
            JSchema = jSchema;
        }


        /// <summary>
        /// filterの条件のバリデーションと条件値のフォーマットを行う。
        /// </summary>
        public object ValidateAndFormat(string propertyName, object value)
        {
            if (JSchema == null)
            {
                return value;
            }

            // 存在しないプロパティでの検索はNotFound (NoSQLでの動作に合わせる)
            if (!JSchema.Properties.ContainsKey(propertyName))
            {
                throw CreateException(ErrorCodeMessage.Code.E10422);
            }

            // 検索値の形式が不正な場合はエラー
            var schema = JSchema.Properties[propertyName];
            if ((schema.Type & JSchemaType.String) == JSchemaType.String)
            {
                if (schema.Format == "date-time")
                {
                    return FormatDateTime(propertyName, value);
                }
                else
                {
                    return FormatString(propertyName, value);
                }
            }
            else if ((schema.Type & JSchemaType.Number) == JSchemaType.Number)
            {
                return FormatNumber(propertyName, value);
            }
            else if ((schema.Type & JSchemaType.Boolean) == JSchemaType.Boolean)
            {
                return FormatBoolean(propertyName, value);
            }
            else if ((schema.Type & JSchemaType.Object) == JSchemaType.Object ||
                     (schema.Type & JSchemaType.Array) == JSchemaType.Array)
            {
                // オブジェクト・配列の検索はサポートしない
                throw CreateInvalidFilterException(propertyName);
            }

            return value;
        }

        /// <summary>
        /// Boolean型の項目かどうか
        /// </summary>
        public bool IsBooleanProperty(string propertyName)
        {
            if (!JSchema.Properties.ContainsKey(propertyName))
            {
                return false;
            }

            return ((JSchema.Properties[propertyName].Type & JSchemaType.Boolean) == JSchemaType.Boolean);
        }


        private object FormatDateTime(string propertyName, object value)
        {
            if (value is string strValue &&
                DateTime.TryParse(strValue, out var datetimeValue))
            {
                // タイムゾーン指定ありのDateTimeはUTCに変換
                // タイムゾーン指定なしならそのまま
                if (datetimeValue.Kind == DateTimeKind.Local)
                {
                    datetimeValue = datetimeValue.ToUniversalTime();
                }

                return datetimeValue;
            }
            else
            {
                throw CreateInvalidFilterException(propertyName, true);
            }
        }

        private object FormatString(string propertyName, object value)
        {
            if (value is string)
            {
                return value;
            }
            else
            {
                throw CreateInvalidFilterException(propertyName, true);
            }
        }

        private object FormatNumber(string propertyName, object value)
        {
            if (value is decimal)
            {
                return value;
            }
            else
            {
                throw CreateInvalidFilterException(propertyName, true);
            }
        }

        private object FormatBoolean(string propertyName, object value)
        {
            if (value is bool)
            {
                return value;
            }
            else
            {
                throw CreateInvalidFilterException(propertyName, true);
            }
        }

        private Exception CreateInvalidFilterException(string propertyName, bool isWrongValue = false)
        {
            var message = isWrongValue
                ? ErrorCodeMessage.GetString(nameof(DynamicApiMessages.SyntaxErrorOfOData_InvalidFilterValue))
                : ErrorCodeMessage.GetString(nameof(DynamicApiMessages.SyntaxErrorOfOData_InvalidFilterColumn));
            var errors = new Dictionary<string, dynamic>() { { propertyName, new string[] { message } } };

            return CreateException(ErrorCodeMessage.Code.E10426, errors);
        }

        private Rfc7807Exception CreateException(ErrorCodeMessage.Code code, Dictionary<string, dynamic> errors = null)
        {
            var rfc7807 = ErrorCodeMessage.GetRFC7807(code, RelativeUri?.Value);
            rfc7807.Errors = errors;

            return new Rfc7807Exception(rfc7807);
        }
    }
}
