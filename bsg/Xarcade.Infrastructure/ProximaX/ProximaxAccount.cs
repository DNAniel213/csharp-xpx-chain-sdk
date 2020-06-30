using System.Collections.Generic;
using System;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using XarcadeModel = Xarcade.Domain.Models;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ProximaX.Sirius.Chain.Sdk.Infrastructure;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;

namespace Xarcade.Api.Prototype.Blockchain
{
    /// <summary>
    /// Contains all methods involving an Account
    /// </summary>
    public class ProximaxAccount
    {
        public ProximaxBlockchainPortal portal;
        public ProximaxAccount(ProximaxBlockchainPortal portal)
        {
            this.portal = portal;
        }

        /// <summary>
        /// Generates Wallet Address
        /// </summary>
        /// <param name="userId">Client-generated user ID</param>
        /// <returns></returns>
        public XarcadeModel.AccountDTO CreateAccount(long userId)
        {
            XarcadeModel.AccountDTO accountDTO = new XarcadeModel.AccountDTO();
            Account account = Account.GenerateNewAccount(portal.networkType);

            accountDTO.userID           = userId;
            accountDTO.walletAddress    = account.Address.Pretty;
            accountDTO.privateKey       = account.PrivateKey;
            accountDTO.publicKey        = account.PublicKey;
            accountDTO.created          = DateTime.Now;

            return accountDTO;
        }

        /// <summary>
        /// Generates Wallet Address based on private Key
        /// </summary>
        /// <param name="userId">Client-generated user ID</param>
        /// <param name="privateKey">external token for private key</param>
        /// <returns></returns>
        public XarcadeModel.AccountDTO CreateAccount(long userId, string privateKey)
        {
            XarcadeModel.AccountDTO accountDTO = new XarcadeModel.AccountDTO();
            Account account = Account.CreateFromPrivateKey(privateKey, portal.networkType);

            accountDTO.userID           = userId;
            accountDTO.walletAddress    = account.Address.Pretty;
            accountDTO.privateKey       = account.PrivateKey;
            accountDTO.publicKey        = account.PublicKey;
            accountDTO.created          = DateTime.Now;

            return accountDTO;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<XarcadeModel.AccountDTO> GetAccountInformation(string privateKey)
        {
            XarcadeModel.AccountDTO accountDTO = new XarcadeModel.AccountDTO();
            Address address = new Address(privateKey, portal.networkType);
            AccountInfo accountInfo = await portal.siriusClient.AccountHttp.GetAccountInfo(address);

            accountDTO.userID           = 000; //get from db
            accountDTO.walletAddress    = accountInfo.Address.Pretty;
            accountDTO.privateKey       = privateKey;
            accountDTO.publicKey        = accountInfo.PublicKey;
            accountDTO.created          = DateTime.Now;
            return accountDTO;
        }


        /// <summary>
        /// Retrieves all the account's transactions into a list
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="numberOfResults"></param>
        /// <returns></returns>
        public async Task<List<XarcadeModel.TransactionDTO>> GetAccountTransactions(string address, int numberOfResults)
        {
            List<XarcadeModel.TransactionDTO> transactionDTOList = new List<XarcadeModel.TransactionDTO>();


            Address addressObject = new Address(address, portal.networkType);
            AccountInfo accountInfo = await portal.siriusClient.AccountHttp.GetAccountInfo(addressObject);
            var queryParams = new QueryParams(numberOfResults, "");

            var transactions = await portal.siriusClient.AccountHttp.Transactions(accountInfo.PublicAccount, queryParams);
            foreach (Transaction transaction in transactions)
            {
                XarcadeModel.TransactionDTO iTransaction = new XarcadeModel.TransactionDTO();
                iTransaction.Hash                        = transaction.TransactionInfo.Hash;
                iTransaction.Height                      = transaction.TransactionInfo.Height;
                iTransaction.Created                     = transaction.Deadline.GetLocalDateTime();    

                XarcadeModel.AssetDTO assetDTO = new XarcadeModel.AssetDTO
                {
                    AssetID = transaction.TransactionInfo.Id,
                    Name    = "x",
                    Quantity = 0,
                    Owner   = null,
                    Created = DateTime.Now
                };
                iTransaction.Asset = assetDTO;
    
                transactionDTOList.Add(iTransaction);
            }

            return transactionDTOList;

        }



    }
}