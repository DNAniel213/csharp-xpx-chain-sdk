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

namespace Xarcade.Application.Xarcade
{
    public class TokenService
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

        public async Task<TokenTransactionDto> CreateXarTokenAsync(XarcadeTokenDto Token)
        {
            if (Token == null)
            {
                return null;
            }

            
            return null;
        }
        public async Task<TokenTransactionDto> CreateTokenAsync(TokenDto Token, string NamespaceName)
        {
            if (Token == null)
            {
                _logger.LogError("Invalid Input!!");
                return null;
            }
            TokenTransactionDto tokentrans = null;

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

                Console.Write("Token quantity:  ");
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
                    //AssetID         = Convert.ToInt64(Token.TokenId),
                    Account         = a,
                    //Namespace       = nsdto
                };
                Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicparam);
                Mosaic m = new Mosaic
                {
                    AssetID = Convert.ToInt64(Token.TokenId),
                    Name = Token.Name,
                    Quantity = Token.Quantity,
                    Owner = ownerdto,
                    Created = DateTime.Now,
                    MosaicID = createMosaicT.MosaicID,
                    Namespace = nsdto
                };
                repo.SaveMosaic(m);
                Asset tokenasset = new Asset
                {
                    AssetID = Convert.ToInt64(Token.TokenId),
                    Name = Token.Name,
                    Quantity = am,
                    Owner = ownerdto,
                    Created = DateTime.Now
                };

                //Links Mosaic To Namespace
                var linkparam = new LinkMosaicParams
                {
                    Account   =  mosaicparam.Account,
                    MosaicID  =  createMosaicT.MosaicID,
                    Namespace =  nsdto,
                };
                var link = await blockchainPortal.LinkMosaicAsync(linkparam);

                Transaction t = new Transaction
                {
                    Hash = link.Hash,
                    Height = link.Height,
                    Asset = tokenasset,
                    Created = DateTime.Now
                };
                repo.SaveTransaction(t);
                //Modifies Mosaic Supply
                //Console.Write("Token quantity:  ");
                //int am = Convert.ToInt32(Console.ReadLine());
                //var modifyparam = new ModifyMosaicSupplyParams
                //{
                //    Account  = mosaicparam.Account,
                //    MosaicID = createMosaicT.MosaicID,
                //    Amount   = am
                //};
                //Console.Write("Modifying " + createMosaicT.MosaicID + " supply by " + am + '\n');
                
                //Transaction modifyMosaicT = await blockchainPortal.ModifyMosaicSupplyAsync(modifyparam);
                
                //repo.SaveTransaction(modifyMosaicT);
                
                
                //Console.WriteLine(link.ToString());
                //tokentest = new TokenDto
                //{
                //    TokenId  = Token.TokenId,
                //    Name     = createNamespaceT.Domain,
                //    Quantity = Convert.ToUInt64(modifyparam.Amount),
                //    Owner    = Token.Owner
                //};

                //tokentrans = new TokenTransactionDto
                //{
                //    Status      = State.Confirmed,
                //    Hash        = link.Hash,
                //    Token       = tokentest,
                //    BlockNumber = 0,
                //    Created     = DateTime.Now
                //};

            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            
            return tokentrans;
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
                var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Game.Owner), FilterOperator.EQUAL));
                Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);

                //Creates Game
                var gameparam = new CreateNamespaceParams
                {
                    Account = await blockchainPortal.CreateAccountAsync(Game.Owner,ownerdto.PrivateKey),
                    Domain = Game.Name,
                };

                Namespace createGame = await blockchainPortal.CreateNamespaceAsync(gameparam);

                repo.SaveNamespace(createGame);

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
                _logger.LogError("Invalid Input!!");
                return null;
            }

            try
            {
                var PrivateKey = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Game.Owner), FilterOperator.EQUAL));
                Console.WriteLine("Extending " + Game.Name + " duration by " + duration);
                var param = new CreateNamespaceParams
                {
                    Account = await blockchainPortal.CreateAccountAsync(Game.Owner,PrivateKey.ToString()),
                    Domain = Game.Name,
                    Duration = duration,
                };

                var account = blockchainPortal.CreateAccountAsync(Game.Owner,PrivateKey.ToString());
                var namespaceInfo = await blockchainPortal.GetNamespaceInformationAsync(Game.Name);

                Namespace extendGame = await blockchainPortal.ExtendNamespaceDurationAsync(Game.Name,PrivateKey.ToString(),namespaceInfo,param);
                
                repo.SaveNamespace(extendGame);
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
                var PrivateKey = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, long>("UserID", Token.Owner), FilterOperator.EQUAL));
                var tokID = repo.portal.ReadDocument("Mosaics", repo.portal.CreateFilter(new KeyValuePair<string, long>("AssetID", Convert.ToInt64(Token.TokenId)), FilterOperator.EQUAL));
                Mosaic mosaicDto = BsonToModel.BsonToTokenDTO(tokID);
                //modify mosaic supply
                Console.Write("Amount to modify:  ");
                int am = Convert.ToInt32(Console.ReadLine());
                var modifyparam = new ModifyMosaicSupplyParams
                {
                    Account = await blockchainPortal.CreateAccountAsync(Token.Owner,PrivateKey.ToString()),
                    MosaicID = mosaicDto.MosaicID,//await blockchainPortal.GetMosaicAsync()
                    Amount = am
                };
                //Console.Write("Modifying " + createMosaicT.MosaicID + " supply by " + am );
                Transaction modifyMosaicT = await blockchainPortal.ModifyMosaicSupplyAsync(modifyparam);
                Console.WriteLine(modifyMosaicT);
                repo.SaveTransaction(modifyMosaicT);

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
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
                    TokenId = Convert.ToInt64(mosaicinfo.MosaicID),
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