using System;

namespace Xarcade.Domain.Authentication
{
    public class VerificationDetails
    {
        public string VerificationToken { get; set; }
        public DateTime Verified { get; set; }
        public bool IsVerified { get; set; }
    }
}
