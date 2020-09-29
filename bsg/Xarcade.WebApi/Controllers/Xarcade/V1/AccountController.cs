using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xarcade.WebApi.Controllers.Xarcade.Models;
using Xarcade.Application.Xarcade;
using Xarcade.Application.ProximaX;
using Xarcade.Application.Xarcade.Models.Account;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Application.Authentication;
using Xarcade.WebApi.Controllers.Authentication.Models.Request;
using Xarcade.WebApi.Controllers.Authentication.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xarcade.Application.Authentication;
using Xarcade.Application.Authentication.Models;
using Org.BouncyCastle.Ocsp;
using System;

namespace Xarcade.WebApi.Controllers.Xarcade.V1
{
    public class AccountController : ControllerBase
    {
        public readonly IAccountService accountService = null;
        private readonly IXarcadeAccountService xarcadeAccountService = null;

        public AccountController(IAccountService accountService, IXarcadeAccountService xarcadeAccountService)
        {
            this.accountService = accountService;
            this.xarcadeAccountService = xarcadeAccountService;

        }

        [HttpGet]
        [Route(Routes.Owner)]
        public async Task<OwnerViewModel> GetOwner([FromQuery] string userId, string searchId)  //userId -> current authenticated user, searchId -> ang gi search
        {
            if(String.IsNullOrWhiteSpace(userId))  return null;

            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, userId);

            Response response = new Response();

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return null;
            }

            if (!string.Equals(authorizedUser.UserId, userId))
            {
                response.Message = "User is not authenticated!";
                return null;
            }


            var ownerViewModel = new OwnerViewModel();
            try
            {
                var ownerDto = await accountService.GetOwnerAccountAsync(searchId);
                var xarcadeUser = await this.xarcadeAccountService.GetXarcadeUserAsync(searchId);

                if (xarcadeUser == null)
                {
                    response.Message = "User does not exist"!;
                    return null;
                }

                ownerViewModel.WalletAddress = ownerDto.WalletAddress;
                ownerViewModel.Name = xarcadeUser.FirstName + " " + xarcadeUser.LastName;
                ownerViewModel.Email = xarcadeUser.Email;

            }catch(Exception e)
            {
                Console.WriteLine(e);

                ownerViewModel = null;
            }

            return ownerViewModel;
        }

        [HttpGet]
        [Route(Routes.User)]
        public async Task<UserViewModel> GetUser([FromQuery] string userId, string searchId)
        {
            if(string.IsNullOrWhiteSpace(userId)) return null;
            Response response = new Response();

            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, userId);

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return null;
            }

            if (!string.Equals(authorizedUser.UserId, userId))
            {
                response.Message = "User is not authenticated!";
                return null;
            }

            var userViewModel = new UserViewModel();

            try
            {

                var userDto = await accountService.GetUserAccountAsync(searchId);
                var xarcadeUser = await this.xarcadeAccountService.GetXarcadeUserAsync(searchId);

                if (xarcadeUser == null)
                {
                    response.Message = "User does not exist"!;
                    return null;
                }

                userViewModel.WalletAddress = userDto.WalletAddress;
                userViewModel.Name = xarcadeUser.FirstName + " " + xarcadeUser.LastName;
                userViewModel.Email = xarcadeUser.Email;
            }catch(Exception e)
            {
                userViewModel = null; 
            }

            return userViewModel;
        }

        [HttpPost]
        [Route(Routes.GenerateOwner)]
        public async Task<Response> CreateOwnerWallet(string userId) 
        {
            Response response = new Response();

            if(String.IsNullOrWhiteSpace(userId))
            {
                response.Message = "Empty Request";
                return response;
            }
            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, userId);

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


            if(string.IsNullOrWhiteSpace(userId)) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            var ownerDto = await accountService.CreateOwnerAccountAsync(userId);
            if(ownerDto== null)
            {
                response.Message = "Transaction unsuccessful";
                response.ViewModel = null;
                return response;
            }
            var ownerViewModel = new OwnerViewModel
            {
                WalletAddress = ownerDto.WalletAddress,
                Name = null,
                Email = null,
                Users = null
            };

            response.Message = "Success!";
            response.ViewModel = ownerViewModel;


            return response;
        }

        [HttpPost]
        [Route(Routes.GenerateUser)]
        public async Task<Response> CreateUserWallet(string ownerId) //TODO Propose email and Name
        {
            Response response = new Response();

            if(String.IsNullOrWhiteSpace(ownerId))
            {
                response.Message = "Empty Request";
                return response;
            }
            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, ownerId);

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return response;
            }

            if (!string.Equals(authorizedUser.UserId, ownerId))
            {
                response.Message = "User is not authenticated!";
                return response;
            }


            if(string.IsNullOrWhiteSpace(ownerId)) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            var userDto = await accountService.CreateUserAccountAsync(Guid.NewGuid().ToString(), ownerId);
            if(userDto== null)
            {
                response.Message = "Transaction unsuccessful";
                response.ViewModel = null;
                return response;
            }

            var userViewModel = new UserViewModel
            {
                WalletAddress = userDto.WalletAddress,
                Name = null,
                Email = null,
            };

            response.Message = "Success!";
            response.ViewModel = userViewModel;

            return response;
        }
    }
}