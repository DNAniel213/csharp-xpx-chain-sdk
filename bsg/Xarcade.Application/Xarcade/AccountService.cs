using System.Threading.Tasks;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Account;

namespace Xarcade.Application.Xarcade
{
    public class AccountService: IAccountService
    {
        private readonly IDataAccessProximaX dataAccessProximaX;
        private readonly IBlockchainPortal blockchainPortal;
        private static ILogger _logger;

        public AccountService(IDataAccessProximaX dataAccessProximaX, IBlockchainPortal blockchainPortal)
        {
            this.dataAccessProximaX = dataAccessProximaX;
            this.blockchainPortal = blockchainPortal;
        }

        /// <summary>
        /// Empty body function to avoid errors for now
        /// </summary>
        public async Task<AccountTransactionDto> CreateOwnerAccountAsync(long UserID)
        {
            return await Task.FromResult<AccountTransactionDto>(null);
        }

        /// <summary>
        /// Empty body function to avoid errors for now
        /// </summary>
        public async Task<AccountTransactionDto> CreateUserAccountAsync(long UserID, long OwnerID)
        {
            return await Task.FromResult<AccountTransactionDto>(null);
        }

        /// <summary>
        /// Empty body function to avoid errors for now
        /// </summary>
        public async Task<OwnerDto> GetOwnerAccountAsync(long UserID)
        {
            return await Task.FromResult<OwnerDto>(null);
        }

        /// <summary>
        /// Empty body function to avoid errors for now
        /// </summary>
        public async Task<UserDto> GetUserAccountAsync(long UserID, long OwnerID)
        {
            return await Task.FromResult<UserDto>(null);
        }
    }
}