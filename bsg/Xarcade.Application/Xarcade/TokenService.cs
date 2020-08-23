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
                Mosaic mosaic = dataAccessProximaX.LoadMosaic(Convert.ToInt64(Token.TokenId));
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
                    TokenDto tdto = new TokenDto
                    {
                        TokenId     = Convert.ToUInt64(link.Asset.AssetID),
                        Name        = link.Asset.Name,
                        Quantity    = link.Asset.Quantity,
                        Owner       = link.Asset.Owner.UserID
                    };

                    tokentransaction = new TokenTransactionDto
                    {
                        Status      = State.Confirmed,
                        Hash        = link.Hash,
                        Token       = tdto,
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
        public async Task<List<TokenDto>> GetTokenListAsync(long userId, long gameId)
        {
            if (userId < 0 || gameId < 0)
            {
                Console.WriteLine("Invalid Input!");
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
                        TokenId     = Convert.ToUInt64(mosaic.AssetID),
                        Name        = mosaic.Name,
                        Quantity    = mosaic.Quantity,
                        Owner       = mosaic.Owner.UserID
                    };
                    Console.WriteLine(tokendto);
                    tokendtolist.Add(tokendto);
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
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

                Console.Write("Token quantity:");
                ulong quantity = Convert.ToUInt64(Console.ReadLine());

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
                Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicparam);

                Mosaic mosaic = new Mosaic
                {
                    AssetID = Convert.ToInt64(xar.TokenId),
                    Name = "XarcadeToken",
                    Quantity = quantity,
                    Owner = createMosaicT.Owner,
                    Created = createMosaicT.Created,
                    MosaicID = createMosaicT.MosaicID,
                    Namespace = null
                };
                dataAccessProximaX.SaveMosaic(mosaic);

                var modsupply = new ModifyMosaicSupplyParams
                {
                    Account   =  account,
                    MosaicID  =  createMosaicT.MosaicID,
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
                //Retrieves a list of namespaces of the owner
                //var nslist = repo.portal.ReadCollection("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, Owner>("Owner", ownerdto), FilterOperator.EQUAL));
                //foreach(var nsdocu in nslist)
                //{
                //    Namespace namesp = BsonToModel.BsonToGameDTO(nsdocu);
                //    Console.WriteLine(namesp);
                //}
                Namespace game = dataAccessProximaX.LoadNamespace(NamespaceName);
                if(game.Owner.UserID != ownerdto.UserID)
                {
                    Console.WriteLine("This is not your namespace!");
                    return null;
                }

                Console.Write("Token quantity:");
                ulong amount = Convert.ToUInt64(Console.ReadLine());

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

                Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicparam);
                Mosaic mosaic = new Mosaic
                {
                    AssetID     = Convert.ToInt64(Token.TokenId),
                    Name        = game.Domain,
                    Quantity    = Token.Quantity,
                    Owner       = ownerdto,
                    Created     = DateTime.Now,
                    MosaicID    = createMosaicT.MosaicID,
                    Namespace   = game
                };
                this.dataAccessProximaX.SaveMosaic(mosaic);
                Asset tokenasset = new Asset
                {
                    AssetID     = Convert.ToInt64(Token.TokenId),
                    Name        = Token.Name,
                    Quantity    = amount,
                    Owner       = ownerdto,
                    Created     = mosaic.Created
                };
                //Adds supply to mosaic i hope
                var modsupply = new ModifyMosaicSupplyParams
                {
                    Account   =  account,
                    MosaicID  =  createMosaicT.MosaicID,
                    Amount    =  Convert.ToInt32(amount),
                };
                var supplied = await blockchainPortal.ModifyMosaicSupplyAsync(modsupply);

                Transaction transaction = new Transaction
                {
                    Hash    = supplied.Hash,
                    Height  = supplied.Height,
                    Asset   = tokenasset,
                    Created = mosaic.Created
                };
                this.dataAccessProximaX.SaveTransaction(transaction);

                TokenDto tokendto = new TokenDto
                {
                    TokenId     = Token.TokenId,
                    Name        = Token.Name,
                    Quantity    = amount,
                    Owner       = ownerdto.UserID,
                };

                tokentransaction = new TokenTransactionDto
                {
                    Status      = State.Unconfirmed,
                    Hash        = transaction.Hash,
                    Token       = tokendto,
                    BlockNumber = transaction.Height, 
                    Created     = mosaic.Created
                };

            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
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
                    Console.WriteLine("Invalid Game. Game already exists!");
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
                Console.WriteLine("Extending " + Game.Name + " duration by " + duration);
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
                Console.WriteLine(e.ToString());
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
                Mosaic mosaicDto = dataAccessProximaX.LoadMosaic(Convert.ToInt64(Token.TokenId));
                //modify mosaic supply
                Console.Write("Amount to modify:  ");
                int amount = Convert.ToInt32(Console.ReadLine());
                Console.Write("Modifying " + mosaicDto.MosaicID + " supply by " + amount );

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
                Console.WriteLine(e.ToString());
                return null;
            }

            return null;
        }

        public async Task<TokenDto> GetTokenInfoAsync(long TokenId)
        {
            if (TokenId < 0)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }

            TokenDto tokenInfo = null;
            Mosaic mosaicDto = dataAccessProximaX.LoadMosaic(TokenId);

            if(mosaicDto == null)
            {
                Console.WriteLine("Mosaic not found!");
            }
            else
            {
                Console.WriteLine(mosaicDto.MosaicID);
                var mosaicinfo = await blockchainPortal.GetMosaicAsync(mosaicDto.MosaicID);
                if(mosaicinfo != null)
                {
                    tokenInfo = new TokenDto
                    {
                        TokenId     = Convert.ToUInt64(mosaicinfo.AssetID),
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

        public async Task<GameDto> GetGameInfoAsync(long GameId)
        {
            if (GameId < 0)
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