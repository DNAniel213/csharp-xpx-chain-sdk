using System;
using System.Collections.Generic;
using System.Text;

namespace Xarcade.Infrastructure.Utilities
{
    public enum EmailType
    {
        Verification,
        PasswordReset,
        AlreadyRegistered
    }

    public class Email
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Origin { get; set; }
        public EmailType Type { get; set; }
        public string VerificationToken { get; set; }
        public string ResetToken { get; set; }
    }
}
