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

        public DataAccessProximaX()
        {

        }

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
                portal.CreateDocument("XarcadeUsers", xarUserDTO.ToBsonDocument());

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
            return true;
        }


        public Owner LoadOwner(string userID)
        {
            var ownerBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, string>("UserID", userID), FilterOperator.EQUAL));
            
            if(ownerBson!=null) //if account exists
            {

                Owner ownerDTO = BsonToModel.BsonToOwnerDTO(ownerBson);
                return ownerDTO;
            }

            return null;
        }   

        public User LoadUser(string userID)
        {
            var userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, string>("UserID", userID), FilterOperator.EQUAL));

            if(userBson!=null) //if account exists
            {
                User userDTO = BsonToModel.BsonToUserDTO(userBson);
                return userDTO;
            }

            return null;
        }   

        public XarcadeUser LoadXarcadeUser(XarcadeUserSearchKey searchKey)
        {
            string key = null;
            string value = null;

            if(!string.IsNullOrEmpty(searchKey.UserID))
            {
                key = "UserID";
                value = searchKey.UserID;
            }
            else if(!string.IsNullOrEmpty(searchKey.Email))
            {
                key = "Email";
                value = searchKey.Email;
            }
            else if(!string.IsNullOrEmpty(searchKey.Username))
            {
                key = "Username";
                value = searchKey.Username;

            }
            else if(!string.IsNullOrEmpty(searchKey.VerificationToken))
            {
                key = "VerificationToken";
                value = searchKey.VerificationToken;
            }

            var userBson = portal.ReadDocument("XarcadeUsers", portal.CreateFilter(new KeyValuePair<string, string>(key, value), FilterOperator.EQUAL));

            if(userBson!=null) //if account exists
            {
                XarcadeUser xarUserDTO = BsonToModel.BsonToXarcadeUserDTO(userBson);
                return xarUserDTO;
            }

            return null;
        }

        public Mosaic LoadMosaic(string tokenID)
        {
            var mosaicBson = portal.ReadDocument("Mosaics", portal.CreateFilter(new KeyValuePair<string, string>("AssetID", tokenID), FilterOperator.EQUAL));

            if(mosaicBson != null) //if mosaic exists
            {
                Mosaic mosaicDTO = BsonToModel.BsonToTokenDTO(mosaicBson);

                return mosaicDTO;
            }
            else
            {
                mosaicBson = portal.ReadDocument("Mosaics", portal.CreateFilter(new KeyValuePair<string, string>("MosaicID", tokenID), FilterOperator.EQUAL));
                if(mosaicBson != null) //if mosaic exists with mosaicID
                {

                    Mosaic mosaicDTO = BsonToModel.BsonToTokenDTO(mosaicBson);

                    return mosaicDTO;
                }
            }

            return null;
        }


        public Namespace LoadNamespace(string gameID)
        {
            var namespaceBson = portal.ReadDocument("Namespaces", portal.CreateFilter(new KeyValuePair<string, string>("Domain", gameID), FilterOperator.EQUAL));

            if(namespaceBson != null) //if namespace exists
            {

                Namespace namespaceDTO = BsonToModel.BsonToGameDTO(namespaceBson);

                return namespaceDTO;
            }
            else 
            {
                namespaceBson = portal.ReadDocument("Namespaces", portal.CreateFilter(new KeyValuePair<string, string>("NamespaceId", gameID), FilterOperator.EQUAL));
                if(namespaceBson != null)
                {

                    Namespace namespaceDTO = BsonToModel.BsonToGameDTO(namespaceBson);
                    return namespaceDTO;
                }
            }

            return null;
        }

        public List<Mosaic> LoadMosaicList(string ownerId)
        {
            var mosaicListBson = portal.ReadCollection("Mosaics", portal.CreateFilter(new KeyValuePair<string, string>("OwnerId", ownerId), FilterOperator.EQUAL));
            if(mosaicListBson != null)//if the list exists
            {
                var mosaicList = new List<Mosaic>();
                foreach(BsonDocument doc in mosaicListBson)
                {
                    Mosaic mosaic = BsonToModel.BsonToTokenDTO(doc);
                    mosaicList.Add(mosaic);
                }
                return mosaicList;
            }
            
            return null;
        }



        public bool UpdateXarcadeUser(XarcadeUser user)
        {
            var result = portal.UpdateDocument("XarcadeUsers", portal.CreateFilter(new KeyValuePair<string, string>("UserID", user.UserID), FilterOperator.EQUAL), user.ToBsonDocument());
            return true;
        }


        public bool UpdateNamespaceDuration(string gameName, DateTime expiry)
        {
            var result = portal.UpdateDocumentField("Namespaces", portal.CreateFilter(new KeyValuePair<string, string>("Domain", gameName), FilterOperator.EQUAL), "Expiry", expiry);

            return true;
        }

        public bool UpdateMosaicQuantity(string assetId, long newQuantity)
        {
            var result = portal.UpdateDocumentField("Mosaics", portal.CreateFilter(new KeyValuePair<string, string>("AssetID", assetId), FilterOperator.EQUAL), "Quantity", newQuantity);

            return true;
        }

        public bool UpdateMosaicLink(string assetId, Namespace newNamespace)
        {
            var result = portal.UpdateDocumentField("Mosaics", portal.CreateFilter(new KeyValuePair<string, string>("AssetID", assetId), FilterOperator.EQUAL), "Namespace", newNamespace);

            return true;
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