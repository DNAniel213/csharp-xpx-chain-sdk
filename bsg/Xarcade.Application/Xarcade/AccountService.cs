using System;
using System.Collections.Generic;
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
        DataAccessProximaX repo = new DataAccessProximaX();

        public AccountService(IDataAccessProximaX dataAccessProximaX, IBlockchainPortal blockchainPortal)
        {
            this.dataAccessProximaX = dataAccessProximaX;
            this.blockchainPortal = blockchainPortal;
        }

        /// <summary>
        /// Creates the Owner account
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        public async Task<OwnerDto> CreateOwnerAccountAsync(string UserID)
        {
            if (repo.portal.CheckExist("Users", repo.portal.CreateFilter(new KeyValuePair<string, string>("UserID", UserID), FilterOperator.EQUAL)))
            {
                _logger.LogError("User ID already exists!!");
                return null;
            }

            if (String.IsNullOrWhiteSpace(UserID))
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
        public async Task<UserDto> CreateUserAccountAsync(string userID, string ownerID)
        {
            if (repo.portal.CheckExist("Users", repo.portal.CreateFilter(new KeyValuePair<string, string>("UserID", userID), FilterOperator.EQUAL)))
            {
                _logger.LogError("User ID already exists!!");
                return null;
            }

            if (String.IsNullOrWhiteSpace(userID))
            {
                _logger.LogError("Invalid User ID!!");
                return null;
            }
            
            var wallet = await blockchainPortal.CreateAccountAsync(userID);
            
            if (wallet == null)
            {
                _logger.LogError("Wallet returned null!!");
                return null;
            }

            //save to db
            var domUser = new User
            {
                UserID = userID,
                OwnerID = ownerID,
                WalletAddress = wallet.WalletAddress,
                PrivateKey = wallet.PrivateKey,
                PublicKey = wallet.PublicKey,
                Created = wallet.Created
            };

            var user = new UserDto
            {
                UserID = userID,
                OwnerID = ownerID,
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
        public async Task<OwnerDto> GetOwnerAccountAsync(string userID)
        {
            var ownerDB = dataAccessProximaX.LoadOwner(userID);

            if (ownerDB == null)
            {
                _logger.LogError("User not found!!");
                return null;
            }

            var ownerAccountInfo = new OwnerDto
            {
                UserID = userID,
                WalletAddress = ownerDB.WalletAddress,
                Created = ownerDB.Created
            };

            return ownerAccountInfo;
        }

        /// <summary>
        /// Gets the account information of a specific User
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        /// <param name="OwnerID">Unique identification that represents the owner</param>
        public async Task<UserDto> GetUserAccountAsync(string userID)
        {
            var userDB = dataAccessProximaX.LoadUser(userID);

            if (userDB == null)
            {
                _logger.LogError("User not found!!");
                return null;
            }
            
            var userAccountInfo = new UserDto
            {
                UserID = userID,
                OwnerID = userDB.OwnerID,
                WalletAddress = userDB.WalletAddress,
                Created = userDB.Created
            };

            return userAccountInfo;
        }

        /// <summary>
        /// Gets the account information of a specific User TODO: finish this functionS
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        /// <param name="OwnerID">Unique identification that represents the owner</param>
        public async Task<List<UserDto>> GetUserList(string ownerID)
        {
            var userDB = dataAccessProximaX.LoadOwner(ownerID);
            
            return null;
        }
    }
}