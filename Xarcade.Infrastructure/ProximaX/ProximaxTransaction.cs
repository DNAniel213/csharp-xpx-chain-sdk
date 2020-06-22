using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProximaX.Sirius.Chain.Sdk.Client;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Mosaics;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions.Messages;
using XarcadeModel = Xarcade.Domain.Models;
using XarcadeParams = Xarcade.Domain.Params;

namespace Xarcade.Api.Prototype.Blockchain
{
    /// <summary>
    /// Contains all methods involving a Transaction
    /// </summary>
    public class ProximaxTransaction
    {
        public ProximaxBlockchainPortal portal;
        public ProximaxTransaction(ProximaxBlockchainPortal portal)
        {
            this.portal = portal;
        }
        
        /// <summary>
        /// Sends desired ammount of currency from an account with currency to another account
        /// </summary>
        /// <returns></returns>
        public async Task<XarcadeModel.TransactionDTO> SendXPXAsync(XarcadeParams.SendXpxParams param)
        {
            XarcadeModel.TransactionDTO transactionDTO = new XarcadeModel.TransactionDTO();
            var siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            var client = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            var networkType = client.NetworkHttp.GetNetworkType().Wait();

            // The registered account with some currency
            Account senderAccount = Account.CreateFromPrivateKey(param.sender.PrivateKey, portal.networkType);
            var Address = new Address(param.recepientAddress,networkType);
            // xpx to be transferred
            var xpxToTransfer = NetworkCurrencyMosaic.CreateRelative(param.amount);

            // Creates transfer transaction, send 10 units using
            // to the new account address
            var transferTransaction = TransferTransaction.Create(
                Deadline.Create(),
                Address,
                new List<Mosaic>()
                {
                xpxToTransfer
                },
                PlainMessage.Create(param.message),
                networkType
            );

            // Get the generation hash 
            await portal.SignAndAnnounceTransaction(senderAccount, transferTransaction);


            return transactionDTO;
        }
    }

}