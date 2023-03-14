using Newtonsoft.Json.Linq;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IJsonValidator
    {
        bool CanValidate(JSchema schema);
        void Validate(JToken value, JsonValidatorContext context);
    }
}