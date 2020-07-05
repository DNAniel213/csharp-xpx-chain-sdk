using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using XarcadeModel = Xarcade.Domain.ProximaX;


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
            accountDTO.UserID        = account["userID"].AsInt64;
            accountDTO.WalletAddress = account["walletAddress"].AsString;
            accountDTO.PrivateKey    = account["privateKey"].AsString;
            accountDTO.PublicKey     = account["publicKey"].AsString;
            accountDTO.Created       = account["created"].ToUniversalTime();
            return accountDTO;
        }

        public static XarcadeModel.UserDTO BsonToUserDTO(BsonDocument user)
        {
            XarcadeModel.UserDTO userDTO = new XarcadeModel.UserDTO();
            userDTO.UserID        = user["userID"].AsInt64;
            userDTO.WalletAddress = user["walletAddress"].AsString;
            userDTO.PrivateKey    = user["privateKey"].AsString;
            userDTO.PublicKey     = user["publicKey"].AsString;
            userDTO.Created       = user["created"].ToUniversalTime();
            userDTO.OwnerID       = user["ownerID"].AsInt64;
            return userDTO;
        }

        public static XarcadeModel.OwnerDTO BsonToOwnerDTO(BsonDocument owner)
        {
            XarcadeModel.OwnerDTO ownerDTO = new XarcadeModel.OwnerDTO();
            ownerDTO.UserID        = owner["userID"].AsInt64;
            ownerDTO.WalletAddress = owner["walletAddress"].AsString;
            ownerDTO.PrivateKey    = owner["privateKey"].AsString;
            ownerDTO.PublicKey     = owner["publicKey"].AsString;
            ownerDTO.Created       = owner["created"].ToUniversalTime();
            return ownerDTO;
        }

        public static XarcadeModel.XarcadeUserDTO BsonToXarcadeUserDTO(BsonDocument xarUser)
        {
            XarcadeModel.XarcadeUserDTO xarUserDTO = new XarcadeModel.XarcadeUserDTO();
            xarUserDTO.userID   = xarUser["userID"].AsInt64;
            xarUserDTO.userName = xarUser["userName"].AsString;
            xarUserDTO.password = xarUser["password"].AsString;
            xarUserDTO.email    = xarUser["email"].AsString;
            xarUserDTO.created  = xarUser["created"].ToUniversalTime();

            return xarUserDTO;
        }


    }

}