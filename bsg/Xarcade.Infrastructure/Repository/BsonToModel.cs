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
            return BsonSerializer.Deserialize<XarcadeModel.AccountDTO>(account);
        }

        public static XarcadeModel.UserDTO BsonToUserDTO(BsonDocument account)
        {
            return BsonSerializer.Deserialize<XarcadeModel.UserDTO>(account);
        }
        public static XarcadeModel.UserDTO BsonToOwnerDTO(BsonDocument account)
        {
            return BsonSerializer.Deserialize<XarcadeModel.UserDTO>(account);
        }
    }

}