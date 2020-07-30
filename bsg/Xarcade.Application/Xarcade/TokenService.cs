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

        public async Task<TokenTransactionDto> CreateTokenAsync(TokenDto Token)
        {
            if (Token == null)
            {
                return null;
                //log error
            }
            TokenDto token = null;
            TokenTransactionDto tokentrans = null;
            var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, string>("UserID", Token.Owner.ToString()), FilterOperator.EQUAL));
            Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);

            //Creates Mosaic
            var mosaicparam = new CreateMosaicParams();
            mosaicparam.Account = await blockchainPortal.CreateAccountAsync(Token.Owner,ownerdto.PrivateKey); 
            Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicparam); 
            
            Console.WriteLine(createMosaicT.ToString());
            repo.SaveMosaic(createMosaicT);

            //Creates Namespace
            Console.Write("Namespace name:  ");
            string name = Console.ReadLine();
            var namespaceparam = new CreateNamespaceParams();
            namespaceparam.Account = await blockchainPortal.CreateAccountAsync(Token.Owner,ownerdto.PrivateKey);
            namespaceparam.Domain = name;
            Namespace createNamespaceT = await blockchainPortal.CreateNamespaceAsync(namespaceparam);

            Console.WriteLine(createNamespaceT.ToString());
            repo.SaveNamespace(createNamespaceT);

            //Modifies Mosaic Supply
            Console.Write("Amount to modify:  ");
            int am = Convert.ToInt32(Console.ReadLine());
            var modifyparam = new ModifyMosaicSupplyParams
            {
                Account = mosaicparam.Account,
                MosaicID = createMosaicT.MosaicID,
                Amount = am
            };
            Console.Write("Modifying " + createMosaicT.MosaicID + " supply by " + am );
            Transaction modifyMosaicT = await blockchainPortal.ModifyMosaicSupplyAsync(modifyparam);
            
            repo.SaveTransaction(modifyMosaicT);
            
            //Links Mosaic To Namespace
            var linkparam = new LinkMosaicParams
            {
                Account   =  mosaicparam.Account,
                MosaicID     = createMosaicT.MosaicID,
                Namespace = createNamespaceT
            };

            var link = await blockchainPortal.LinkMosaicAsync(linkparam);

            token = new TokenDto
            {
                TokenId = Convert.ToUInt64(createMosaicT.MosaicID),
                Name = createNamespaceT.Domain,
                Quantity = Convert.ToUInt64(modifyparam.Amount),
                Owner = Token.Owner
            };

            tokentrans = new TokenTransactionDto
            {
                Hash = link.Hash,
                Token = token,
                //BlockNumber = ,
                Created = DateTime.Now
            };

            return tokentrans;
        }

        public async Task<TokenTransactionDto> CreateGameAsync(GameDto Game)
        {
            if (Game == null)
            {
                return null;
                //log error
            }
            var result = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, string>("UserID", Game.Owner.ToString()), FilterOperator.EQUAL));
            Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);

            //Creates Game
            Console.Write("Game name:  ");
            string name = Console.ReadLine();
            var gameparam = new CreateNamespaceParams();
            gameparam.Account = await blockchainPortal.CreateAccountAsync(Game.Owner,ownerdto.PrivateKey);
            gameparam.Domain = name;
            Namespace createGame = await blockchainPortal.CreateNamespaceAsync(gameparam);

            Console.WriteLine(createGame.ToString());
            repo.SaveNamespace(createGame);

            return null;  
        }

        public async Task<TokenTransactionDto> ExtendGameAsync(GameDto Game)
        {
            if (Game == null)
            {
                return null;
                //log error
            }
            //extend namespace
            var PrivateKey = repo.portal.ReadDocument("Owners", repo.portal.CreateFilter(new KeyValuePair<string, string>("UserID", Game.Owner.ToString()), FilterOperator.EQUAL));
            Console.Write("Number of Days:  ");
            ulong days = Convert.ToUInt32(Console.ReadLine());//take note of the remaining duration of the namespace | 365 days max
            ulong duration = days * 86400/15;
            
            var param = new CreateNamespaceParams();
            param.Account = await blockchainPortal.CreateAccountAsync(Game.Owner,PrivateKey.ToString());
            param.Domain = Game.Name;
            

            var account = blockchainPortal.CreateAccountAsync(Game.Owner,PrivateKey.ToString());
            var namespaceInfo = await blockchainPortal.GetNamespaceInformationAsync(Game.Name);
            Namespace extendGame = await blockchainPortal.ExtendNamespaceDurationAsync(Game.Name,PrivateKey.ToString(),namespaceInfo,duration,param);
            
            Console.WriteLine(extendGame.ToString());
            repo.SaveNamespace(extendGame);
            return null;
        }

        public async Task<TokenTransactionDto> ModifyTokenSupply(TokenDto Token)
        {
            if (Token == null)
            {
                return null;
                //log error
            }
            var mosaicparam = new CreateMosaicParams();
            Mosaic createMosaicT = await blockchainPortal.CreateMosaicAsync(mosaicparam); 

            //modify mosaic supply
            Console.Write("Amount to modify:  ");
            int am = Convert.ToInt32(Console.ReadLine());
            var modifyparam = new ModifyMosaicSupplyParams
            {
                Account = mosaicparam.Account,
                MosaicID = createMosaicT.MosaicID,
                Amount = am
            };
            Console.Write("Modifying " + createMosaicT.MosaicID + " supply by " + am );
            Transaction modifyMosaicT = await blockchainPortal.ModifyMosaicSupplyAsync(modifyparam);
            
            repo.SaveTransaction(modifyMosaicT);

            return null;
        }

        public async Task<TokenDto> GetTokenInfoAsync(long TokenId)
        {
            if (TokenId == 0)
            {
                return null;
                //log error
            }
            TokenDto tokenInfo = null;

            var result = repo.portal.ReadDocument("Mosaics", repo.portal.CreateFilter(new KeyValuePair<string, string>("MosaicID", TokenId.ToString()), FilterOperator.EQUAL));
            Mosaic mosaicDto = BsonToModel.BsonToTokenDTO(result);
            var mosaicinfo = await blockchainPortal.GetMosaicAsync(mosaicDto.MosaicID);

            tokenInfo = new TokenDto
            {
                TokenId = Convert.ToUInt64(mosaicinfo.MosaicID),
                Name = mosaicinfo.Name,
                Quantity = mosaicinfo.Quantity,
                Owner = mosaicinfo.Owner.UserID,
            };

            return tokenInfo;
        }

        public async Task<GameDto> GetGameInfoAsync(long GameId)
        {
            if (GameId == 0)
            {
                return null;
                //log error
            }
            var result = repo.portal.ReadDocument("Namespaces", repo.portal.CreateFilter(new KeyValuePair<string, string>("NamespaceID", GameId.ToString()), FilterOperator.EQUAL));
            Namespace gameDto = BsonToModel.BsonToGameDTO(result);
            Namespace namespaceInfo = await blockchainPortal.GetNamespaceInformationAsync(gameDto.Domain);
            GameDto gameInfo = null;

            gameInfo = new GameDto
            {
                GameId = namespaceInfo.NamespaceID,
                Name = namespaceInfo.Domain,
                Owner = namespaceInfo.Owner.UserID,
                Expiry = namespaceInfo.Expiry
            };

            return gameInfo;
        }

    }
}