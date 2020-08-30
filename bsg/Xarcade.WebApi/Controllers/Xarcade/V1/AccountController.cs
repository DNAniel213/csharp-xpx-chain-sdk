using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xarcade.WebApi.Controllers.Xarcade.Models;
using Xarcade.Application.Xarcade;
using Xarcade.Application.Xarcade.Models.Account;

namespace Xarcade.WebApi.Controllers.Xarcade.V1
{
    public class AccountController : ControllerBase
    {
        public readonly IAccountService accountService = null;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        [Route(Routes.Owner)]
        public async Task<OwnerViewModel> GetOwner([FromQuery] long ownerId)
        {
            if(ownerId < 0)  return null;
            OwnerViewModel ownerViewModel = null;
            try
            {
                var ownerDto = await accountService.GetOwnerAccountAsync(ownerId);

                ownerViewModel.WalletAddress = ownerDto.WalletAddress;
                ownerViewModel.Name = "Dane";
                ownerViewModel.Email = "Dane@gmail.com";
            }catch(Exception e)
            {
                ownerViewModel = null;
            }

            return ownerViewModel;
        }

        [HttpGet]
        [Route(Routes.User)]
        public async Task<UserViewModel> GetUser([FromQuery] long userId)
        {
            if(userId < 0) return null;

            var userViewModel = new UserViewModel();

            try
            {

                var userDto = await accountService.GetUserAccountAsync(userId);

                userViewModel.WalletAddress = userDto.WalletAddress;
                userViewModel.Name = "Foo " + userId;
                userViewModel.Email = "Foo@bar.com";
            }catch(Exception e)
            {
                userViewModel = null; 
            }

            return userViewModel;
        }

        [HttpPost]
        [Route(Routes.GenerateOwner)]
        public async Task<Response> CreateOwnerWallet(long ownerId) //TODO Propose email and Name
        {

            Response response = new Response();

            if(ownerId < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var ownerDto = await accountService.CreateOwnerAccountAsync(ownerId);

                var ownerViewModel = new OwnerViewModel
                {
                    WalletAddress = ownerDto.WalletAddress,
                    Name = null,
                    Email = null,
                    Users = null
                };

                response.Message = "Success!";
                response.ViewModel = ownerViewModel;
            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }

            return response;
        }

        [HttpPost]
        [Route(Routes.GenerateUser)]
        public async Task<Response> CreateUserWallet(long userId, long ownerId) //TODO Propose email and Name
        {

            Response response = new Response();

            if(ownerId < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var userDto = await accountService.CreateUserAccountAsync(userId, ownerId);

                var userViewModel = new UserViewModel
                {
                    WalletAddress = userDto.WalletAddress,
                    Name = null,
                    Email = null,
                };

                response.Message = "Success!";
                response.ViewModel = userViewModel;
            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }

            return response;
        }
    }
}