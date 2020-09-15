using System;
using System.Collections.Generic;

namespace Xarcade.Domain.Authentication
{
    public class XarcadeUser
    {
        public string UserID { get; set; }
        public XarcadeUserDetails UserDetails { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public bool AcceptTerms { get; set; }
        public VerificationDetails Verification { get; set; }
        public PasswordResetDetails PasswordReset { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
