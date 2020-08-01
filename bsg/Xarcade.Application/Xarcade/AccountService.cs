using System.Threading.Tasks;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Account;
using Xarcade.Domain.ProximaX;

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
            var wallet = await blockchainPortal.CreateAccountAsync(UserID);
            var owner = new OwnerDto
            {
                UserID = wallet.UserID,
                WalletAddress = wallet.WalletAddress,
                Created = wallet.Created
            };

            var accountTransaction = await blockchainPortal.GetTransactionInformationAsync(owner.WalletAddress);
            var accountTransactionDto = new AccountTransactionDto
            {
                Hash = accountTransaction.Hash,
                Account = owner,
                BlockNumber = 0,
                Created = accountTransaction.Created
            };

            dataAccessProximaX.SaveOwner(accountTransaction);
            return accountTransactionDto;
        }

        /// <summary>
        /// Empty body function to avoid errors for now
        /// </summary>
        public async Task<AccountTransactionDto> CreateUserAccountAsync(long UserID, long OwnerID)
        {
            var wallet = await blockchainPortal.CreateAccountAsync(UserID);

            var user = new UserDto
            {
                UserID = wallet.UserID,
                OwnerID = OwnerID,
                WalletAddress = wallet.WalletAddress,
                Created = wallet.Created
            };

            var accountTransaction = await blockchainPortal.GetTransactionInformationAsync(user.WalletAddress);
            var accountTransactionDto = new AccountTransactionDto
            {
                Hash = accountTransaction.Hash,
                Account = user,
                BlockNumber = 0,
                Created = accountTransaction.Created
            };

            return accountTransactionDto;
        }

        /// <summary>
        /// Empty body function to avoid errors for now
        /// </summary>
        public async Task<OwnerDto> GetOwnerAccountAsync(long UserID)
        {
            
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