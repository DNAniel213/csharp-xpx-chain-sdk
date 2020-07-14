using Xarcade.Infrastructure.ProximaX;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;


namespace Xarcade.Application.Xarcade
{
   
    public class TransactionService
    {
        public readonly IBlockchainPortal blockchainPortal;
        public readonly IDataAccessProximaX dataAccessProximaX;
        public static ILogger _logger;
    }

}