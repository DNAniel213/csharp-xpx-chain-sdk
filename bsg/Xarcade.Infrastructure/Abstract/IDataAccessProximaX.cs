using System;
using System.Collections.Generic;
using MongoDB.Bson;

using Xarcade.Domain.ProximaX;
using Xarcade.Domain.Authentication;

namespace Xarcade.Infrastructure.Abstract
{
    public class XarcadeUserSearchKey
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserID { get; set; }
        public string VerificationToken { get; set; }
        public string RefreshToken { get; set; }
        public int JwtAttachId { get; set; }
    }
    public interface IDataAccessProximaX
    {
        bool SaveOwner(Owner ownerDTO);
        bool SaveUser(User userDTO);
        bool SaveNamespace(Namespace namespaceDTO);
        bool SaveMosaic(Mosaic mosaicDTO);
        bool SaveXar(Domain.ProximaX.Xarcade xarcadeDTO);
        bool SaveTransaction(Transaction transactionDTO);
        bool SaveXarcadeUser(XarcadeUser xarUserDTO);
        bool UpdateNamespaceDuration(string gameName, DateTime expiry);
        bool UpdateMosaicQuantity(string assetId, long newQuantity);
        bool UpdateXarcadeUser(XarcadeUser user);
        bool UpdateMosaicLink(string assetId, Namespace newNamespace);
        XarcadeUser LoadXarcadeUser(XarcadeUserSearchKey searchKey);
        Owner LoadOwner(string userID);
        User LoadUser(string userID);
        Mosaic LoadMosaic(string tokenID);
        Namespace LoadNamespace(string gameID);
        List<Mosaic> LoadMosaicList(string ownerId );
        Boolean CheckExistNamespace(string namespaceName);
        Boolean CheckExistToken(string tokenName);
    }
}
