using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xarcade.Infrastructure.Utilities
{
    public class XarcadeValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
