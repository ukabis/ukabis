using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Validations
{
    public static class ValidationExtension
    {
        public static IDictionary<string,string> ToDictionaryErrors(this List<ValidationResult> validationResults)
        {
            var result = new Dictionary<string, string>();
            validationResults.ForEach(x => result.Add(x.ErrorMessage, string.Join(",", x.MemberNames)));
            return result;
        }
    }
}
