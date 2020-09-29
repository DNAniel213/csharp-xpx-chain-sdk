using System.Threading.Tasks;
using System.Collections.Generic;
using  Xarcade.Application.Authentication.Models;

namespace Xarcade.Application.Authentication
{
    public interface IXarcadeAccountService
    {
        Task<bool> RegisterAccountAsync(AccountDto account, string origin);
        Task<bool> ForgotPasswordAsync(string email, string origin);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetDetails, string userId);
        Task<AuthenticateDto> AuthenticateAsync(LoginDto loginDetails, string ipAddress);
        Task<AuthenticateDto> RefreshTokenAsync(string token, string ipAddress);
        Task<bool> RevokeTokenAsync(string token, string ipAddress);
        Task<bool> VerifyEmailAsync(string token);
        Task<AccountDto> GetXarcadeUserAsync(string userId);
        Task<bool> UpdateXarcadeUserAsync(AccountDto account);
        AccountDto GetAuthorizedXarcadeUser(IDictionary<object, object> context, string userId);

    }
}
