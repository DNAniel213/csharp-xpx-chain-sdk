using System;

namespace Xarcade.Domain.Authentication
{
    public class PasswordResetDetails
    {
        public string ResetToken { get; set; }
        public DateTime PasswordReset { get; set; }
        public DateTime ResetTokenExpiry { get; set; }
    }
}
