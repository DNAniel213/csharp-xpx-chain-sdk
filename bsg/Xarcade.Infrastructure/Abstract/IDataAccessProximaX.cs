using System;
using System.Collections.Generic;
using MongoDB.Bson;

using Xarcade.Domain.ProximaX;
using Xarcade.Domain.Authentication;
using Xarcade.Infrastructure.Cryptography;

namespace Xarcade.Infrastructure.Abstract
{
    public interface IDataAccessProximaX
    {
        bool SaveOwner(Owner ownerDTO);
        bool SaveUser(User userDTO);
        bool SaveNamespace(Namespace namespaceDTO);
        bool SaveMosaic(Mosaic mosaicDTO);
        bool SaveXar(Domain.ProximaX.Xarcade xarcadeDTO);
        bool SaveTransaction(Transaction transactionDTO);
        bool SaveXarcadeUser(XarcadeUser xarUserDTO);
        bool SaveKeys(Keys keys);
        Keys LoadKeys(string publickey);
        Owner LoadOwner(long userID);
        User LoadUser(long userID);
        Mosaic LoadMosaic(long tokenID);
        Namespace LoadNamespace(string gameName);
        Namespace LoadNamespace(long gameID);
        List<BsonDocument> LoadMosaicList(Owner ownerDTO);
        Boolean CheckExistNamespace(string namespaceName);
        Boolean CheckExistToken(string tokenName);
    }
}
