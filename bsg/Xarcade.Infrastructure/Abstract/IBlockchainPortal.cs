using System.Collections.Generic;
using System.Threading.Tasks;
using Xarcade.Infrastructure.ProximaX.Params;
using XarcadeModel = Xarcade.Domain.ProximaX;

namespace Xarcade.Api.Blockchain.Abstract
{
    public interface IBlockchainPortal
    {
        /// <summary>
        /// Generates a wallet for a specific user
        /// </summary>
        /// <param name="userID">Unique identification that represents the user</param>
        /// <returns>Account Information</returns>
        Task<XarcadeModel.AccountDTO> CreateAccount(long userID);

        /// <summary>
        /// Generates a wallet for a specific user using a private key
        /// </summary>
        /// <param name="userID">Unique identification that represents the user</param>
        /// <param name="privateKey">The private key to create the wallet with</param>
        /// <returns></returns>
        Task<XarcadeModel.AccountDTO> CreateAccount(long userID, string privateKey);

        /// <summary>
        /// Retrieves the transactions of specified account
        /// </summary>
        /// <param name="address">Address representing the wallet</param>
        /// <param name="numberOfResults">The number of results retrieved</param>
        /// <returns></returns>
        Task<List<XarcadeModel.TransactionDTO>> GetAccountTransactions(string address, int numberOfResults);
        
        /// <summary>
        /// Creates a new mosaic
        /// </summary>
        /// <param name="param">Defines the mutability, transferability, divisibility, and levy mutability of new mosaic</param>
        /// <returns></returns>
        Task<XarcadeModel.MosaicDTO> CreateMosaicAsync(CreateMosaicParams param);

        /// <summary>
        /// Creates a transaction that modifies currency's supply by specified amount
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<XarcadeModel.TransactionDTO> ModifyMosaicSupply(ModifyMosaicSupplyParams param);
        
        /// <summary>
        /// Retrieves the mosaic's details using the mosaic ID. Returns null if it does not exist.
        /// </summary>
        /// <param name="mosaicID">Unique identification that represents the mosaic</param>
        /// <returns></returns>
        Task<XarcadeModel.MosaicDTO> GetMosaicAsync(ulong mosaicID);
        
        /// <summary>
        /// Creates a transaction to sends mosaic from one account to another
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<XarcadeModel.TransactionDTO> SendMosaicAsync(SendMosaicParams param);
        
        /// <summary>
        /// Links a mosaic to a namespace
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<XarcadeModel.TransactionDTO> LinkMosaicAsync(LinkMosaicParams param);
        
        /// <summary>
        /// Creates a new namespace
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<XarcadeModel.NamespaceDTO> CreateNamespaceAsync(CreateNamespaceParams param);

        /// <summary>
        /// Retrieves namespace information. Returns null if it doesn't exist
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        Task<XarcadeModel.NamespaceDTO> GetNamespaceInformation (string namespaceName);
        
        /// <summary>
        /// Sends xpx to the specified wallet address
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<XarcadeModel.TransactionDTO> SendXPXAsync(SendXpxParams param);


        /// <summary>
        /// Retrieves the transaction information. Returns null if it does not exist
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<XarcadeModel.TransactionDTO> GetTransactionInformation (string hash);

    }
}