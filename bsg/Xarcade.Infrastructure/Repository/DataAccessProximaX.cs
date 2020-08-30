using MongoDB.Bson;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xarcade.Domain.ProximaX;
using Xarcade.Domain.Authentication;
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Infrastructure.Abstract;

namespace Xarcade.Infrastructure.Repository
{
    public class DataAccessProximaX : IDataAccessProximaX
    {
        private static IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        public RepositoryPortal portal = new RepositoryPortal(config);
        private static ILogger _logger;

        public bool SaveOwner(Owner ownerDTO)
        {
            try
            {
                portal.CreateDocument("Owners", ownerDTO.ToBsonDocument());

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        public bool SaveUser(User userDTO)
        {
            try
            {
                portal.CreateDocument("Users", userDTO.ToBsonDocument());

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        public bool SaveNamespace(Namespace namespaceDTO)
        {
            try
            {
                portal.CreateDocument("Namespaces", namespaceDTO.ToBsonDocument());

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        public bool SaveMosaic(Mosaic mosaicDTO)
        {
            try
            {
                portal.CreateDocument("Mosaics", mosaicDTO.ToBsonDocument());

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        public bool SaveXar(Domain.ProximaX.Xarcade xarcadeDTO)
        {
            try
            {
                portal.CreateDocument("Xarcade", xarcadeDTO.ToBsonDocument());

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        public bool SaveTransaction(Transaction transactionDTO)
        {
            try
            {
                portal.CreateDocument("Transactions", transactionDTO.ToBsonDocument());

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        public bool SaveXarcadeUser(XarcadeUser xarUserDTO)
        {
            try
            {
                portal.CreateDocument("Authentication", xarUserDTO.ToBsonDocument());

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }

        public Owner LoadOwner(long userID)
        {
            var ownerBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, long>("UserID", userID), FilterOperator.EQUAL));
            if(ownerBson!=null) //if account exists
            {
                Owner ownerDTO = BsonToModel.BsonToOwnerDTO(ownerBson);
                return ownerDTO;
            }

            return null;
        }   

        public User LoadUser(long userID)
        {
            var userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, long>("UserID", userID), FilterOperator.EQUAL));

            if(userBson!=null) //if account exists
            {
                User userDTO = BsonToModel.BsonToUserDTO(userBson);
                return userDTO;
            }

            return null;
        }   

        public XarcadeUser LoadXarcadeUser(long userID)
        {
            var userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, long>("UserID", userID), FilterOperator.EQUAL));
            if(userBson!=null) //if account exists
            {
                XarcadeUser xarUserDTO = BsonToModel.BsonToXarcadeUserDTO(userBson);
                return xarUserDTO;
            }

            return null;
        }

        public Mosaic LoadMosaic(long tokenID)
        {
            var mosaicBson = portal.ReadDocument("Mosaics", portal.CreateFilter(new KeyValuePair<string, long>("AssetID", Convert.ToInt64(tokenID)), FilterOperator.EQUAL));
            if(mosaicBson != null) //if mosaic exists
            {
                Mosaic mosaicDTO = BsonToModel.BsonToTokenDTO(mosaicBson);
                return mosaicDTO;
            }

            return null;
        }

        public Namespace LoadNamespace(string gameName)
        {
            var namespaceBson = portal.ReadDocument("Namespaces", portal.CreateFilter(new KeyValuePair<string, string>("Domain", gameName), FilterOperator.EQUAL));
            if(namespaceBson != null) //if namespace exists
            {
                Namespace namespaceDTO = BsonToModel.BsonToGameDTO(namespaceBson);
                return namespaceDTO;
            }

            return null;
        }

        public Namespace LoadNamespace(long gameID)
        {
            var namespaceBson = portal.ReadDocument("Namespaces", portal.CreateFilter(new KeyValuePair<string, long>("NamespaceId", gameID), FilterOperator.EQUAL));
            if(namespaceBson != null) //if namespace exists
            {
                Namespace namespaceDTO = BsonToModel.BsonToGameDTO(namespaceBson);
                return namespaceDTO;
            }

            return null;
        }

        public List<BsonDocument> LoadMosaicList(Owner ownerDTO)
        {
            var mosaicListBson = portal.ReadCollection("Mosaics", portal.CreateFilter(new KeyValuePair<string, Owner>("Owner", ownerDTO), FilterOperator.EQUAL));
            if(mosaicListBson != null)//if the list exists
            {
                return mosaicListBson;
            }
            
            return null;
        }

        public Boolean CheckExistNamespace(string namespaceName)
        {
            var nsExist = portal.CheckExist("Namespaces", portal.CreateFilter(new KeyValuePair<string, string>("Domain", namespaceName), FilterOperator.EQUAL));
            if(nsExist == true)//if namespace already exists
            {
                return nsExist;
            }
            
            return false;
        }

        public Boolean CheckExistToken(string tokenName)
        {
            var tokenExist = portal.CheckExist("Mosaics", portal.CreateFilter(new KeyValuePair<string, string>("Name", tokenName), FilterOperator.EQUAL));
            if(tokenExist == true)//if namespace already exists
            {
                return tokenExist;
            }
            
            return false;
        }
    }
}