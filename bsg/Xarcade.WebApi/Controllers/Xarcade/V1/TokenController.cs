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
    public class TokenController : ControllerBase
    {
        public readonly ITokenService tokenService = null;
        private readonly IXarcadeAccountService xarcadeAccountService;

        public TokenController(ITokenService tokenService, IXarcadeAccountService xarcadeAccountService)
        {
            this.tokenService = tokenService;
            this.xarcadeAccountService = xarcadeAccountService;
        }

        [HttpPost]
        [Route(Routes.GenerateToken)]
        public async Task<Response> CreateToken(string name,  string owner, string namespaceName)
        {

            Response response = new Response();

            if(string.IsNullOrWhiteSpace(name) ||string.IsNullOrWhiteSpace(owner)) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }
            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, owner);

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return null;
            }

            if (!string.Equals(authorizedUser.UserId, owner))
            {
                response.Message = "User is not authenticated!";
                return null;
            }

            try
            {
                var tokenDto = new TokenDto
                {
                    TokenId =  Guid.NewGuid().ToString(),
                    Name = name,
                    Quantity = 0,
                    Owner    = owner
                };

                var tokenTransaction = await tokenService.CreateTokenAsync(tokenDto);

                var tokenTransactionViewModel = new TransactionViewModel
                {
                    Hash = tokenTransaction.Hash,
                    Created = tokenTransaction.Created,
                    Status = tokenTransaction.Status + ""
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


        [HttpPost]
        [Route(Routes.GenerateGame)]

        public async Task<Response> CreateGame(string name, long duration, string owner)
        {
            Response response = new Response();

            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, owner);

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return null;
            }

            if (!string.Equals(authorizedUser.UserId, owner))
            {
                response.Message = "User is not authenticated!";
                return null;
            }

            if(string.IsNullOrWhiteSpace(name) || duration < 0 || string.IsNullOrWhiteSpace(owner)) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var gameDto = new GameDto
                {
                    //GameId = Guid.NewGuid().ToString(),
                    Name = name,
                    Owner = owner,
                    Expiry = DateTime.Now 
                };
                var game = await tokenService.CreateGameAsync(gameDto);

                var transactionViewModel = new TransactionViewModel
                {
                    Hash = game.Hash,
                    Created = game.Created,
                    Status = game.Status.ToString()
                };
                response.Message = "Transaction Pending!";
                response.ViewModel = transactionViewModel;
            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }

            return response;
        }

        [HttpPost]
        [Route(Routes.RegisterGame)]
        public async Task<Response> RegisterGame(string userId, string gameId, string tokenId )
        {
            Response response = new Response();

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

            if(string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(gameId)|| string.IsNullOrWhiteSpace(tokenId)) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var token = await tokenService.GetTokenInfoAsync(tokenId);
                var game = await tokenService.GetGameInfoAsync(gameId);

                if(game.Owner != token.Owner)
                {
                    response.Message = "Owner mismatch!";
                    return response;
                }
                var linkTransaction = await tokenService.RegisterTokenAsync(token, game);
                var transactionViewModel = new TransactionViewModel
                {
                    Hash = linkTransaction.Hash,
                    Created = linkTransaction.Created,
                    Status = linkTransaction.Status.ToString()
                };
                response.Message = "Transaction Pending!";
                response.ViewModel = transactionViewModel;

            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }

            return response;
        }

        [HttpPost]
        [Route(Routes.ExtendGame)]
        public async Task<Response> ExtendGame(string owner, string gameId, ulong duration)
        {
            Response response = new Response();
            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, owner);

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return response;
            }

            if (!string.Equals(authorizedUser.UserId, owner))
            {
                response.Message = "User is not authenticated!";
                return response;
            }

            if(string.IsNullOrWhiteSpace(gameId) || duration < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var gameDto = new GameDto
                {
                    GameId = gameId,
                };
                var gameTransactionDTO = await tokenService.ExtendGameAsync(gameDto, duration); 

                var gameTransactionViewModel = new TransactionViewModel
                {
                    Hash = gameTransactionDTO.Hash,
                    Created = gameTransactionDTO.Created,
                    Status = gameTransactionDTO.Status +"",
                };
                
                response.Message = "Success!";
                response.ViewModel = gameTransactionViewModel;
            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }

            return response;
        }

        [HttpPost]
        [Route(Routes.ModifyTokenSupply)]
        public async Task<Response> ModifyTokenSupply(string userId, string tokenId, ulong supply)
        {
            Response response = new Response();

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
            if(string.IsNullOrWhiteSpace(tokenId) || supply < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var tokenDto = new TokenDto
                {
                    TokenId = tokenId,
                    Quantity =  supply 
                };
                var tokenTransaction = await tokenService.ModifyTokenSupplyAsync(tokenDto);


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

        [HttpPost]
        [Route(Routes.GenerateXarToken)]
        public async Task<Response> CreateXarToken(string name, ulong supply, string owner)
        {
            
            Response response = new Response();
            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, owner);

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                response.Message = "Authenticated user list are null!";
                return response;
            }

            if (!string.Equals(authorizedUser.UserId, owner))
            {
                response.Message = "User is not authenticated!";
                return response;
            }
            if(string.IsNullOrWhiteSpace(name) || supply < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }
            /*
            try
            {
                var xarDto = new XarcadeTokenDto
                {
                    TokenId = 0, //TODO needs to be generated
                    Name = name,
                    Owner = owner,
                };

                var xarTokenTransaction = await tokenService.CreateTokenAsync(xarDto);

                var xarTokenTransactionViewModel = new TransactionViewModel
                {
                    Hash = xarTokenTransaction.Hash,
                    Created = xarTokenTransaction.Created,
                    Status = xarTokenTransaction.Status.ToString()
                };

                response.Message = "Success!";
                response.ViewModel = xarTokenTransactionViewModel;
            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }
*/
            return response;
        }

        [HttpGet]
        [Route(Routes.Token)]        
        public async Task<TokenViewModel> GetTokenInfo(string userId, string tokenId)
        {
            TokenViewModel tokenViewModel = new TokenViewModel();
            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, userId);

            if (authorizedUser == null)
            {
                Console.WriteLine(HttpContext.Items[0]);
                //TODO add logger

                return null;
            }

            if (!string.Equals(authorizedUser.UserId, userId))
            {
                //TODO add logger

                return null;
            }

            if(String.IsNullOrWhiteSpace(tokenId)) 
            {
                tokenViewModel = null;
            }


            try
            {
                var token = await tokenService.GetTokenInfoAsync(tokenId);

                tokenViewModel.Name = token.Name;
                tokenViewModel.Quantity = token.Quantity;
                
            }catch(Exception e)
            {
                tokenViewModel = null;
            }

            return tokenViewModel;
        }

        [HttpGet]
        [Route(Routes.Game)]        
        public async Task<GameViewModel> GetGameInfo(string userId, string gameId)
        {
            var gameViewModel = new GameViewModel();

            TokenViewModel tokenViewModel = new TokenViewModel();
            var authorizedUser = this.xarcadeAccountService.GetAuthorizedXarcadeUser(HttpContext.Items, userId);

            if (authorizedUser == null)
            {
                //TODO add logger

                Console.WriteLine(HttpContext.Items[0]);
                return null;
            }

            if (!string.Equals(authorizedUser.UserId, userId))
            {
                //TODO add logger
                return null;
            }

            if(string.IsNullOrWhiteSpace(gameId)) 
            {
               gameViewModel = null;
            }

            try
            {

                var game = await tokenService.GetGameInfoAsync(gameId);
                //TODO add GET ALL TOKENS under game

                gameViewModel.Name = game.Name;
                gameViewModel.Expiry = game.Expiry;
                gameViewModel.Tokens = null;//TODO ALL TOKENS IN GAME


            }catch(Exception e)
            {
                gameViewModel = null;
            }

            return gameViewModel;
        }
        


    }
}