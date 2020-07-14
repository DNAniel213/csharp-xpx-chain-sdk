using Xarcade.Domain.ProximaX;
using Xarcade.Domain.Authentication;

namespace Xarcade.Infrastructure.Repository
{
    public interface IDataAccessProximaX
    {
        public bool SaveOwner(Owner ownerDTO);
        public bool SaveUser(User userDTO);
        public bool SaveNamespace(Namespace namespaceDTO);
        public bool SaveMosaic(Mosaic mosaicDTO);
        public bool SaveXar(Domain.ProximaX.Xarcade xarcadeDTO);
        public bool SaveTransaction(Transaction transactionDTO);
        public bool SaveXarcadeUser(XarcadeUser xarUserDTO);
        public Owner LoadOwner(long userID);
        public User LoadUser(long userID);
    }
}
