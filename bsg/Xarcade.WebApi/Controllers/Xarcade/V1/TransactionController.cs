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

namespace Xarcade.WebApi.Controllers.Xarcade.V1
{
    public class TransactionController : ControllerBase
    {
        public readonly ITransactionService transactionService = null;
        public readonly IAccountService accountService = null;
        public readonly ITokenService tokenService = null;
        public TransactionController(ITransactionService transactionService, IAccountService accountService, ITokenService tokenService)
        {
            this.transactionService = transactionService;
            this.accountService = accountService;
            this.tokenService = tokenService;
        }

        /*

        [HttpPost]
        [Route(Routes.SendToken)]
        public async Task<Response> SendToken(long senderId, long receiverId, long amount, string message, long tokenId)
        {
            Response response = new Response();

            if(senderId < 0 || receiverId < 0 || amount < 0 || tokenId < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var token = new TokenDto
                {
                    TokenId = tokenId,
                    Quantity = amount
                };
                
                var sender = await accountService.GetUserAccountAsync(senderId, 0);
                var receiver = await accountService.GetUserAccountAsync(receiverId, 0);
                var tokenTransaction = transactionService.SendTokenAsync(token, sender, receiver);
                
                var tokenTransactionViewModel = new TransactionViewModel
                {
                    Hash = tokenTransaction.Hash,
                    Created = tokenTransaction.Created,
                    Status = tokenTransaction.Status.ToString()
                };

                response.Message = "Success!";
                response.ViewModel = tokenViewModel;
            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }
        }

        */


    }

}