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
            accountDTO.UserID        = account["UserID"].AsInt64;
            accountDTO.WalletAddress = account["WalletAddress"].AsString;
            accountDTO.PrivateKey    = account["PrivateKey"].AsString;
            accountDTO.PublicKey     = account["PublicKey"].AsString;
            accountDTO.Created       = account["Created"].ToUniversalTime();
            return accountDTO;
        }

        public static User BsonToUserDTO(BsonDocument user)
        {
            var userDTO = new User();
            userDTO.UserID        = user["UserID"].AsInt64;
            userDTO.WalletAddress = user["WalletAddress"].AsString;
            userDTO.PrivateKey    = user["PrivateKey"].AsString;
            userDTO.PublicKey     = user["PublicKey"].AsString;
            userDTO.Created       = user["Created"].ToUniversalTime();
            userDTO.OwnerID       = user["OwnerID"].AsInt64;
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
            gameDTO.NamespaceID = game["NamespaceID"].AsInt64;
            gameDTO.Domain      = game["Domain"].AsString;
            try
            {
                gameDTO.LayerOne    = game["LayerOne"].AsString;
                gameDTO.LayerTwo    = game["LayerTwo"].AsString;
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
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