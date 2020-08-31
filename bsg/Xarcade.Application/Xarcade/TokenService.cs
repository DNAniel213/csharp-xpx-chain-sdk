using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Infrastructure.ProximaX.Params;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Domain.ProximaX;
using Xarcade.Application.ProximaX;

namespace Xarcade.Application.Xarcade
{
    public class TokenService:ITokenService
    {
        private readonly IDataAccessProximaX dataAccessProximaX;
        private readonly IBlockchainPortal blockchainPortal;
        private static ILogger _logger;

        public TokenService(IDataAccessProximaX dataAccessProximaX, IBlockchainPortal blockchainPortal)
        {
            this.dataAccessProximaX = dataAccessProximaX;
            this.blockchainPortal = blockchainPortal;
        }
        //link the mosaic to namespace
        public async Task<TokenTransactionDto> RegisterTokenAsync(TokenDto Token, GameDto Game)
        {
            if(Token == null || Game == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            
            TokenTransactionDto tokentransaction = null;

            try
            {
                Owner ownerdto = dataAccessProximaX.LoadOwner(Token.Owner);
                Mosaic mosaic = dataAccessProximaX.LoadMosaic(Token.TokenId);
                Namespace game = dataAccessProximaX.LoadNamespace(Game.Name);

                Account account = new Account
                {
                    UserID          = ownerdto.UserID,
                    WalletAddress   = ownerdto.WalletAddress,
                    PrivateKey      = ownerdto.PrivateKey,
                    PublicKey       = ownerdto.PublicKey,
                    Created         = ownerdto.Created
                };
                //Links Mosaic To Namespace
                var linkparam = new LinkMosaicParams
                {
                    Account   =  account,
                    MosaicID  =  mosaic.MosaicID,
                    Namespace =  game,
                };
                var link = await blockchainPortal.LinkMosaicAsync(linkparam);
                if (link != null)
                {
                    TokenDto tokendto = new TokenDto
                    {
                        TokenId     = link.Asset.AssetID,
                        Name        = link.Asset.Name,
                        Quantity    = link.Asset.Quantity,
                        Owner       = link.Asset.Owner.UserID
                    };

                    tokentransaction = new TokenTransactionDto
                    {
                        Status      = State.Confirmed,
                        Hash        = link.Hash,
                        Token       = tokendto,
                        BlockNumber = link.Height,
                        Created     = link.Created
                    };
                }
                else
                {
                    _logger.LogInfo("LinkMosaicAsync failed!");
                }
                
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return tokentransaction;
        }
        public async Task<List<TokenDto>> GetTokenListAsync(string userId, string gameId)
        {
            if (String.IsNullOrWhiteSpace(userId) || String.IsNullOrWhiteSpace(gameId))
            {
                return null;
            }
            
            List<TokenDto> tokendtolist = new List<TokenDto>();
            try
            {
                
                Owner ownerdto = dataAccessProximaX.LoadOwner(userId);
                var tokenlist = dataAccessProximaX.LoadMosaicList(ownerdto);
                //var nsresult = repo.portal.ReadDocument("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, long>("NamespaceId", gameId), FilterOperator.EQUAL));
                foreach (var token in tokenlist)
                {
                    Mosaic mosaic = BsonToModel.BsonToTokenDTO(token);
                    TokenDto tokendto = new TokenDto
                    {
                        TokenId     = mosaic.AssetID,
                        Name        = mosaic.Name,
                        Quantity    = mosaic.Quantity,
                        Owner       = mosaic.Owner.UserID
                    };
                    tokendtolist.Add(tokendto);
                }
                
            }
            catch(Exception e)
            {
                return null;
            }

            return tokendtolist;
        }
        //create xarcade token
        public async Task<TokenTransactionDto> CreateXarTokenAsync(XarcadeTokenDto xar)
        {
            if (xar == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            TokenTransactionDto tokentransaction = null;
            try
            {
                Owner ownerdto = dataAccessProximaX.LoadOwner(xar.Owner);

                ulong quantity = 0;

                Account account = new Account
                {
                    UserID          = ownerdto.UserID,
                    WalletAddress   = ownerdto.WalletAddress,
                    PrivateKey      = ownerdto.PrivateKey,
                    PublicKey       = ownerdto.PublicKey,
                    Created         = ownerdto.Created
                };
                //Creates Mosaic
                var mosaicparam = new CreateMosaicParams
                {
                    Account = account,
                };
                var mosaicTuple = await blockchainPortal.CreateMosaicAsync(mosaicparam);

                Mosaic mosaic = new Mosaic
                {
                    AssetID = xar.TokenId,
                    Name = "XarcadeToken",
                    Quantity = quantity,
                    Owner = mosaicTuple.tMosaic.Owner,
                    Created = mosaicTuple.tMosaic.Created,
                    MosaicID = mosaicTuple.tMosaic.MosaicID,
                    Namespace = null
                };
                dataAccessProximaX.SaveMosaic(mosaic);

                var modsupply = new ModifyMosaicSupplyParams
                {
                    Account   =  account,
                    MosaicID  =  mosaicTuple.tMosaic.MosaicID,
                    Amount    =  Convert.ToInt32(quantity),
                };
                var supplied = await blockchainPortal.ModifyMosaicSupplyAsync(modsupply);
                dataAccessProximaX.SaveTransaction(supplied);
                
                TokenDto token = new TokenDto
                {
                    TokenId = xar.TokenId,
                    Name = mosaic.Name,
                    Quantity = quantity,
                    Owner = account.UserID
                };

                tokentransaction = new TokenTransactionDto
                {
                    Status = State.Unconfirmed,
                    Hash = supplied.Hash,
                    Token = token,
                    BlockNumber = supplied.Height,
                    Created = supplied.Created
                };
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            return tokentransaction;
        }
        public async Task<TokenTransactionDto> CreateTokenAsync(TokenDto Token, string NamespaceName)
        {
            if (Token == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            TokenTransactionDto tokentransaction = null;

            try
            {

                Owner ownerdto = dataAccessProximaX.LoadOwner(Token.Owner);

                Namespace game = dataAccessProximaX.LoadNamespace(NamespaceName);

                if(game != null)
                {
                    if(game.Owner.UserID != ownerdto.UserID)
                    {
                        
                        return null;
                    }
                }
                ulong amount = Token.Quantity;

                Account account = new Account
                {
                    UserID          = ownerdto.UserID,
                    WalletAddress   = ownerdto.WalletAddress,
                    PrivateKey      = ownerdto.PrivateKey,
                    PublicKey       = ownerdto.PublicKey,
                    Created         = ownerdto.Created
                };
                //Creates Mosaic
                var mosaicparam = new CreateMosaicParams
                {
                    Account = account,
                };

                var mosaicTuple = await blockchainPortal.CreateMosaicAsync(mosaicparam);

                Mosaic mosaic = new Mosaic
                {
                    AssetID     = Token.TokenId,
                    Name        = Token.Name,
                    Quantity    = Token.Quantity,
                    Owner       = ownerdto,
                    Created     = DateTime.Now,
                    MosaicID    = mosaicTuple.tMosaic.MosaicID,
                    Namespace   = game
                };
                this.dataAccessProximaX.SaveMosaic(mosaic);

                var tokendto = new TokenDto
                {
                    TokenId     = Token.TokenId,
                    Name        = Token.Name,
                    Quantity    = amount,
                    Owner       = ownerdto.UserID,
                };

                tokentransaction = new TokenTransactionDto
                {
                    Status      = State.Unconfirmed,
                    Hash        = mosaicTuple.tx.Hash,
                    Token       = tokendto,
                    BlockNumber = mosaicTuple.tx.Height, 
                    Created     = mosaic.Created
                };

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            
            return tokentransaction;
        }

        public async Task<TokenTransactionDto> CreateGameAsync(GameDto Game)
        {
            if (Game == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            
            try
            {
                var gamecheck = dataAccessProximaX.CheckExistNamespace(Game.Name);
                if(gamecheck == true)
                {
                    return null;
                }else
                {
                    Owner ownerdto = dataAccessProximaX.LoadOwner(Game.Owner);
                    var gameparam = new CreateNamespaceParams
                    {
                        Account     = ownerdto,
                        Domain      = Game.Name,
                        Duration    = 1000,
                        Parent      = null,
                    };
                    //Creates Game
                    var createGame = await blockchainPortal.CreateNamespaceAsync(gameparam);
                    Namespace game = new Namespace
                    {
                        NamespaceId     = createGame.gameName.NamespaceId,
                        Domain          = createGame.gameName.Domain,
                        LayerOne        = createGame.gameName.LayerOne,
                        LayerTwo        = createGame.gameName.LayerTwo,
                        Owner           = createGame.gameName.Owner,
                        Expiry          = createGame.gameName.Expiry,
                        Created         = createGame.gameName.Created
                    };
                    this.dataAccessProximaX.SaveNamespace(game);
                }
                

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return null;
        }

        public async Task<TokenTransactionDto> ExtendGameAsync(GameDto Game, ulong duration)
        {
            if (Game == null || duration <= 0)
            {
                _logger.LogError("At least one input is empty!");
                return null;
            }

            try
            {
                Owner ownerdto = dataAccessProximaX.LoadOwner(Game.Owner);
                var param = new CreateNamespaceParams
                {
                    Account = ownerdto,
                    Domain = Game.Name,
                    Duration = duration,
                };
                var namespaceInfo = await blockchainPortal.GetNamespaceInformationAsync(Game.Name);
                var extendGame = await blockchainPortal.ExtendNamespaceDurationAsync(Game.Name,ownerdto.PrivateKey,namespaceInfo,param);
                this.dataAccessProximaX.SaveNamespace(extendGame);

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return null;
        }

        public async Task<TokenTransactionDto> ModifyTokenSupplyAsync(TokenDto Token)
        {
            if (Token == null)
            {
                _logger.LogError("Input is invaid!!");
                return null;
            }

            try
            {
                Owner ownerdto = dataAccessProximaX.LoadOwner(Token.Owner);
                Mosaic mosaicDto = dataAccessProximaX.LoadMosaic(Token.TokenId);
                //modify mosaic supply
                int amount = Convert.ToInt32(Console.ReadLine());
                

                var modifyparam = new ModifyMosaicSupplyParams
                {
                    Account = ownerdto,
                    MosaicID = mosaicDto.MosaicID,
                    Amount = amount
                };

                var modifyMosaicT = await blockchainPortal.ModifyMosaicSupplyAsync(modifyparam);

                Transaction t = new Transaction
                {
                    Hash    = modifyMosaicT.Hash,
                    Height  = modifyMosaicT.Height,
                    Asset   = modifyMosaicT.Asset,
                    Created = modifyMosaicT.Created,
                };
                this.dataAccessProximaX.SaveTransaction(t);

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return null;
        }

        public async Task<TokenDto> GetTokenInfoAsync(string TokenId)
        {
            if (String.IsNullOrWhiteSpace(TokenId))
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }

            TokenDto tokenInfo = null;
            Mosaic mosaicDto = dataAccessProximaX.LoadMosaic(TokenId);

            if(mosaicDto == null)
            {
                _logger.LogInfo("Mosaic not found!");
            }
            else
            {
                var mosaicinfo = await blockchainPortal.GetMosaicAsync(mosaicDto.MosaicID);
                if(mosaicinfo != null)
                {
                    tokenInfo = new TokenDto
                    {
                        TokenId     = mosaicinfo.AssetID,
                        Name        = mosaicinfo.Name,
                        Quantity    = mosaicinfo.Quantity,
                        Owner       = mosaicinfo.Owner.UserID,
                    };
                }
                else
                {
                    _logger.LogInfo("GetMosaicAsync failed!");
                }
            }

            return tokenInfo;
        }

        public async Task<GameDto> GetGameInfoAsync(string GameId)
        {
            if (String.IsNullOrWhiteSpace(GameId))
            {
                _logger.LogError("Game ID is not valid!");
                return null;
            }
            
            GameDto gameInfo = null;
            Namespace gameDto = dataAccessProximaX.LoadNamespace(GameId);
            gameInfo = new GameDto
            {
                GameId  = gameDto.NamespaceId,
                Name    = gameDto.Domain,
                Owner   = gameDto.Owner.UserID,
                Expiry  = gameDto.Expiry
            };
            
            return gameInfo;
        }

    }
}