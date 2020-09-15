using authentication.prototype.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using authentication.prototype.Model.Request;
using Xarcade.Application.Authentication;
using Xarcade.Application.Authentication.Models;

namespace Xarcade.WebApi.Controllers.Authentication
{
    public class XarcadeAccountController : ControllerBase
    {
        private readonly IXarcadeAccountService xarcadeAccountService;

        public XarcadeAccountController(IXarcadeAccountService xarcadeAccountService)
        {
            this.xarcadeAccountService = xarcadeAccountService;
        }

        [HttpPost]
        [Route(Routes.Register)]
        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            if (!string.Equals(request.Password, request.ConfirmPassword))
            {
                return new RegisterResponse()
                {
                    Message = "Password does not match!" 
                };
            }

            var account = new AccountDto()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                AcceptTerms = request.AcceptTerms
            };

            var isOk = await this.xarcadeAccountService.RegisterAccountAsync(account, Request.Headers["origin"]);
            if (!isOk)
            {
                return new RegisterResponse()
                {
                    Message = "Password does not match!"
                };
            }

            return new RegisterResponse()
            {
                Message = "Account registered!"
            };
        }
    }


}
