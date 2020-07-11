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

        public bool SaveOwner(Owner ownerDTO)
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

        public bool SaveUser(User userDTO)
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

        public bool SaveNamespace(Namespace namespaceDTO)
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

        public bool SaveMosaic(Mosaic mosaicDTO)
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

        public bool SaveXar(Domain.ProximaX.Xarcade xarcadeDTO)
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

        public bool SaveTransaction(Transaction transactionDTO)
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

        public bool SaveXarcadeUser(XarcadeUser xarUserDTO)
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

        public Owner LoadOwner(long userID)
        {
            var ownerBson = portal.ReadDocument("Owners", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(ownerBson!=null) //if account exists
            {
                Owner ownerDTO = BsonToModel.BsonToOwnerDTO(ownerBson);
                return ownerDTO;
            }

            return null;
        }   

        public User LoadUser(long userID)
        {
            var userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(userBson!=null) //if account exists
            {
                User userDTO = BsonToModel.BsonToUserDTO(userBson);
                return userDTO;
            }

            return null;
        }   

        public XarcadeUser LoadXarcadeUser(long userID)
        {
            var userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(userBson!=null) //if account exists
            {
                XarcadeUser xarUserDTO = BsonToModel.BsonToXarcadeUserDTO(userBson);
                return xarUserDTO;
            }

            return null;
        }   
    }
}