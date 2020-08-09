using MongoDB.Bson;
using Xarcade.Domain.ProximaX;
using Xarcade.Domain.Authentication;

using System;

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
            ownerDTO.UserID        = owner["UserID"].AsInt64;
            ownerDTO.WalletAddress = owner["WalletAddress"].AsString;
            ownerDTO.PrivateKey    = owner["PrivateKey"].AsString;
            ownerDTO.PublicKey     = owner["PublicKey"].AsString;
            ownerDTO.Created       = owner["Created"].ToUniversalTime();
            return ownerDTO;
        }

        public static XarcadeUser BsonToXarcadeUserDTO(BsonDocument xarUser)
        {
            var xarUserDTO = new XarcadeUser();
            xarUserDTO.UserID   = xarUser["UserID"].AsInt64;
            xarUserDTO.UserName = xarUser["UserName"].AsString;
            xarUserDTO.Password = xarUser["Password"].AsString;
            xarUserDTO.Email    = xarUser["Email"].AsString;
            xarUserDTO.Created  = xarUser["Created"].ToUniversalTime();

            return xarUserDTO;
        }

        public static Namespace BsonToGameDTO(BsonDocument game)
        {
            var gameDTO = new Namespace();
            gameDTO.NamespaceId = game["NamespaceID"].AsInt64;
            gameDTO.Domain      = game["Domain"].AsString;
            try
            {
                gameDTO.LayerOne    = game["LayerOne"].AsString;
                gameDTO.LayerTwo    = game["LayerTwo"].AsString;
            }catch(Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
            gameDTO.Owner       = BsonToOwnerDTO(game["Owner"].AsBsonDocument);
            gameDTO.Expiry      = game["Expiry"].ToUniversalTime();
            gameDTO.Created     = game["Created"].ToUniversalTime();

            return gameDTO;
        }
 
        public static Mosaic BsonToTokenDTO(BsonDocument token)
        {
            //Console.WriteLine(token["Namespace"].AsBsonDocument);
            Console.WriteLine(token["Owner"].AsBsonDocument);
            var mosaicDTO = new Mosaic();
            mosaicDTO.MosaicID  = (ulong)token["MosaicID"].AsInt64;
            //mosaicDTO.Namespace = BsonToGameDTO(token["Namespace"]);//no out
            mosaicDTO.AssetID   = token["AssetID"].AsInt64;//no out
            mosaicDTO.Name      = token["Name"].AsString;//no out
            mosaicDTO.Quantity  = (ulong)token["Quantity"].AsInt64;//no out
            mosaicDTO.Owner     = BsonToOwnerDTO(token["Owner"].AsBsonDocument);//no out
            mosaicDTO.Created   = token["Created"].ToUniversalTime();//no out
            return mosaicDTO;
        }

    }

}