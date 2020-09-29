using System;

namespace Xarcade.Domain.Authentication
{
    public class RefreshToken
    {
        public int TokenId { get; set; }
        public XarcadeUser XarcadeUser { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public bool IsExpired { get; set; }
        public DateTime Created { get; set; }
        public string CreatorIp { get; set; }
        public DateTime Revoked { get; set; }
        public string RevokerIp { get; set; }
        public string ReplacementToken { get; set; }
        public bool IsActive { get; set; }
    }
}
