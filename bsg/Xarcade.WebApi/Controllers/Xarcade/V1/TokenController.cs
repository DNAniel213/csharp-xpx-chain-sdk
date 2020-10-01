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
        public async Task<Response> CreateToken(string name,  string owner, string namespaceName, ulong quantity)
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
                    Quantity = quantity,
                    Owner    = owner
                };

                var tokenTransaction = await tokenService.CreateTokenAsync(tokenDto, namespaceName);

                if(tokenTransaction != null)
                {
                    var tokenTransactionViewModel = new TransactionViewModel
                    {
                        Hash = tokenTransaction.Hash,
                        Created = tokenTransaction.Created,
                        Status = tokenTransaction.Status + ""
                    };
                    response.Message = "Success!";
                    response.ViewModel = tokenTransactionViewModel;
                }
                else
                {
                    response.Message = "Transaction Unsuccessful!";
                }

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

            var gameDto = new GameDto
            {
                //GameId = Guid.NewGuid().ToString(),
                Name = name,
                Owner = owner,
                Expiry = DateTime.Now 
            };
            var game = await tokenService.CreateGameAsync(gameDto);
            if(game== null)
            {
                response.Message = "Transaction unsuccessful";
                response.ViewModel = null;
                return response;
            }

            var transactionViewModel = new TransactionViewModel
            {
                Hash = game.Hash,
                Created = game.Created,
                Status = game.Status.ToString()
            };
            response.Message = "Transaction Pending!";
            response.ViewModel = transactionViewModel;


            return response;
        }

        [HttpPut]
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
                if(linkTransaction== null)
                {
                    response.Message = "Transaction unsuccessful";
                    response.ViewModel = null;
                    return response;
                }

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

        [HttpPut]
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

            var gameDto = new GameDto
            {
                GameId = gameId,
            };


            var gameTransactionDTO = await tokenService.ExtendGameAsync(gameDto, duration); 
            if(gameTransactionDTO== null)
            {
                response.Message = "Transaction unsuccessful";
                response.ViewModel = null;
                return response;
            }


            var gameTransactionViewModel = new TransactionViewModel
            {
                Hash = gameTransactionDTO.Hash,
                Created = gameTransactionDTO.Created,
                Status = gameTransactionDTO.Status +"",
            };
            
            response.Message = "Success!";
            response.ViewModel = gameTransactionViewModel;

            return response;
        }

        [HttpPut]
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

            var tokenDto = new TokenDto
            {
                TokenId = tokenId,
                Quantity =  supply 
            };
            var tokenTransaction = await tokenService.ModifyTokenSupplyAsync(tokenDto);

            if(tokenTransaction== null)
            {
                response.Message = "Transaction unsuccessful";
                response.ViewModel = null;
                return response;
            }

            var tokenTransactionViewModel = new TransactionViewModel
            {
                Hash = tokenTransaction.Hash,
                Created = tokenTransaction.Created,
                Status = tokenTransaction.Status.ToString()
            };
            response.Message = "Success!";
            response.ViewModel = tokenTransactionViewModel;

            return response;
        }



        [HttpGet]
        [Route(Routes.Token)]        
        public async Task<TokenViewModel> GetTokenInfo(string userId, string tokenId)
        {
            if(String.IsNullOrWhiteSpace(userId) || String.IsNullOrWhiteSpace(tokenId)) return null;
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



            var token = await tokenService.GetTokenInfoAsync(tokenId);
            if(token== null) return null;

            tokenViewModel.Name = token.Name;
            tokenViewModel.Quantity = token.Quantity;
            tokenViewModel.TokenId = token.TokenId;


            return tokenViewModel;
        }

        [HttpGet]
        [Route(Routes.TokenList)]        
        public async Task<List<TokenViewModel>> GetTokenList(string userId)
        {
            var tokenViewModels = new List<TokenViewModel>();
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

            if(String.IsNullOrWhiteSpace(userId)) 
            {
                tokenViewModels = null;
            }


            try
            {
                var tokenList = await tokenService.GetTokenListAsync(userId);
                if(tokenList.Count > 0)
                {
                    foreach(TokenDto tok in tokenList)
                    {
                        var token = new TokenViewModel
                        {
                            Name = tok.Name,
                            Quantity = tok.Quantity,
                            TokenId  = tok.TokenId
                        };
                        tokenViewModels.Add(token);
                    }
                }

            }catch(Exception e)
            {
                tokenViewModels = null;
            }

            return tokenViewModels;
        }
        
        [HttpGet]
        [Route(Routes.GameList)]        
        public async Task<List<GameViewModel>> GetGameList(string userId)
        {
            var gameViewModels = new List<GameViewModel>();
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

            if(String.IsNullOrWhiteSpace(userId)) 
            {
                gameViewModels = null;
            }


            try
            {
                var gameList = await tokenService.GetGameListAsync(userId);

                foreach(GameDto game in gameList)
                {
                    var gameViewModel = new GameViewModel
                    {
                        Name = game.Name,
                        Expiry = game.Expiry,

                    };
                    gameViewModels.Add(gameViewModel);
                }

            }catch(Exception e)
            {
                Console.WriteLine(e);
                gameViewModels = null;
            }

            return gameViewModels;
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
                var tokenList = await tokenService.GetTokenListAsync(userId);
                gameViewModel.Name = game.Name;
                gameViewModel.Expiry = game.Expiry;
                gameViewModel.Tokens = new List<TokenViewModel>();
                if(tokenList.Count > 0)
                {
                    gameViewModel.Tokens = new List<TokenViewModel>();
                    foreach(TokenDto tok in tokenList)
                    {
                        var token = new TokenViewModel
                        {
                            Name = tok.Name,
                            Quantity = tok.Quantity,
                            TokenId  = tok.TokenId
                        };
                        gameViewModel.Tokens.Add(token);
                    }
                }

            }catch(Exception e)
            {
                gameViewModel = null;
            }

            return gameViewModel;
        }

/*
        [HttpGet]
        [Route(Routes.Game)]        
        public async Task<List<TokenViewModel>> GetTokenList(string userId)
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
*/
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
            return response;
        }
    }
}
