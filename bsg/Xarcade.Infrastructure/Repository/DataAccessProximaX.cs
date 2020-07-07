using MongoDB.Bson;
using System;
using System.Collections.Generic;
using Xarcade.Domain.ProximaX;
using Xarcade.Domain.Authentication;


namespace Xarcade.Infrastructure.Repository
{
    public class DataAccessProximaX
    {
        public RepositoryPortal portal = new RepositoryPortal();

        public bool SaveOwner(OwnerDTO ownerDTO)
        {
            try
            {
                portal.CreateDocument("Owners", ownerDTO.ToBsonDocument());

            }catch(Exception)
            {
                return false;
                //TODO log e
            }
            return true;
        }

        public bool SaveUser(UserDTO userDTO)
        {
            try
            {
                portal.CreateDocument("Users", userDTO.ToBsonDocument());

            }catch(Exception)
            {
                //TODO log e
                return false;
            }
            return true;
        }

        public bool SaveNamespace(NamespaceDTO namespaceDTO)
        {
            try
            {
                portal.CreateDocument("Namespaces", namespaceDTO.ToBsonDocument());

            }catch(Exception)
            {
                //TODO log e
                return false;
            }
            return true;
        }

        public bool SaveMosaic(MosaicDTO mosaicDTO)
        {
            try
            {
                portal.CreateDocument("Mosaics", mosaicDTO.ToBsonDocument());

            }catch(Exception)
            {
                //TODO log e
                return false;
            }
            return true;
        }

        public bool SaveXar(XarcadeDTO xarcadeDTO)
        {
            try
            {
                portal.CreateDocument("Xarcade", xarcadeDTO.ToBsonDocument());

            }catch(Exception)
            {
                //TODO log e
                return false;
            }
            return true;
        }

        public bool SaveTransaction(TransactionDTO transactionDTO)
        {
            try
            {
                portal.CreateDocument("Transactions", transactionDTO.ToBsonDocument());

            }catch(Exception)
            {
                //TODO log e
                return false;
            }
            return true;
        }

        public bool SaveXarcadeUser(XarcadeUserDTO xarUserDTO)
        {
            try
            {
                portal.CreateDocument("Authentication", xarUserDTO.ToBsonDocument());

            }catch(Exception)
            {
                //TODO log e
                
                return false;
            }
            return true;
        }

        public OwnerDTO LoadOwner(long userID)
        {
            var ownerBson = portal.ReadDocument("Owners", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(ownerBson!=null) //if account exists
            {
                OwnerDTO ownerDTO = BsonToModel.BsonToOwnerDTO(ownerBson);
                return ownerDTO;
            }

            return null;
        }   

        public UserDTO LoadUser(long userID)
        {
            var userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(userBson!=null) //if account exists
            {
                UserDTO userDTO = BsonToModel.BsonToUserDTO(userBson);
                return userDTO;
            }

            return null;
        }   

        public XarcadeUserDTO LoadXarcadeUser(long userID)
        {
            var userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(userBson!=null) //if account exists
            {
                XarcadeUserDTO xarUserDTO = BsonToModel.BsonToXarcadeUserDTO(userBson);
                return xarUserDTO;
            }

            return null;
        }   
    }
}