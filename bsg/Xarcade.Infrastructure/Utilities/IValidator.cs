using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Xarcade.Infrastructure.Utilities
{
    public interface IValidator
    {
        XarcadeValidationResult Validate<TParam>(TParam parameter);
    }
}
