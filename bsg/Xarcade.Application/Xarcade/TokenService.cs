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
        DataAccessProximaX repo = new DataAccessProximaX();

        public TokenService(IDataAccessProximaX dataAccessProximaX, IBlockchainPortal blockchainPortal)
        {
            this.dataAccessProximaX = dataAccessProximaX;
            this.blockchainPortal = blockchainPortal;
        }
        //link the mosaic to namespace
        public async Task<TokenTransactionDto> RegisterTokenAsync(TokenDto Token, GameDto Game)
        {
            return null;
        }
        public async Task<List<TokenDto>> GetTokenListAsync(long userId, long gameId)
        {
            return null;
        }
        //create xarcade token
        public async Task<TokenTransactionDto> CreateXarTokenAsync(XarcadeTokenDto Token)
        {
            if (Token == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            TokenTransactionDto ttd = null;
            try
            {
                var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Token.Owner), FilterOperator.EQUAL)); 
                Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);

                Console.Write("Token quantity:");
                ulong am = Convert.ToUInt64(Console.ReadLine());

                Account a = new Account
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
                    Account             = a,
                    //IsSupplyMutable   = ,
                    //IsLevyMutable     = ,
                    //IsTransferrable   = ,
                    //Divisibility      = ,
                    //Duration          = ,
                };
                Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicparam);
                Mosaic m = new Mosaic
                {
                    AssetID = Convert.ToInt64(Token.TokenId),
                    Name = null,
                    Quantity = Token.Quantity,
                    Owner = ownerdto,
                    Created = DateTime.Now,
                    MosaicID = createMosaicT.MosaicID,
                    Namespace = null
                };
                this.dataAccessProximaX.SaveMosaic(m);
                var modsupply = new ModifyMosaicSupplyParams
                {
                    Account   =  a,
                    MosaicID  =  createMosaicT.MosaicID,
                    Amount    =  Convert.ToInt32(am),
                };
                var supplied = await blockchainPortal.ModifyMosaicSupplyAsync(modsupply);

                Asset tokenasset = new Asset
                {
                    AssetID = Convert.ToInt64(Token.TokenId),
                    Name = "XarcadeToken",
                    Quantity = am,
                    Owner = ownerdto,
                    Created = m.Created
                };

                TokenDto t = new TokenDto
                {
                    TokenId = tokenasset.AssetID,
                    Name = tokenasset.Name,
                    Quantity = tokenasset.Quantity,
                    Owner = tokenasset.Owner.UserID
                };

                ttd = new TokenTransactionDto
                {
                    Status = State.Unconfirmed,
                    Hash = null,
                    Token = t,
                    BlockNumber = 0,
                    Created = tokenasset.Created
                };
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return ttd;
        }
        public async Task<TokenTransactionDto> CreateTokenAsync(TokenDto Token, string NamespaceName)
        {
            if (Token == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            TokenTransactionDto ttd = null;

            try
            {
                var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Token.Owner), FilterOperator.EQUAL)); 
                Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);
                //Retrieves a list of namespaces of the owner
                //var nslist = repo.portal.ReadCollection("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, Owner>("Owner", ownerdto), FilterOperator.EQUAL));
                //foreach(var nsdocu in nslist)
                //{
                //    Namespace namesp = BsonToModel.BsonToGameDTO(nsdocu);
                //    Console.WriteLine(namesp);
                //}
                var nsDocu =  repo.portal.ReadDocument("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, string>("Domain", NamespaceName), FilterOperator.EQUAL));
                Namespace nsdto = BsonToModel.BsonToGameDTO(nsDocu);
                if(nsdto.Owner.UserID != ownerdto.UserID)
                {
                    Console.WriteLine("This is not your namespace!");
                    return null;
                }

                Console.Write("Token quantity:");
                ulong am = Convert.ToUInt64(Console.ReadLine());

                Account a = new Account
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
                    Account             = a,
                    //IsSupplyMutable   = ,
                    //IsLevyMutable     = ,
                    //IsTransferrable   = ,
                    //Divisibility      = ,
                    //Duration          = ,
                };
                Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicparam);
                Mosaic m = new Mosaic
                {
                    AssetID = Convert.ToInt64(Token.TokenId),
                    Name = nsdto.Domain,
                    Quantity = Token.Quantity,
                    Owner = ownerdto,
                    Created = DateTime.Now,
                    MosaicID = createMosaicT.MosaicID,
                    Namespace = nsdto
                };
                this.dataAccessProximaX.SaveMosaic(m);
                Asset tokenasset = new Asset
                {
                    AssetID = Convert.ToInt64(Token.TokenId),
                    Name = Token.Name,
                    Quantity = am,
                    Owner = ownerdto,
                    Created = m.Created
                };

                //Links Mosaic To Namespace
                var linkparam = new LinkMosaicParams
                {
                    Account   =  a,
                    MosaicID  =  createMosaicT.MosaicID,
                    Namespace =  nsdto,
                };
                var link = await blockchainPortal.LinkMosaicAsync(linkparam);
                //Adds supply to mosaic
                var modsupply = new ModifyMosaicSupplyParams
                {
                    Account   =  a,
                    MosaicID  =  createMosaicT.MosaicID,
                    Amount    =  Convert.ToInt32(am),
                };
                var supplied = await blockchainPortal.ModifyMosaicSupplyAsync(modsupply);

                Transaction t = new Transaction
                {
                    Hash = link.Hash,
                    Height = link.Height,
                    Asset = tokenasset,
                    Created = m.Created
                };
                this.dataAccessProximaX.SaveTransaction(t);

                TokenDto td = new TokenDto
                {
                    TokenId = Token.TokenId,
                    Name = Token.Name,
                    Quantity = am,
                    Owner = ownerdto.UserID,
                };

                ttd = new TokenTransactionDto
                {
                    Status = 0,
                    Hash = t.Hash,
                    Token = td,
                    BlockNumber = 0,
                    Created = m.Created
                };

            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            
            return ttd;
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
                var nsExist = repo.portal.CheckExist("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, string>("Domain", Game.Name), FilterOperator.EQUAL));
                if(nsExist == true)
                {
                    Console.WriteLine("Invalid Namespace. Namespace already exists!");
                    return null;
                }else
                {
                    var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Game.Owner), FilterOperator.EQUAL));
                    Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);
                    var gameparam = new CreateNamespaceParams
                    {
                        Account = ownerdto,
                        Domain = Game.Name,
                        Duration = 1000,
                        Parent = null,
                    };
                    //Creates Game
                    var createGame = await blockchainPortal.CreateNamespaceAsync(gameparam);
                    this.dataAccessProximaX.SaveNamespace(createGame.gameName);
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
            if (Game == null || duration == 0)
            {
                _logger.LogError("At least one input is empty!");
                return null;
            }

            try
            {
                var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Game.Owner), FilterOperator.EQUAL));
                Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);
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
                var ownerresult = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Token.Owner), FilterOperator.EQUAL));
                Owner ownerdto = BsonToModel.BsonToOwnerDTO(ownerresult);
                var mosaicresult = repo.portal.ReadDocument("Mosaics", repo.portal.CreateFilter(new KeyValuePair<string, long>("AssetID", Convert.ToInt64(Token.TokenId)), FilterOperator.EQUAL));
                Mosaic mosaicDto = BsonToModel.BsonToTokenDTO(mosaicresult);
                //modify mosaic supply
                Console.Write("Amount to modify:  ");
                int am = Convert.ToInt32(Console.ReadLine());
                Console.Write("Modifying " + mosaicDto.MosaicID + " supply by " + am );

                var modifyparam = new ModifyMosaicSupplyParams
                {
                    Account = ownerdto,
                    MosaicID = mosaicDto.MosaicID,
                    Amount = am
                };

                var modifyMosaicT = await blockchainPortal.ModifyMosaicSupplyAsync(modifyparam);

                Transaction t = new Transaction
                {
                    Hash = modifyMosaicT.Hash,
                    Height = modifyMosaicT.Height,
                    Asset = modifyMosaicT.Asset,
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
            if (TokenId == 0)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            TokenDto tokenInfo = null;
            try
            {
                var result = repo.portal.ReadDocument("Mosaics", repo.portal.CreateFilter(new KeyValuePair<string, long>("AssetID", TokenId), FilterOperator.EQUAL));

                Mosaic mosaicDto = BsonToModel.BsonToTokenDTO(result);
                if(mosaicDto == null)
                {
                    Console.WriteLine("NO");
                }else
                {
                    Console.WriteLine("YES");
                }

                Console.WriteLine(mosaicDto.MosaicID);
                var mosaicinfo = await blockchainPortal.GetMosaicAsync(mosaicDto.MosaicID);
                tokenInfo = new TokenDto
                {
                    TokenId = mosaicinfo.AssetID,
                    Name = mosaicinfo.Name,
                    Quantity = mosaicinfo.Quantity,
                    Owner = mosaicinfo.Owner.UserID,
                };

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            

            return tokenInfo;
        }

        public async Task<GameDto> GetGameInfoAsync(long GameId)
        {
            if (GameId == 0)
            {
                return null;
                //log error
            }
            var result = repo.portal.ReadDocument("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, long>("NamespaceID", GameId), FilterOperator.EQUAL));
            Namespace gameDto = BsonToModel.BsonToGameDTO(result);
            Namespace namespaceInfo = await blockchainPortal.GetNamespaceInformationAsync(gameDto.Domain);
            GameDto gameInfo = null;

            gameInfo = new GameDto
            {
                GameId = namespaceInfo.NamespaceId,
                Name = namespaceInfo.Domain,
                Owner = namespaceInfo.Owner.UserID,
                Expiry = namespaceInfo.Expiry
            };

            return gameInfo;
        }

    }
}