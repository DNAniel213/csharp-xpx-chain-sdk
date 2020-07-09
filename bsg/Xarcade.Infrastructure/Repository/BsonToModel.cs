using MongoDB.Bson;
using Xarcade.Domain.ProximaX;
using Xarcade.Domain.Authentication;


namespace Xarcade.Infrastructure.Repository
{
    /// <summary>
    /// Deserializes Bson into Xarcade.Domain Models
    /// </summary>
    public static class BsonToModel
    {
        public static Account BsonToAccountDTO(BsonDocument account)
        {
            var accountDTO = new Account();
            accountDTO.UserID        = account["userID"].AsInt64;
            accountDTO.WalletAddress = account["walletAddress"].AsString;
            accountDTO.PrivateKey    = account["privateKey"].AsString;
            accountDTO.PublicKey     = account["publicKey"].AsString;
            accountDTO.Created       = account["created"].ToUniversalTime();
            return accountDTO;
        }

        public static User BsonToUserDTO(BsonDocument user)
        {
            var userDTO = new User();
            userDTO.UserID        = user["userID"].AsInt64;
            userDTO.WalletAddress = user["walletAddress"].AsString;
            userDTO.PrivateKey    = user["privateKey"].AsString;
            userDTO.PublicKey     = user["publicKey"].AsString;
            userDTO.Created       = user["created"].ToUniversalTime();
            userDTO.OwnerID       = user["ownerID"].AsInt64;
            return userDTO;
        }

        public static Owner BsonToOwnerDTO(BsonDocument owner)
        {
            var ownerDTO = new Owner();
            ownerDTO.UserID        = owner["userID"].AsInt64;
            ownerDTO.WalletAddress = owner["walletAddress"].AsString;
            ownerDTO.PrivateKey    = owner["privateKey"].AsString;
            ownerDTO.PublicKey     = owner["publicKey"].AsString;
            ownerDTO.Created       = owner["created"].ToUniversalTime();
            return ownerDTO;
        }

        public static XarcadeUser BsonToXarcadeUserDTO(BsonDocument xarUser)
        {
            var xarUserDTO = new XarcadeUser();
            xarUserDTO.UserID   = xarUser["userID"].AsInt64;
            xarUserDTO.UserName = xarUser["userName"].AsString;
            xarUserDTO.Password = xarUser["password"].AsString;
            xarUserDTO.Email    = xarUser["email"].AsString;
            xarUserDTO.Created  = xarUser["created"].ToUniversalTime();

            return xarUserDTO;
        }


    }

}