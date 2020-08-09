using System;
using System.Threading.Tasks;
using Xarcade.Infrastructure.Abstract;
using Xarcade.Infrastructure.Repository;
using Xarcade.Infrastructure.Utilities.Logger;
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
        /// Creates the Owner account
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        public async Task<OwnerDto> CreateOwnerAccountAsync(long UserID)
        {
            if (UserID <= 0)
            {
                _logger.LogError("Invalid User ID!!");
                return null;
            }
            
            var wallet = await blockchainPortal.CreateAccountAsync(UserID);

            if (wallet == null)
            {
                _logger.LogError("Wallet returned null!!");
                return null;
            }

            //save to db owner table
            var domOwner = new Owner
            {
                UserID = UserID,
                WalletAddress = wallet.WalletAddress,
                PrivateKey = wallet.PrivateKey,
                PublicKey = wallet.PublicKey,
                Created = wallet.Created
            };

            //save to db user table
            var domUser = new User
            {
                UserID = UserID,
                OwnerID = UserID,
                WalletAddress = wallet.WalletAddress,
                PrivateKey = wallet.PrivateKey,
                PublicKey = wallet.PublicKey,
                Created = wallet.Created
            };

            var owner = new OwnerDto
            {
                UserID = UserID,
                WalletAddress = wallet.WalletAddress,
                Created = wallet.Created
            };

            try
            {
                dataAccessProximaX.SaveOwner(domOwner);
                dataAccessProximaX.SaveUser(domUser); 
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return owner;
        }

        /// <summary>
        /// Creates a user account
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        /// <param name="OwnerID">Unique identification that represents the owner</param>
        public async Task<UserDto> CreateUserAccountAsync(long UserID, long OwnerID)
        {
            if (UserID <= 0)
            {
                _logger.LogError("Invalid User ID!!");
                return null;
            }

            var wallet = await blockchainPortal.CreateAccountAsync(UserID);
            
            if (wallet == null)
            {
                _logger.LogError("Wallet returned null!!");
                return null;
            }

            //save to db
            var domUser = new User
            {
                UserID = UserID,
                OwnerID = OwnerID,
                WalletAddress = wallet.WalletAddress,
                PrivateKey = wallet.PrivateKey,
                PublicKey = wallet.PublicKey,
                Created = wallet.Created
            };

            var user = new UserDto
            {
                UserID = UserID,
                OwnerID = OwnerID,
                WalletAddress = wallet.WalletAddress,
                Created = wallet.Created
            };

            try
            {
                dataAccessProximaX.SaveUser(domUser); //save to db
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return user;
        }

        /// <summary>
        /// Gets the account information of a specific Owner
        /// </summary>
        /// <param name="OwnerID">Unique identification that represents the owner</param>
        public async Task<OwnerDto> GetOwnerAccountAsync(long UserID)
        {
            var ownerDB = dataAccessProximaX.LoadOwner(UserID);

            var ownerAccountInfo = new OwnerDto
            {
                UserID = UserID,
                WalletAddress = ownerDB.WalletAddress,
            };

            return ownerAccountInfo;
        }

        /// <summary>
        /// Gets the account information of a specific User
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        /// <param name="OwnerID">Unique identification that represents the owner</param>
        public async Task<UserDto> GetUserAccountAsync(long UserID)
        {
            var userDB = dataAccessProximaX.LoadUser(UserID);
            
            var userAccountInfo = new UserDto
            {

            };

            return userAccountInfo;
        }
    }
}