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
            accountDTO.UserID        = account["UserID"].AsString;
            accountDTO.WalletAddress = account["WalletAddress"].AsString;
            accountDTO.PrivateKey    = account["PrivateKey"].AsString;
            accountDTO.PublicKey     = account["PublicKey"].AsString;
            accountDTO.Created       = account["Created"].ToUniversalTime();
            return accountDTO;
        }

        public static User BsonToUserDTO(BsonDocument user)
        {
            var userDTO = new User();
            userDTO.UserID        = user["UserID"].AsString;
            userDTO.WalletAddress = user["WalletAddress"].AsString;
            userDTO.PrivateKey    = user["PrivateKey"].AsString;
            userDTO.PublicKey     = user["PublicKey"].AsString;
            userDTO.Created       = user["Created"].ToUniversalTime();
            userDTO.OwnerID       = user["OwnerID"].AsString;
            return userDTO;
        }

        public static Owner BsonToOwnerDTO(BsonDocument owner)
        {
            var ownerDTO = new Owner();
            ownerDTO.UserID        = owner["UserID"].AsString;
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
            gameDTO.NamespaceId = game["NamespaceID"].AsString;
            gameDTO.Domain      = game["Domain"].AsString;

            try
            {

                gameDTO.LayerOne    = game["LayerOne"].AsString;
                gameDTO.LayerTwo    = game["LayerTwo"].AsString;
            }catch(Exception e)
            {
            }
            gameDTO.Owner       = BsonToOwnerDTO(game["Owner"].AsBsonDocument);
            gameDTO.Expiry      = game["Expiry"].ToUniversalTime();
            gameDTO.Created     = game["Created"].ToUniversalTime();

            return gameDTO;
        }
 
        public static Mosaic BsonToTokenDTO(BsonDocument token)
        {
            var mosaicDTO = new Mosaic();
            mosaicDTO.MosaicID  = token["MosaicID"].AsString;
            try
            {
                mosaicDTO.Namespace = BsonToGameDTO(token["Namespace"].AsBsonDocument);
            }catch(Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
            mosaicDTO.AssetID   = token["AssetID"].AsString;
            mosaicDTO.Name      = token["Name"].AsString;
            mosaicDTO.Quantity  = (ulong)token["Quantity"].AsInt64;
            mosaicDTO.Owner     = BsonToOwnerDTO(token["Owner"].AsBsonDocument);
            mosaicDTO.Created   = token["Created"].ToUniversalTime();
            return mosaicDTO;
        }

    }

}