using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Xarcade.Infrastructure.Utilities
{
    public class XarcadeValidator : IValidator
    {
        public XarcadeValidationResult Validate<TParam>(TParam parameter)
        {
            if (IsNullOrDefault<TParam>(parameter))
            {
                return new XarcadeValidationResult
                {
                    IsValid = false,
                    ErrorMessages = new List<string>
                    {
                        "Parameter is null!"
                    }
                };
            }

            var context = new ValidationContext(parameter, serviceProvider: null, items: null);
            var errorResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(parameter, context, errorResults, true))
            {
                return new XarcadeValidationResult
                {
                    IsValid = false,
                    ErrorMessages = errorResults.Select(e => e.ErrorMessage).ToList()
                };
            }

            return new XarcadeValidationResult
            {
                IsValid = true,
                ErrorMessages = null
            };
        }

        private bool IsNullOrDefault<TReturn>(TReturn value)
        {
            if (EqualityComparer<TReturn>.Default.Equals(value, default(TReturn)))
                return true;
            return false;
        }
    }
}
