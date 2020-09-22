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

            xarUserDTO.UserID   = xarUser["UserID"].AsString;
            xarUserDTO.Username = xarUser["Username"].AsString;
            xarUserDTO.PasswordHash = xarUser["Password"].AsString;
            xarUserDTO.AcceptTerms    = xarUser["Email"].AsBoolean;
            xarUserDTO.Created  = xarUser["Created"].ToUniversalTime();
            xarUserDTO.Modified  = xarUser["Created"].ToUniversalTime();
            xarUserDTO.Role = (Role)xarUser["Role"].ToInt32();

            xarUserDTO.UserDetails = BsonToXarcadeUserDetails(xarUser["UserDetails"].AsBsonDocument);
            xarUserDTO.Verification = BsonToVerificationDetails(xarUser["VerificationDetails"].AsBsonDocument);
            xarUserDTO.PasswordReset = BsonToPasswordResetDetails(xarUser["PasswordReset"].AsBsonDocument);

            foreach (BsonDocument bsonRefreshToken in xarUser["RefreshTokens"].AsBsonArray)
            {
                var refreshToken = BsonToRefreshToken(bsonRefreshToken);
                xarUserDTO.RefreshTokens.Add(refreshToken);
            }
            return xarUserDTO;
        }
        public static PasswordResetDetails BsonToPasswordResetDetails(BsonDocument xarPasswordResetBson)
        {
            var xarPasswordResetDetails = new PasswordResetDetails();

            xarPasswordResetDetails.ResetToken = xarPasswordResetBson["ResetToken"].AsString;
            xarPasswordResetDetails.PasswordReset = xarPasswordResetBson["PasswordReset"].ToUniversalTime();
            xarPasswordResetDetails.ResetTokenExpiry = xarPasswordResetBson["ResetTokenExpiry"].ToUniversalTime();

            return xarPasswordResetDetails;
        }
        public static RefreshToken BsonToRefreshToken(BsonDocument xarRefreshTokenBson)
        {
            var xarRefreshToken = new RefreshToken();

            xarRefreshToken.TokenId     = xarRefreshTokenBson["TokenId"].AsInt32;
            xarRefreshToken.XarcadeUser = BsonToXarcadeUserDTO(xarRefreshTokenBson["XarcadeUser"].AsBsonDocument);
            xarRefreshToken.Token       = xarRefreshTokenBson["Token"].AsString;
            xarRefreshToken.Expiry      = xarRefreshTokenBson["Expiry"].ToUniversalTime();
            xarRefreshToken.IsExpired   = xarRefreshTokenBson["IsExpired"].AsBoolean;
            xarRefreshToken.Created     = xarRefreshTokenBson["CreatorIp"].ToUniversalTime();
            xarRefreshToken.CreatorIp   = xarRefreshTokenBson["CreatorIp"].AsString;
            xarRefreshToken.Revoked     = xarRefreshTokenBson["Revoked"].ToUniversalTime();
            xarRefreshToken.RevokerIp   = xarRefreshTokenBson["RevokerIp"].AsString;
            xarRefreshToken.ReplacementToken = xarRefreshTokenBson["ReplacementToken"].AsString;
            xarRefreshToken.IsActive    = xarRefreshTokenBson["IsActive"].AsBoolean;

            return xarRefreshToken;
        }

        public static XarcadeUserDetails BsonToXarcadeUserDetails(BsonDocument xarUserDetailsBson)
        {
            var xarUserDetails = new XarcadeUserDetails();

            xarUserDetails.FirstName = xarUserDetailsBson["FirstName"].AsString;
            xarUserDetails.LastName = xarUserDetailsBson["LastName"].AsString;
            xarUserDetails.Email = xarUserDetailsBson["Email"].AsString;

            return xarUserDetails;
        }

        public static VerificationDetails BsonToVerificationDetails(BsonDocument verificationDetailsBson)
        {
            var xarVerificationToken = new VerificationDetails();

            xarVerificationToken.VerificationToken = verificationDetailsBson["VerificationToken"].AsString;
            xarVerificationToken.Verified = verificationDetailsBson["Verified"].ToUniversalTime();
            xarVerificationToken.IsVerified = verificationDetailsBson["IsVerified"].AsBoolean;

            return xarVerificationToken;
        }


        public static Namespace BsonToGameDTO(BsonDocument game)
        {
            var gameDTO = new Namespace();
        
            gameDTO.NamespaceId = game["NamespaceId"].AsString;
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