using Xarcade.WebApi.Controllers.Authentication.Models.Request;
using Xarcade.WebApi.Controllers.Authentication.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xarcade.Application.Authentication;
using Xarcade.Application.Authentication.Models;
using Org.BouncyCastle.Ocsp;
using System;

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
        public async Task<RegisterResponse> Register([FromBody]RegisterRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
            {
                return new RegisterResponse()
                {
                    Message = "Empty Request!"
                };
            }
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
                Message = "Account registered! Please check your email"
            };
        }

        [HttpPost]
        [Route(Routes.Authenticate)]
        public async Task<AuthenticateResponse> Authenticate([FromBody]AuthenticateRequest request)
        {
            var loginDetails = new LoginDto()
            {
                Username = request.Username,
                Password = request.Password
            };

            var authenticateResult = await this.xarcadeAccountService.AuthenticateAsync(loginDetails, IpAddress());

            if (authenticateResult.AuthenticationError == AuthenticationError.Ok)
                SetTokenCookie(authenticateResult.RefreshToken.Token);

            return new AuthenticateResponse()
            {
                Message = GetAuthenticationErrorMessage(authenticateResult.AuthenticationError),
                AuthenticationData = authenticateResult
            };
        }

        [HttpPost]
        [Route(Routes.VerifyEmail)]
        public async Task<VerifyEmailResponse> VerifyEmail([FromBody]VerifyEmailRequest request)
        {
            var isVerified = await this.xarcadeAccountService.VerifyEmailAsync(request.Token);
            if (!isVerified)
            {
                return new VerifyEmailResponse()
                {
                    Message = "Email verification failed!"
                };
            }

            return new VerifyEmailResponse()
            {
                Message = "Ok"
            };
        }

        [HttpGet]
        [Route(Routes.GetXarUser)]
        public async Task<GetXarcadeUserResponse> GetXarcadeUser(string userId)
        {
            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, userId);
            var response = new GetXarcadeUserResponse();
            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return response;
            }

            if (!string.Equals(authorizedUser.UserId, userId))
            {
                response.Message = "User is not authenticated!";
                return response;
            }

            var xarcadeUser = await this.xarcadeAccountService.GetXarcadeUserAsync(userId);
            if (xarcadeUser == null)
            {
                response.Message = "User does not exist"!;
                return response;
            }

            response.Message = "Ok";
            response.XarcadeUser = xarcadeUser;
            return response;
        }

        private string GetAuthenticationErrorMessage(AuthenticationError error)
        {
            switch (error)
            {
                case AuthenticationError.InvalidParameters:
                    return "Invalid login details!";
                case AuthenticationError.InvalidIpAddress:
                    return "Ip address is empty!";
                case AuthenticationError.AccountNotVerified:
                    return "Account verification failed!";
                case AuthenticationError.InvalidPassword:
                    return "Password is invalid!";
                case AuthenticationError.AccountUpdateFailed:
                    return "An unexpected error occured!";
                case AuthenticationError.Ok:
                default:
                    return "Ok";
            }
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }



}
