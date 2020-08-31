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

namespace Xarcade.WebApi.Controllers.Xarcade.V1
{
    public class TokenController : ControllerBase
    {
        public readonly ITokenService tokenService = null;
        public TokenController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        [HttpPost]
        [Route(Routes.GenerateToken)]
        public async Task<Response> CreateToken(string name,  long owner, string namespaceName)
        {
            Response response = new Response();

            if(string.IsNullOrWhiteSpace(name) || owner < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var tokenDto = new TokenDto
                {
                    TokenId =  0, //TODO needs to be generated
                    Name = name,
                    Quantity = 0,
                    Owner    = owner
                };

                var tokenTransaction = await tokenService.CreateTokenAsync(tokenDto, namespaceName);
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
        public async Task<Response> CreateGame(string name, long duration, long owner)
        {
            Response response = new Response();

            if(string.IsNullOrWhiteSpace(name) || duration < 0 || owner < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var gameDto = new GameDto
                {
                    GameId = 00, //TODO needs to be generated
                    Name = name,
                    Owner = owner,
                    Expiry = DateTime.Now //TODO needs long
                };
                var game = await tokenService.CreateGameAsync(gameDto);

                var gameViewModel = new GameViewModel
                {
                    Name = name,
                    Expiry = DateTime.Now, //TODO needs long conversion
                    Tokens = null
                };
                response.Message = "Success!";
                response.ViewModel = gameViewModel;
            }catch(Exception e)
            {
                response.Message = e.ToString();
                response.ViewModel = null;
            }

            return response;
        }

        [HttpPost]
        [Route(Routes.ExtendGame)]
        public async Task<Response> ExtendGame(long gameId, ulong duration)
        {
            Response response = new Response();

            if(gameId < 0 || duration < 0) 
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
                var gameTransactionDTO = await tokenService.ExtendGameAsync(gameDto, duration); //TODO propose add duration to make things simpler

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
        public async Task<Response> ModifyTokenSupply(ulong tokenId, ulong supply)
        {
            Response response = new Response();

            if(tokenId < 0 || supply < 0) 
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
                    Quantity =  supply //TODO needs long
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
        public async Task<Response> CreateXarToken(string name, ulong supply, long owner)
        {
            Response response = new Response();

            if(string.IsNullOrWhiteSpace(name) || supply < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            try
            {
                var xarDto = new XarcadeTokenDto
                {
                    TokenId = 0, //TODO needs to be generated
                    Name = name,
                    Owner = owner,
                };

                var xarTokenTransaction = await tokenService.CreateXarTokenAsync(xarDto);

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

            return response;
        }

        [HttpGet]
        [Route(Routes.Token)]        
        public async Task<TokenViewModel> GetTokenInfo(long tokenId)
        {
            TokenViewModel tokenViewModel = new TokenViewModel();

            if(tokenId < 0) 
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
        [Route(Routes.Token)]        
        public async Task<GameViewModel> GetGameInfo(long gameId)
        {
            var gameViewModel = new GameViewModel();

            if(gameId < 0) 
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