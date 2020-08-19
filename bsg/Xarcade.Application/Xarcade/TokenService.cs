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
            if(Token == null || Game == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            
            TokenTransactionDto ttd = null;

            try
            {
                var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Token.Owner), FilterOperator.EQUAL)); 
                Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);

                var mosaicresult = repo.portal.ReadDocument("Mosaics", repo.portal.CreateFilter(new KeyValuePair<string, long>("AssetID", Convert.ToInt64(Token.TokenId)), FilterOperator.EQUAL));
                Mosaic m = BsonToModel.BsonToTokenDTO(mosaicresult);

                var namespaceresult = repo.portal.ReadDocument("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, string>("Domain", Game.Name), FilterOperator.EQUAL));
                Namespace n = BsonToModel.BsonToGameDTO(namespaceresult);

                Account a = new Account
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
                    Account   =  a,
                    MosaicID  =  m.MosaicID,
                    Namespace =  n,
                };
                var link = await blockchainPortal.LinkMosaicAsync(linkparam);
                TokenDto tdto = new TokenDto
                {
                    TokenId     = Convert.ToUInt64(link.Asset.AssetID),
                    Name        = link.Asset.Name,
                    Quantity    = link.Asset.Quantity,
                    Owner       = link.Asset.Owner.UserID
                };
                ttd = new TokenTransactionDto
                {
                    Status      = State.Confirmed,
                    Hash        = link.Hash,
                    Token       = tdto,
                    BlockNumber = link.Height,
                    Created     = link.Created
                };
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return ttd;
        }
        public async Task<List<TokenDto>> GetTokenListAsync(long userId, long gameId)
        {
            if (userId < 0 || gameId < 0)
            {
                Console.WriteLine("Invalid Input!");
                return null;
            }
            
            List<TokenDto> ttdlist = new List<TokenDto>();
            try
            {
                var ownerresult = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", userId), FilterOperator.EQUAL));
                Owner ownerdto = BsonToModel.BsonToOwnerDTO(ownerresult);
                var tokenlist = repo.portal.ReadCollection("Mosaics", repo.portal.CreateFilter(new KeyValuePair<string, Owner>("Owner", ownerdto), FilterOperator.EQUAL));
                var nsresult = repo.portal.ReadDocument("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, long>("NamespaceId", gameId), FilterOperator.EQUAL));
                foreach (var token in tokenlist)
                {
                    Mosaic t = BsonToModel.BsonToTokenDTO(token);
                    TokenDto ttd = new TokenDto
                    {
                        TokenId     = Convert.ToUInt64(t.AssetID),
                        Name        = t.Name,
                        Quantity    = t.Quantity,
                        Owner       = t.Owner.UserID
                    };
                    Console.WriteLine(ttd);
                    ttdlist.Add(ttd);
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return ttdlist;
        }
        //create xarcade token
        public async Task<TokenTransactionDto> CreateXarTokenAsync(XarcadeTokenDto xar)
        {
            if (xar == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            TokenTransactionDto ttd = null;
            try
            {
                var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", xar.Owner), FilterOperator.EQUAL)); 
                Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);

                Console.Write("Token quantity:");
                ulong quan = Convert.ToUInt64(Console.ReadLine());

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
                    Account = a,
                };
                Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicparam);

                Mosaic m = new Mosaic
                {
                    AssetID = Convert.ToInt64(xar.TokenId),
                    Name = "XarcadeToken",
                    Quantity = quan,
                    Owner = createMosaicT.Owner,
                    Created = createMosaicT.Created,
                    MosaicID = createMosaicT.MosaicID,
                    Namespace = null
                };
                this.dataAccessProximaX.SaveMosaic(m);

                var modsupply = new ModifyMosaicSupplyParams
                {
                    Account   =  a,
                    MosaicID  =  createMosaicT.MosaicID,
                    Amount    =  Convert.ToInt32(quan),
                };
                var supplied = await blockchainPortal.ModifyMosaicSupplyAsync(modsupply);

                TokenDto t = new TokenDto
                {
                    TokenId = xar.TokenId,
                    Name = m.Name,
                    Quantity = quan,
                    Owner = a.UserID
                };

                ttd = new TokenTransactionDto
                {
                    Status = State.Unconfirmed,
                    Hash = supplied.Hash,
                    Token = t,
                    BlockNumber = supplied.Height,
                    Created = supplied.Created
                };
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
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
                    Account = a,
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
                //Adds supply to mosaic i hope
                var modsupply = new ModifyMosaicSupplyParams
                {
                    Account   =  a,
                    MosaicID  =  createMosaicT.MosaicID,
                    Amount    =  Convert.ToInt32(am),
                };
                var supplied = await blockchainPortal.ModifyMosaicSupplyAsync(modsupply);

                Transaction t = new Transaction
                {
                    Hash = supplied.Hash,
                    Height = supplied.Height,
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
                    Status = State.Unconfirmed,
                    Hash = t.Hash,
                    Token = td,
                    BlockNumber = t.Height, 
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
                        Account     = ownerdto,
                        Domain      = Game.Name,
                        Duration    = 1000,
                        Parent      = null,
                    };
                    //Creates Game
                    var createGame = await blockchainPortal.CreateNamespaceAsync(gameparam);
                    Namespace ns = new Namespace
                    {
                        NamespaceId     = createGame.gameName.NamespaceId,
                        Domain          = createGame.gameName.Domain,
                        LayerOne        = createGame.gameName.LayerOne,
                        LayerTwo        = createGame.gameName.LayerTwo,
                        Owner           = createGame.gameName.Owner,
                        Expiry          = createGame.gameName.Expiry,
                        Created         = createGame.gameName.Created
                    };
                    this.dataAccessProximaX.SaveNamespace(ns);
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
                    TokenId     = Convert.ToUInt64(mosaicinfo.AssetID),
                    Name        = mosaicinfo.Name,
                    Quantity    = mosaicinfo.Quantity,
                    Owner       = mosaicinfo.Owner.UserID,
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
            if (GameId < 0)
            {
                return null;
                //log error
            }
            
            GameDto gameInfo = null;
            try
            {
                var result = repo.portal.ReadDocument("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, long>("NamespaceId", GameId), FilterOperator.EQUAL));
                Namespace gameDto = BsonToModel.BsonToGameDTO(result);
                //Namespace namespaceInfo = await blockchainPortal.GetNamespaceInformationAsync(gameDto.Domain);

                gameInfo = new GameDto
                {
                    GameId  = gameDto.NamespaceId,
                    Name    = gameDto.Domain,
                    Owner   = gameDto.Owner.UserID,
                    Expiry  = gameDto.Expiry
                };
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return gameInfo;
        }

    }
}