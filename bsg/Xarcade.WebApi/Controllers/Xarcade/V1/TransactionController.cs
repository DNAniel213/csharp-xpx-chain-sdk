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
using Xarcade.Application.Xarcade.Models.Transaction;
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
using System.Collections.Generic;


namespace Xarcade.WebApi.Controllers.Xarcade.V1
{
    public class TransactionController : ControllerBase
    {
        public readonly ITransactionService transactionService = null;
        public readonly IAccountService accountService = null;
        public readonly ITokenService tokenService = null;
        private readonly IXarcadeAccountService xarcadeAccountService;

        public TransactionController(ITransactionService transactionService, IAccountService accountService, ITokenService tokenService, IXarcadeAccountService xarcadeAccountService)
        {
            this.transactionService = transactionService;
            this.accountService = accountService;
            this.tokenService = tokenService;
                        this.xarcadeAccountService = xarcadeAccountService;

        }


        [HttpPost]
        [Route(Routes.SendToken)]
        public async Task<Response> SendToken([FromQuery]string senderId, string receiverId, string tokenId, ulong amount, string message)
        {
            Response response = new Response();

            if(String.IsNullOrWhiteSpace(senderId) || String.IsNullOrWhiteSpace(receiverId) || String.IsNullOrWhiteSpace(tokenId)  || amount < 0 ) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, senderId);

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return response;
            }

            if (!string.Equals(authorizedUser.UserId, senderId))
            {
                response.Message = "User is not authenticated!";
                return response;
            }

            try
            {
                var token = new TokenDto
                {
                    TokenId = tokenId,
                    Quantity = amount
                };
                
                var tokenTransaction = await transactionService.SendTokenAsync(token, senderId, receiverId, message);
                
                var tokenTransactionViewModel = new TransactionViewModel
                {
                    Hash = tokenTransaction.Hash,
                    Created = tokenTransaction.Created,
                    Status = tokenTransaction.Status.ToString()
                };

                response.Message = "Success!";
                response.ViewModel = tokenTransactionViewModel;
            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }
            return response;

        }

        [HttpGet]
        [Route(Routes.Transactions)]
        public async Task<List<TransactionViewModel>> GetTransactionList([FromQuery]string userId)
        {

            var transactions = new List<TransactionViewModel>();
            if(String.IsNullOrWhiteSpace(userId) ) 
            {
                return null;
            }

            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, userId);

            if (authorizedUser == null)
            {
                //TODO add LOGGER
                return null;
            }

            if (!string.Equals(authorizedUser.UserId, userId))
            {
                //TODO add LOGGER
                return null;
            }

            var transactionList = await transactionService.GetTransactionListAsync(userId);
            if(transactionList != null)
            {
                foreach(var iTransaction in transactionList)
                {
                    var transaction = new TransactionViewModel
                    {
                        Hash = iTransaction.Hash,
                        Created = iTransaction.Created,
                        Status = "Confirmed"
                    };

                    transactions.Add(transaction);
                }
            }

            return transactions;

        }





    }

}