using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using XarcadeModel = Xarcade.Domain.Models;


namespace Xarcade.Api.Prototype.Repository
{
    /// <summary>
    /// Deserializes Bson into Xarcade.Domain Models
    /// </summary>
    public static class BsonToModel
    {
        public static XarcadeModel.AccountDTO BsonToAccountDTO(BsonDocument account)
        {
            XarcadeModel.AccountDTO accountDTO = new XarcadeModel.AccountDTO();
            accountDTO.userID = account["userID"].AsInt64;
            accountDTO.walletAddress = account["walletAddress"].AsString;
            accountDTO.privateKey = account["privateKey"].AsString;
            accountDTO.publicKey = account["publicKey"].AsString;
            accountDTO.created = account["created"].ToUniversalTime();
            return accountDTO;
        }

        public static XarcadeModel.UserDTO BsonToUserDTO(BsonDocument user)
        {
            XarcadeModel.UserDTO userDTO = new XarcadeModel.UserDTO();
            userDTO.userID = user["userID"].AsInt64;
            userDTO.walletAddress = user["walletAddress"].AsString;
            userDTO.privateKey = user["privateKey"].AsString;
            userDTO.publicKey = user["publicKey"].AsString;
            userDTO.created = user["created"].ToUniversalTime();
            userDTO.ownerID = user["ownerID"].AsInt64;
            return userDTO;
        }

        public static XarcadeModel.OwnerDTO BsonToOwnerDTO(BsonDocument owner)
        {
            XarcadeModel.OwnerDTO ownerDTO = new XarcadeModel.OwnerDTO();
            ownerDTO.userID = owner["userID"].AsInt64;
            ownerDTO.walletAddress = owner["walletAddress"].AsString;
            ownerDTO.privateKey = owner["privateKey"].AsString;
            ownerDTO.publicKey = owner["publicKey"].AsString;
            ownerDTO.created = owner["created"].ToUniversalTime();
            return ownerDTO;
        }

        public static XarcadeModel.XarcadeUserDTO BsonToXarcadeUserDTO(BsonDocument xarUser)
        {
            XarcadeModel.XarcadeUserDTO xarUserDTO = new XarcadeModel.XarcadeUserDTO();
            xarUserDTO.userID = xarUser["userID"].AsInt64;
            xarUserDTO.userName = xarUser["userName"].AsString;
            xarUserDTO.password = xarUser["password"].AsString;
            xarUserDTO.email = xarUser["email"].AsString;
            xarUserDTO.created = xarUser["created"].ToUniversalTime();

            return xarUserDTO;
        }


    }

}