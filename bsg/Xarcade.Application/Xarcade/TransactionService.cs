using Xarcade.Infrastructure.ProximaX;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;

using Xarcade.Application.Xarcade.Models.Token;
using XarcadeModel = Xarcade.Domain.ProximaX;


namespace Xarcade.Application.Xarcade
{
    /// <summary>Xarcade Application Layer GameDto Model</summary>
    public class TransactionService
    {
        public readonly IBlockchainPortal blockchainPortal;
        public readonly DataAccessProximaX dataAccessProximaX;
        public static ILogger _logger;
    }

    public async Task<XarcadeModel.Transaction> (TokenDTO token, Account account, Transaction transaction)
    {
    }
}