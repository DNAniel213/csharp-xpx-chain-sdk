using Xarcade.Application.Authentication.Models;

namespace Xarcade.WebApi.Controllers.Authentication.Models.Response
{
    public class AuthenticateResponse : AuthenticationResponse
    {
        public AuthenticateDto AuthenticationData { get; set; }
    }
}
