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
/*
        [HttpPost]
        [Route(Routes.GenerateToken)]
        public async Task<Response> CreateToken(string name, ulong supply, long owner)
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
                var tokenDto = new TokenDto
                {
                    TokenId = "Needs to be generated", //TODO needs to be generated
                    Name = name,
                    Quantity = supply,
                    Owner    = owner
                };

                var token = await tokenService.CreateTokenAsync(tokenDto);
                var tokenViewModel = new TokenViewModel
                {
                    Name = name,
                    Quantity = supply
                };
                response.Message = "Success!";
                response.ViewModel = tokenViewModel;
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

            if(string.IsNullOrWhiteSpace(name) || supply < 0) 
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
                    Expiry = DateTime.Now() //TODO needs long
                };
                var game = await tokenService.CreateGameAsync(gameDto);

                var gameViewModel = new GameViewModel
                {
                    Name = name,
                    Expiry = DateTime.Now(), //TODO needs long conversion
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
        public async Task<Response> ExtendGame(long gameId, long duration)
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
                    Name = name,
                };
                var game = await tokenService.ExtendGameAsync(gameDto); //TODO propose add duration to make things simpler

                var gameViewModel = new GameViewModel
                {
                    Name = game.Name,
                    Expiry = game.Expiry, //TODO return new expiry date
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
        [Route(Routes.ModifyTokenSupply)]
        public async Task<Response> ModifyTokenSupply(long tokenId, long supply)
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
        public async Task<Response> CreateXarToken(string name, ulong supply, long Owner)
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
                    TokenId = "Needs to be generated", //TODO needs to be generated
                    Name = name,
                    Owner = owner,
                };

                var xarTokenTransaction = await tokenService.CreateXarToken(xarDto);

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

            if(tokenId < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            TokenViewModel tokenViewModel = new TokenViewModel();

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

            if(gameId < 0) 
            {
                response.ViewModel = null;
                response.Message = "Missing or incorrect parameters";
                return response;
            }

            var gameViewModel = new GameViewModel();
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


    }
}