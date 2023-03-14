using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;
using JP.DataHub.Com.Validations.Attributes;
using JP.DataHub.Com.Extensions;
using Unity.Interception.PolicyInjection.Pipeline;

namespace JP.DataHub.Com.Validations
{
    public static class ValidatorEx
    {
        public static bool TryValidateValue(object value, ValidationContext validationContext, ICollection<ValidationResult> validationResults, ValidationAttribute validationAttribute) => Validator.TryValidateValue(value, validationContext, validationResults, new ValidationAttribute[] { validationAttribute });


        public static void ExceptionValidateObject(object obj, bool validateAllProperties = true)
        {
            if (obj == null)
            {
                throw new NullReferenceException("obj is null");
            }

            List<ValidationResult> result = new List<ValidationResult>();
            var type = obj.GetType();
            var props = type.GetProperties();
            var validationContext = new ValidationContext(obj, null, null);
            List<Exception> exceptions = new List<Exception>();
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var etas = prop.GetCustomAttributes().ToList();
                for (int j = 0; j < etas?.Count; j++)
                {
                    if (etas[j] is ValidationAttribute va)
                    {
                        var val = prop.GetValue(obj);
                        if (TryValidateValue(val, validationContext, result, va) == false)
                        {
                            var ivae = va as IValidationAttributeEx;
                            Type exceptionType = (ivae?.ExceptionType == null) ? typeof(Exception) : ivae.ExceptionType;
                            var exception = exceptionType.Create<Exception>(new object[] { result.Last().ErrorMessage });
                            //var exception = new ArgumentNullException(result[i].ErrorMessage);
                            //var exception = new Exception(result[i].ErrorMessage);
                            exceptions.Add(exception);
                        }
                    }
                }
                if (validateAllProperties == false && exceptions.Count != 0)
                {
                    throw new AggregateException(exceptions);
                }
            }
            if (exceptions.Count != 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        public static void ExceptionValidateValueMethodInvocation(this IMethodInvocation input, bool validateAllArguments = true)
        {
            if (input == null)
            {
                throw new NullReferenceException("input is null");
            }

            for (int i = 0; i < input.Arguments.Count; i++)
            {
                var arg = input.Arguments[i];
                var method = input.MethodBase;
                var ps = method.GetParameters();
                var gca = ps[0].GetCustomAttributes();
                Console.WriteLine($"{arg}");
            }
        }

        public static void ExceptionValidateValue(this IMethodInvocation input)
        {
            List<Exception> exceptions = new List<Exception>();
            var parameters = input.MethodBase.GetParameters();
            for (int i = 0; i < input.Arguments.Count; i++)
            {
                var arg = input.Inputs[i];
                var val = input.Arguments[i];
                var pi = input.Arguments.GetParameterInfo(i);
                foreach (var va in pi.GetCustomAttributes<ValidationAttribute>())
                {
                    var validationContext = new ValidationContext(typeof(Type), null, null);
                    List<ValidationResult> result = new List<ValidationResult>();
                    if (TryValidateValue(val, validationContext, result, va) == false)
                    {
                        var ivae = va as IValidationAttributeEx;
                        Type exceptionType = (ivae?.ExceptionType == null) ? typeof(Exception) : ivae.ExceptionType;
                        var exception = exceptionType.Create<Exception>(new object[] { "Method " + input.MethodBase.ToString() + " " + result[0].ErrorMessage.Replace("RuntimeType field",parameters[i].Name + " parameter") });
                        //var exception = new ArgumentNullException(result[i].ErrorMessage);
                        //var exception = new Exception(result[i].ErrorMessage);
                        exceptions.Add(exception);
                    }
                }
            }
            if (exceptions.Count != 0)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}