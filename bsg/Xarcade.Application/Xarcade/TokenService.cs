using System;
using System.Threading.Tasks;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Infrastructure.ProximaX;
using Xarcade.Infrastructure.ProximaX.Params;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Application.Xarcade.Models.Account;
using Xarcade.Domain.ProximaX;
using System.Collections.Generic;
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
            var result = repo.portal.ReadDocument("Owner", repo.portal.CreateFilter(new KeyValuePair<string, string>("userId", Token.Owner.ToString()), FilterOperator.EQUAL));
            Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);

            //Creates Mosaic
            var mosaicparam = new CreateMosaicParams();
            mosaicparam.Account = blockchainPortal.CreateAccountAsync(Token.Owner,ownerdto.PrivateKey).GetAwaiter().GetResult(); 
            Mosaic createMosaicT = blockchainPortal.CreateMosaicAsync(mosaicparam).GetAwaiter().GetResult(); 
            
            Console.WriteLine(createMosaicT.ToString());
            repo.SaveMosaic(createMosaicT);

            //Creates Namespace
            Console.Write("Namespace name:  ");
            string name = Console.ReadLine();
            var namespaceparam = new CreateNamespaceParams();
            namespaceparam.Account = blockchainPortal.CreateAccountAsync(Token.Owner,ownerdto.PrivateKey).GetAwaiter().GetResult();
            namespaceparam.Domain = name;
            Namespace createNamespaceT = blockchainPortal.CreateNamespaceAsync(namespaceparam).GetAwaiter().GetResult();

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
            Transaction modifyMosaicT = blockchainPortal.ModifyMosaicSupplyAsync(modifyparam).GetAwaiter().GetResult();
            
            repo.SaveTransaction(modifyMosaicT);
            
            //Links Mosaic To Namespace
            var linkparam = new LinkMosaicParams
            {
                Account   =  mosaicparam.Account,
                MosaicID     = createMosaicT.MosaicID,
                Namespace = createNamespaceT
            };

            var link = blockchainPortal.LinkMosaicAsync(linkparam).GetAwaiter().GetResult();

            token = new TokenDto
            {
                TokenId = Convert.ToInt64(createMosaicT.MosaicID),
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
            var result = repo.portal.ReadDocument("Owner", repo.portal.CreateFilter(new KeyValuePair<string, string>("userId", Game.Owner.ToString()), FilterOperator.EQUAL));
            Owner ownerdto = BsonToModel.BsonToOwnerDTO(result);

            //Creates Game
            Console.Write("Game name:  ");
            string name = Console.ReadLine();
            var gameparam = new CreateNamespaceParams();
            gameparam.Account = blockchainPortal.CreateAccountAsync(Game.Owner,ownerdto.PrivateKey).GetAwaiter().GetResult();
            gameparam.Domain = name;
            Namespace createGame = blockchainPortal.CreateNamespaceAsync(gameparam).GetAwaiter().GetResult();

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
            var PrivateKey = repo.portal.ReadDocument("Owner", repo.portal.CreateFilter(new KeyValuePair<string, string>("userId", Game.Owner.ToString()), FilterOperator.EQUAL));
            Console.Write("Number of Days:  ");
            ulong days = Convert.ToUInt32(Console.ReadLine());//take note of the remaining duration of the namespace | 365 days max
            ulong duration = days * 86400/15;
            
            var param = new CreateNamespaceParams();
            param.Account = blockchainPortal.CreateAccountAsync(Game.Owner,PrivateKey.ToString()).GetAwaiter().GetResult();
            param.Domain = Game.Name;
            

            var account = blockchainPortal.CreateAccountAsync(Game.Owner,PrivateKey.ToString());
            var namespaceInfo = blockchainPortal.GetNamespaceInformationAsync(Game.Name).GetAwaiter().GetResult();
            Namespace extendGame = blockchainPortal.ExtendNamespaceDurationAsync(Game.Name,PrivateKey.ToString(),namespaceInfo,duration,param).GetAwaiter().GetResult();
            
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
            Mosaic createMosaicT = blockchainPortal.CreateMosaicAsync(mosaicparam).GetAwaiter().GetResult(); 

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
            Transaction modifyMosaicT = blockchainPortal.ModifyMosaicSupplyAsync(modifyparam).GetAwaiter().GetResult();
            
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

            var result = repo.portal.ReadDocument("Mosaic", repo.portal.CreateFilter(new KeyValuePair<string, string>("MosaicId", TokenId.ToString()), FilterOperator.EQUAL));
            Mosaic mosaicDto = BsonToModel.BsonToTokenDTO(result);

            tokenInfo = new TokenDto
            {
                TokenId = Convert.ToInt64(mosaicDto.MosaicID),
                Name = mosaicDto.Namespace.Domain,
                //Quantity = ,
                //Owner = ,
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
            var result = repo.portal.ReadDocument("Namespace", repo.portal.CreateFilter(new KeyValuePair<string, string>("NamespaceId", GameId.ToString()), FilterOperator.EQUAL));
            Namespace gameDto = BsonToModel.BsonToGameDTO(result);
            GameDto gameInfo = null;

            gameInfo = new GameDto
            {
                GameId = gameDto.NamespaceId,
                Name = gameDto.Domain,
                Owner = gameDto.Owner.UserID,
                Expiry = gameDto.Expiry
            };

            return gameInfo;
        }

    }
}