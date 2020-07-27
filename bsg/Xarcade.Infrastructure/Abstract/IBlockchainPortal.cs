using System.Collections.Generic;
using System.Threading.Tasks;
using Xarcade.Infrastructure.ProximaX.Params;
using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.Abstract
{
    public interface IBlockchainPortal
    {
        /// <summary>
        /// Generates a wallet for a specific user
        /// </summary>
        /// <param name="userID">Unique identification that represents the user</param>
        /// <returns>Account Information</returns>
        Task<Account> CreateAccountAsync(long userID);

        /// <summary>
        /// Generates a wallet for a specific user using a private key
        /// </summary>
        /// <param name="userID">Unique identification that represents the user</param>
        /// <param name="privateKey">The private key to create the wallet with</param>
        /// <returns></returns>
        Task<Account> CreateAccountAsync(long userID, string privateKey);

        /// <summary>
        /// Retrieves the transactions of specified account
        /// </summary>
        /// <param name="address">Address representing the wallet</param>
        /// <param name="numberOfResults">The number of results retrieved</param>
        /// <returns></returns>
        Task<List<Transaction>> GetAccountTransactionsAsync(string address, int numberOfResults);
        
        /// <summary>
        /// Creates a new mosaic
        /// </summary>
        /// <param name="param">Defines the mutability, transferability, divisibility, and levy mutability of new mosaic</param>
        /// <returns></returns>
        Task<Mosaic> CreateMosaicAsync(CreateMosaicParams param);

        /// <summary>
        /// Creates a transaction that modifies currency's supply by specified amount
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Transaction> ModifyMosaicSupplyAsync(ModifyMosaicSupplyParams param);
        
        /// <summary>
        /// Retrieves the mosaic's details using the mosaic ID. Returns null if it does not exist.
        /// </summary>
        /// <param name="mosaicID">Unique identification that represents the mosaic</param>
        /// <returns></returns>
        Task<Mosaic> GetMosaicAsync(ulong mosaicID);
        
        /// <summary>
        /// Creates a transaction to sends mosaic from one account to another
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Transaction> SendMosaicAsync(SendMosaicParams param);
        
        /// <summary>
        /// Links a mosaic to a namespace
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Transaction> LinkMosaicAsync(LinkMosaicParams param);
        
        /// <summary>
        /// Creates a new namespace
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Namespace> CreateNamespaceAsync(CreateNamespaceParams param);

        /// <summary>
        /// Extends a new namespace duration
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Namespace> ExtendNamespaceDurationAsync(string namespaceName,string privateKey,Namespace namespaceInfo, ulong duration, CreateNamespaceParams param);
        
        /// <summary>
        /// Retrieves namespace information. Returns null if it doesn't exist
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        Task<Namespace> GetNamespaceInformationAsync (string namespaceName);
        
        /// <summary>
        /// Sends xpx to the specified wallet address
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Transaction> SendXPXAsync(SendXpxParams param);


        /// <summary>
        /// Retrieves the transaction information. Returns null if it does not exist
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Transaction> GetTransactionInformationAsync (string hash);

    }
}