using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator
{
    internal class NumberCustomValidator : IJsonValidator
    {
        public void Validate(JToken value, JsonValidatorContext context)
        {
            if (value.Type == JTokenType.Integer || value.Type == JTokenType.Float)
            {
                JsonPropertyFormatParser.ParseFormat(context.Schema.Format).Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.NumberDigit).ToList().ForEach(x => CheckNumberDigit(context, value, x.Match.Groups[x.KeyName1], x.Match.Groups[x.KeyName2]));
            }
            else if (value.Type == JTokenType.Array)
            {
                foreach (var v in value)
                {
                    Validate(v, context);
                }
            }
        }

        public bool CanValidate(JSchema schema) => JsonPropertyFormatParser.ParseFormat(schema.Format).Count > 0;

        private const string ErrorMessage = "The [Number] format does not match.";

        private void CheckNumberDigit(JsonValidatorContext context, JToken value, Group group1, Group group2)
        {
            var val = value.ToString();
            if (!string.IsNullOrEmpty(val))
            {
                int? d1len = string.IsNullOrEmpty(group1.Value) == true ? null : (int?)int.Parse(group1.Value);
                int? d2len = string.IsNullOrEmpty(group2.Value) == true ? null : (int?)int.Parse(group2.Value);
                if (d1len == null && d2len != null)
                {
                    d1len = d2len;
                    d2len = null;
                }
                string[] vals = val.Split('.');
                if (vals.Length != 0 && vals[0].StartsWith("-") == true)
                {
                    vals[0] = vals[0].Substring(1);
                }
                if ((vals.Length == 1 && vals[0].Length > d1len) || d1len == null)
                {
                    context.RaiseError(ErrorMessage);
                }
                else if (vals.Length == 2)
                {
                    if (d2len == null)
                    {
                        if (vals[0].Length + vals[1].Length > d1len)
                        {
                            context.RaiseError(ErrorMessage);
                        }
                    }
                    else
                    {
                        if (vals[0].Length + vals[1].Length > d1len || vals[1].Length > d2len)
                        {
                            context.RaiseError(ErrorMessage);
                        }
                    }
                }
                else if (vals.Length >= 3)
                {
                    context.RaiseError(ErrorMessage);
                }
            }
        }

    }
}
