using MongoDB.Bson;
using System;
using System.Collections.Generic;
using XarcadeModels = Xarcade.Domain.ProximaX;
using Xarcade.Domain.Authentication;


namespace Xarcade.Api.Prototype.Repository
{
    public class DataAccessProximaX
    {
        public RepositoryPortal portal = new RepositoryPortal();

        public bool SaveOwner(XarcadeModels.OwnerDTO ownerDTO)
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

        public bool SaveUser(XarcadeModels.UserDTO userDTO)
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

        public bool SaveNamespace(XarcadeModels.NamespaceDTO namespaceDTO)
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

        public bool SaveMosaic(XarcadeModels.MosaicDTO mosaicDTO)
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

        public bool SaveXar(XarcadeModels.XarcadeDTO xarcadeDTO)
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

        public bool SaveTransaction(XarcadeModels.TransactionDTO transactionDTO)
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

        public XarcadeModels.OwnerDTO LoadOwner(long userID)
        {
            BsonDocument ownerBson = portal.ReadDocument("Owners", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(ownerBson!=null) //if account exists
            {
                XarcadeModels.OwnerDTO ownerDTO = BsonToModel.BsonToOwnerDTO(ownerBson);
                return ownerDTO;
            }

            return null;
        }   

        public XarcadeModels.UserDTO LoadUser(long userID)
        {
            BsonDocument userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(userBson!=null) //if account exists
            {
                XarcadeModels.UserDTO userDTO = BsonToModel.BsonToUserDTO(userBson);
                return userDTO;
            }

            return null;
        }   

        public XarcadeUserDTO LoadXarcadeUser(long userID)
        {
            BsonDocument userBson = portal.ReadDocument("Users", portal.CreateFilter(new KeyValuePair<string, string>("userID", userID+""), FilterOperator.EQUAL));
            if(userBson!=null) //if account exists
            {
                XarcadeUserDTO xarUserDTO = BsonToModel.BsonToXarcadeUserDTO(userBson);
                return xarUserDTO;
            }

            return null;
        }   
    }
}