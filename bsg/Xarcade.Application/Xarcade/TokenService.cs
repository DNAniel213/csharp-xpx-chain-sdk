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
            Owner ownerDto = dataAccessProximaX.LoadOwner(Token.Owner);
            if(ownerDto == null)
            {
                _logger.LogInfo("Owner not found!");
                return null;
            }
            Mosaic mosaic = dataAccessProximaX.LoadMosaic(Convert.ToInt64(Token.TokenId));
            if(mosaic == null)
            {
                _logger.LogInfo("Mosaic not found!");
                return null;
            }
            Namespace game = dataAccessProximaX.LoadNamespace(Game.Name);
            if(game == null)
            {
                _logger.LogInfo("Game not found!");
                return null;
            }

            Account account = new Account
            {
                UserID          = ownerDto.UserID,
                WalletAddress   = ownerDto.WalletAddress,
                PrivateKey      = ownerDto.PrivateKey,
                PublicKey       = ownerDto.PublicKey,
                Created         = ownerDto.Created
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
                TokenDto tokenDto = new TokenDto
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
                    Token       = tokenDto,
                    BlockNumber = link.Height,
                    Created     = link.Created
                };
            }
            else
            {
                _logger.LogInfo("LinkMosaicAsync failed!");
                return null;
            }
                
            return tokentransaction;
        }
        public async Task<List<TokenDto>> GetTokenListAsync(long userId, long gameId)
        {
            if (userId < 0 || gameId < 0)
            {
                _logger.LogInfo("Invalid Input!");
                return null;
            }
            
            List<TokenDto> tokenDtoList = new List<TokenDto>();
            Owner ownerDto = dataAccessProximaX.LoadOwner(userId);
            if(ownerDto == null)
            {
                _logger.LogInfo("Owner not found!");
                return null;
            }

            var tokenList = dataAccessProximaX.LoadMosaicList(ownerDto);
            Namespace nsResult = dataAccessProximaX.LoadNamespace(gameId);
            if(nsResult == null)
            {
                _logger.LogInfo("Game not found!");
                return null;
            }

            foreach (var token in tokenList)
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
                tokenDtoList.Add(tokendto);
            }
                
            return tokenDtoList;
        }
        //create xarcade token
        public async Task<TokenTransactionDto> CreateXarTokenAsync(XarcadeTokenDto xar)
        {
            if (xar == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            TokenTransactionDto tokenTransaction = null;
            Owner ownerDto = dataAccessProximaX.LoadOwner(xar.Owner);
            if(ownerDto == null)
            {
                _logger.LogInfo("Owner not found!");
                return null;
            }

            Account account = new Account
            {
                UserID          = ownerDto.UserID,
                WalletAddress   = ownerDto.WalletAddress,
                PrivateKey      = ownerDto.PrivateKey,
                PublicKey       = ownerDto.PublicKey,
                Created         = ownerDto.Created
            };

            var mosaicParam = new CreateMosaicParams
            {
                Account = account,
            };
            
            Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicParam);
            if(createMosaicT == null)
            {
                _logger.LogInfo("Mosaic creation failed!");
                return null;
            }

            Mosaic mosaic = new Mosaic
            {
                AssetID = Convert.ToInt64(xar.TokenId),
                Name = "XarcadeToken",
                Quantity = xar.Quantity,
                Owner = createMosaicT.Owner,
                Created = createMosaicT.Created,
                MosaicID = createMosaicT.MosaicID,
                Namespace = null
            };
            dataAccessProximaX.SaveMosaic(mosaic);

            var tokenSupply = new ModifyMosaicSupplyParams
            {
                Account   =  account,
                MosaicID  =  createMosaicT.MosaicID,
                Amount    =  Convert.ToInt32(xar.Quantity),
            };
            var supplied = await blockchainPortal.ModifyMosaicSupplyAsync(tokenSupply);
            if(supplied != null)
            { 
                dataAccessProximaX.SaveTransaction(supplied);
            }
            else
            {
                _logger.LogInfo("Modifying supply failed!");
                return null;
            }
                
            TokenDto token = new TokenDto
            {
                TokenId = xar.TokenId,
                Name = mosaic.Name,
                Quantity = xar.Quantity,
                Owner = account.UserID
            };

            tokenTransaction = new TokenTransactionDto
            {
                Status = State.Unconfirmed,
                Hash = supplied.Hash,
                Token = token,
                BlockNumber = supplied.Height,
                Created = supplied.Created
            };
            
            return tokenTransaction;
        }
        public async Task<TokenTransactionDto> CreateTokenAsync(TokenDto Token, string NamespaceName)
        {
            if (Token == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            
            TokenTransactionDto tokenTransaction = null;
            Namespace game = null;
            Owner ownerDto = dataAccessProximaX.LoadOwner(Token.Owner);
            if(ownerDto == null)
            {
                _logger.LogInfo("Owner not found!");
                return null;
            }

            var tokenCheck = dataAccessProximaX.CheckExistToken(Token.Name);

            if(tokenCheck != true)
            {
                var gameCheck = dataAccessProximaX.CheckExistNamespace(NamespaceName);
                if(gameCheck != true)
                {
                    _logger.LogInfo("Game does not exist!");
                    return null;
                }
                game = dataAccessProximaX.LoadNamespace(NamespaceName);
                if(game.Owner.UserID != ownerDto.UserID)
                {
                    _logger.LogInfo("This is not your game!");
                    return null;
                }
                
            }

            Account account = new Account
            {
                UserID          = ownerDto.UserID,
                WalletAddress   = ownerDto.WalletAddress,
                PrivateKey      = ownerDto.PrivateKey,
                PublicKey       = ownerDto.PublicKey,
                Created         = ownerDto.Created
            };
            var mosaicParam = new CreateMosaicParams
            {
                Account = account,
            };
            Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicParam);
            if(createMosaicT == null)
            {
                _logger.LogInfo("Mosaic creation failed!");
                return null;
            }

            Mosaic mosaic = new Mosaic
            {
                AssetID     = Convert.ToInt64(Token.TokenId),
                Name        = game.Domain,
                Quantity    = Token.Quantity,
                Owner       = ownerDto,
                Created     = DateTime.Now,
                MosaicID    = createMosaicT.MosaicID,
                Namespace   = game
            };
            dataAccessProximaX.SaveMosaic(mosaic);
            Asset tokenAsset = new Asset
            {
                AssetID     = Convert.ToInt64(Token.TokenId),
                Name        = Token.Name,
                Quantity    = Token.Quantity,
                Owner       = ownerDto,
                Created     = mosaic.Created
            };
            
            var tokenSupply = new ModifyMosaicSupplyParams
            {
                Account   =  account,
                MosaicID  =  createMosaicT.MosaicID,
                Amount    =  Convert.ToInt32(Token.Quantity),
            };
            var supplied = await blockchainPortal.ModifyMosaicSupplyAsync(tokenSupply);
            if(supplied == null)
            {
                _logger.LogInfo("Modifying supply failed!");
                return null;
            }
            Transaction transaction = new Transaction
            {
                Hash    = supplied.Hash,
                Height  = supplied.Height,
                Asset   = tokenAsset,
                Created = mosaic.Created
            };
            dataAccessProximaX.SaveTransaction(transaction);

            TokenDto tokenDto = new TokenDto
            {
                TokenId     = Token.TokenId,
                Name        = Token.Name,
                Quantity    = Token.Quantity,
                Owner       = ownerDto.UserID,
            };

            tokenTransaction = new TokenTransactionDto
            {
                Status      = State.Unconfirmed,
                Hash        = transaction.Hash,
                Token       = tokenDto,
                BlockNumber = transaction.Height, 
                Created     = mosaic.Created
            };

            return tokenTransaction;
        }

        public async Task<TokenTransactionDto> CreateGameAsync(GameDto Game)
        {
            if (Game == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }

            var gameCheck = dataAccessProximaX.CheckExistNamespace(Game.Name);
            if(gameCheck == true)
            {
                _logger.LogInfo("Game already exists!");
                return null;
            }
            Owner ownerdto = dataAccessProximaX.LoadOwner(Game.Owner);
            var gameparam = new CreateNamespaceParams
            {
                Account     = ownerdto,
                Domain      = Game.Name,
                Duration    = 1000,
                Parent      = null,
            };
            
            var createGame = await blockchainPortal.CreateNamespaceAsync(gameparam);
            if(createGame.gameName == null || createGame.tx == null)
            {
                _logger.LogInfo("Game creation failed!");
                return null;
            }

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
            dataAccessProximaX.SaveNamespace(game);
            

            return null;
        }

        public async Task<TokenTransactionDto> ExtendGameAsync(GameDto Game, ulong duration)
        {
            if (Game == null || duration <= 0)
            {
                _logger.LogError("A parameter is null/empty!");
                return null;
            }

            Owner ownerDto = dataAccessProximaX.LoadOwner(Game.Owner);
            if(ownerDto == null)
            {
                _logger.LogInfo("Owner not found!");
                return null;
            }

            var param = new CreateNamespaceParams
            {
                Account = ownerDto,
                Domain = Game.Name,
                Duration = duration,
            };
            var namespaceInfo = await blockchainPortal.GetNamespaceInformationAsync(Game.Name);
            if(namespaceInfo == null)
            {
                _logger.LogInfo("Namespace not found!");
                return null;
            }
            var extendGame = await blockchainPortal.ExtendNamespaceDurationAsync(Game.Name,ownerDto.PrivateKey,namespaceInfo,param);
            if(extendGame == null)
            {
                _logger.LogInfo("Extending duration failed!");
                return null;
            }
            dataAccessProximaX.SaveNamespace(extendGame);

            return null;
        }

        public async Task<TokenTransactionDto> ModifyTokenSupplyAsync(TokenDto Token)
        {
            if (Token == null)
            {
                _logger.LogError("Input is invaid!!");
                return null;
            }

            Owner ownerDto = dataAccessProximaX.LoadOwner(Token.Owner);
            if(ownerDto == null)
            {
                _logger.LogInfo("Owner not found!");
                return null;
            }

            Mosaic mosaicDto = dataAccessProximaX.LoadMosaic(Convert.ToInt64(Token.TokenId));
            if(mosaicDto == null)
            {
                _logger.LogInfo("Mosaic not found!");
                return null;
            }

            var modifyParam = new ModifyMosaicSupplyParams
            {
                Account = ownerDto,
                MosaicID = mosaicDto.MosaicID,
                Amount = Convert.ToInt32(Token.Quantity)
            };

            var modifyMosaicT = await blockchainPortal.ModifyMosaicSupplyAsync(modifyParam);
            if(modifyMosaicT == null)
            {
                _logger.LogInfo("Modifying supply failed!");
                return null;
            }
            Transaction transaction = new Transaction
            {
                Hash    = modifyMosaicT.Hash,
                Height  = modifyMosaicT.Height,
                Asset   = modifyMosaicT.Asset,
                Created = modifyMosaicT.Created,
            };
            dataAccessProximaX.SaveTransaction(transaction);

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
                _logger.LogInfo("Mosaic not found!");
                return null;
            }
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
                _logger.LogInfo("Mosaic not found in the blockchain!");
                return null;
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
            if(gameDto == null)
            {
                _logger.LogInfo("Game not found!");
                return null;
            }
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