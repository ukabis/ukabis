using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Infrastructure.Core.Database;
using JP.DataHub.Infrastructure.Core.Validations.Attributes;

namespace JP.DataHub.Core.Validations
{
    public static class DbValidator
    {
        private static  Lazy<ICharacterLimit> _charLimit => new Lazy<ICharacterLimit>(() => UnityCore.Resolve<ICharacterLimit>());
        private static ICharacterLimit CharLimit => _charLimit.Value;

        public static bool TryValidate(object obj, ICollection<ValidationResult> validationResults) => TryValidate(obj, null, validationResults);

        public static bool TryValidate(object obj, ValidationContext validationContext, ICollection<ValidationResult> validationResults)
        {
            if (obj == null || CharLimit == null)
            {
                return true;
            }
            if (validationContext == null)
            {
                validationContext = new ValidationContext(obj);
            }

            var props = obj.GetType().GetProperties();
            foreach (var prop in props)
            {
                var val = prop.GetValue(obj);
                var cas = prop.GetCustomAttributes(true).ToList();
                var dbmap = cas.GetCustomAttribute<DbMapAttribute>();
                if (dbmap != null)
                {
                    cas = cas.RemoveCustomAttribute<DbMapAttribute>();
                    var dbpropcas = CharLimit.GetCustomAttribute(dbmap.Database, dbmap.Table, dbmap.Column);
                    cas.AddRange(dbpropcas);
                }
                foreach (var ca in cas)
                {
                    if (ca is KeyAttribute)
                    {
                        Guid nullguid = new Guid();
                        if (val == null || (val is string && string.IsNullOrEmpty(val as string) == true) || (val is Guid && (Guid)val == nullguid))
                        {
                            var member = new List<string>() { prop.Name };
                            validationResults.Add(new ValidationResult($"{prop.Name} is the key. Key field is required.", member));
                        }
                    }
                    else if (ca is MaxLengthAttribute mla)
                    {
                        string str = val?.ToString();
                        if (mla.Length < str?.Length)
                        {
                            var member = new List<string>() { prop.Name };
                            validationResults.Add(new ValidationResult($"The maximum number of {prop.Name} character is {mla.Length}.", member));
                        }
                    }
                    else if (ca is ValidationAttribute va)
                    {
                        var vcprop = new ValidationContext(prop) { DisplayName = prop.Name };
                        var vr = va.GetValidationResult(val, vcprop);
                        if (vr != null)
                        {
                            var member = new List<string>() { prop.Name };
                            var newvr = new ValidationResult(vr.ErrorMessage, member);
                            validationResults.Add(newvr);
                        }
                    }
                }
            }

            return validationResults.Count() == 0 ? true : false;
        }
    }
}
