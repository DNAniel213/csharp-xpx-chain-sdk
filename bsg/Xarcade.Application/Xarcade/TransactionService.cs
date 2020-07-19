using System.Threading.Tasks;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Application.Xarcade.Models.Account;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Application.Xarcade.Models.Transaction;

namespace Xarcade.Application.Xarcade
{
   
    public class TransactionService
    {
        public readonly IBlockchainPortal blockchainPortal;
        public readonly IDataAccessProximaX dataAccessProximaX;
        public static ILogger _logger;

        public TransactionService(IDataAccessProximaX dataAccessProximaX, IBlockchainPortal blockchainPortal)
        {
            this.dataAccessProximaX = dataAccessProximaX;
            this.blockchainPortal = blockchainPortal;
        }
        /// <summary>
        /// Sends a custom token from user to another user
        /// </summary>
        public async Task<TokenTransactionDto> SendTokenAsync(TokenDto token,AccountDto sender, AccountDto receiver)
        {
            
            return await Task.FromResult<TokenTransactionDto>(null);
        }

        /// <summary>
        /// Sends a Xar token from user to another user
        /// </summary>
        public async Task<TokenTransactionDto> SendXarAsync(TokenDto token,AccountDto sender, AccountDto receiver)
        {
            return await Task.FromResult<TokenTransactionDto>(null);
        }
        /// <summary>
        /// Sends a Xpx token from user to another user
        /// </summary>
        public async Task<TokenTransactionDto> SendXpxAsync(TokenDto token,AccountDto sender, AccountDto receiver)
        {
            return await Task.FromResult<TokenTransactionDto>(null);
        }
    }

}