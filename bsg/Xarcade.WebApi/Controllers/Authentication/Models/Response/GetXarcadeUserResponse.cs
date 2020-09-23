using Xarcade.Application.Authentication.Models;

namespace Xarcade.WebApi.Controllers.Authentication.Models.Response
{
    public class GetXarcadeUserResponse : AuthenticationResponse
    {
        public AccountDto XarcadeUser { get; set; }
    }
}
