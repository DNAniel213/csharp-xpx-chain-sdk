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
        /// <param name="senderPrivateKey">The private key of the sender </param>
        /// <param name="receiverAddress">The address of the receiver </param>
        /// <param name="ammount">The ammount of xpx </param>
        /// <param name="message">The message from the sender to the reciever</param>
        /// <returns></returns>
        public async Task<XarcadeModel.TransactionDTO> SendXPXAsync(XarcadeModel.AccountDTO sender, string receiverAddress,ulong ammount,string message)
        {
            XarcadeModel.TransactionDTO transactionDTO = new XarcadeModel.TransactionDTO();
            var siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            var client = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            var networkType = client.NetworkHttp.GetNetworkType().Wait();

            // The registered account with some currency
            Account senderAccount = Account.CreateFromPrivateKey(sender.PrivateKey, portal.networkType);
            var Address = new Address(receiverAddress,networkType);
            // xpx to be transferred
            var xpxToTransfer = NetworkCurrencyMosaic.CreateRelative(ammount);

            // Creates transfer transaction, send 10 units using
            // to the new account address
            var transferTransaction = TransferTransaction.Create(
                Deadline.Create(),
                Address,
                new List<Mosaic>()
                {
                xpxToTransfer
                },
                PlainMessage.Create(message),
                networkType
            );

            // Get the generation hash 
            var generationHash = await siriusClient.BlockHttp.GetGenerationHash();

            // Signs the transaction using the registered account
            var signedTransaction = senderAccount.Sign(transferTransaction,generationHash);

            // Announces to the network
            await client.TransactionHttp.Announce(signedTransaction);

            return transactionDTO;
        }
    }

}