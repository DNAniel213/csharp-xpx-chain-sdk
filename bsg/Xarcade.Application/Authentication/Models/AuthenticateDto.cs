using Xarcade.Domain.Authentication;

namespace Xarcade.Application.Authentication.Models
{
    public enum AuthenticationError
    {
        Ok,
        InvalidParameters,
        InvalidIpAddress,
        AccountNotVerified,
        InvalidPassword,
        AccountUpdateFailed
    }

    public class AuthenticateDto
    {
        public AccountDto Account { get; set; }
        public string JwtToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public AuthenticationError AuthenticationError { get; set; }
    }
}
