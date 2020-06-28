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
            XarcadeModel.AssetDTO assetDTO = new XarcadeModel.AssetDTO
            {
                AssetID = "XPX",
                Name    = param.message,
                Quantity = param.amount,
                Owner   = param.sender,
                Created = DateTime.Now
            };


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

            transactionDTO.Hash  = transferTransaction.GetHashCode().ToString();
            transactionDTO.Height = 0000;
            transactionDTO.Asset = assetDTO;


            // Get the generation hash 
            await portal.SignAndAnnounceTransaction(senderAccount, transferTransaction);


            return transactionDTO;
        }

        /// <summary>
        /// Uses a transaction hash to get transaction information
        /// <param name="transactionDTO"></param>
        /// </summary>
        /// <returns></returns>
        public async Task<XarcadeModel.TransactionDTO> GetTransactionInformation (XarcadeModel.TransactionDTO param)
        {
            XarcadeModel.TransactionDTO transactionDTO = new XarcadeModel.TransactionDTO();
            var transactionInfo = await portal.siriusClient.TransactionHttp.GetTransaction(param.Hash);
            transactionDTO.Hash  = transactionInfo.GetHashCode().ToString();
            transactionDTO.Height = 0000;
            transactionDTO.Asset = param.Asset;

            return transactionDTO;
        }

        /// <summary>
        /// Monitors a transaction process
        /// <param name=""> hash value of transaction</param>
        /// </summary>
        /// <returns></returns>
        public async Task<XarcadeModel.TransactionDTO> MonitorTransactionAsync(XarcadeModel.TransactionDTO transDTO, XarcadeParams.SendXpxParams param)
        {
        // Creates instance of SiriusClient
        var client = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
        var siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
        Account senderAccount = Account.CreateFromPrivateKey(param.sender.PrivateKey, portal.networkType);
        var transactionInfo = await portal.siriusClient.TransactionHttp.GetTransaction(transDTO.Hash);
        //XarcadeModel.TransactionDTO transaction = GetTransactionInformation(transactionDTO).GetAwaiter().GetResult();
        //var transaction = GetTransactionInformation(transactionDTO).GetAwaiter().GetResult();

        // Generates new account
        var newAccount = Account.GenerateNewAccount(portal.networkType);

        // Get the generation hash 
        var generationHash = await siriusClient.BlockHttp.GetGenerationHash();

        // Signs the transaction using the registered account
        var signedTransaction = senderAccount.Sign
        (
            transactionInfo,
            generationHash
        );

        // Creates instance of SiriusWebSocketClient to monitor transactions
        // If you need to enable the secure protocol use
        // new SiriusWebSocketClient("bctestnet1.xpxsirius.io", 3000,useSSL:true);
        var ws = new SiriusWebSocketClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL, 3000);

        // Opens the listener
        await ws.Listener.Open();

        // Monitors if the websocker listener is alive by subscribing to NewBlock channel.
        // Blocks are generated every 15 seconds in average, so a timeout can be raised if
        // there is no response after 30 seconds.
        ws.Listener.NewBlock()
        .Timeout(TimeSpan.FromSeconds(30))  
        .Subscribe(
            block => {
            Console.WriteLine($"New block is created {block.Height}");
            },
            err => {
            Console.WriteLine($"Unexpected error {err}");
            ws.Listener.Close();
            }
        );

        // Monitors if there is any validation error with the issued transaction
        var signerAddress = Address.CreateFromPublicKey(signedTransaction.Signer,portal.networkType);

        ws.Listener.TransactionStatus(signerAddress)
        .Timeout(TimeSpan.FromSeconds(30))  
        .Subscribe(
            // transaction info
            tx =>
            {
                Console.WriteLine($"Transaction id {tx.Hash} - status {tx.Status}");
            },
            // handle if any error occured
            txErr =>
            {
                Console.WriteLine($"Transaction error - {txErr}");
                ws.Listener.Close();
            }
        );
        

        // Monitors if the transaction arrives the network but not yet include in the block
        var unconfirmedTx = await ws.Listener.UnconfirmedTransactionsAdded(newAccount.Address)
                                            .Take(1)
                                            .Timeout(TimeSpan.FromSeconds(30));

        // Monitors if the transaction get included in the block
        var confirmedTx = await ws.Listener.ConfirmedTransactionsGiven(newAccount.Address)
                                        .Take(1)
                                        .Timeout(TimeSpan.FromSeconds(30));

        // Announces to the network
        await client.TransactionHttp.Announce(signedTransaction);

        // Gets the results
        var unconfirmedResult =  confirmedTx;

        Console.WriteLine($"Request transaction {unconfirmedResult.TransactionInfo.Hash} reached network");

        var confirmedResult = confirmedTx;

        Console.WriteLine($"Request confirmed with transaction {confirmedResult.TransactionInfo.Hash}");

        return null;
        }
    }

}