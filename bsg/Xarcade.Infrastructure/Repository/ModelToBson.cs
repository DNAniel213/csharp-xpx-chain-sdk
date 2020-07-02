using MongoDB.Bson;
using XarcadeModel = Xarcade.Domain.Models;


namespace Xarcade.Api.Prototype.Repository
{
    /// <summary>
    /// Serializes Xarcade.Models into Bson
    /// </summary>
    public static class ModelToBson
    {
        public static BsonDocument AccountDTOtoBson(XarcadeModel.AccountDTO account)
        {
            return account.ToBsonDocument();
        }
    }
}