using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ProximaX.Sirius.Chain.Sdk.Model.Blockchain;
using ProximaX.Sirius.Chain.Sdk.Client;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
using ProximaX.Sirius.Chain.Sdk.Infrastructure;
using ProximaX.Sirius.Chain.Sdk.Model.Mosaics;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions.Messages;
using ProximaX.Sirius.Chain.Sdk.Model.Namespaces;

using XarcadeModel = Xarcade.Domain.ProximaX;
using Xarcade.Infrastructure.ProximaX.Params;
using Xarcade.Infrastructure.Abstract;

namespace Xarcade.Infrastructure.ProximaX
{
    public class ProximaxBlockchainPortal : IBlockchainPortal
    {
        private const string PROXIMAX_NODE_URL = "https://bctestnet1.brimstone.xpxsirius.io"; 
        private static SiriusClient siriusClient = null;


        public ProximaxBlockchainPortal()
        {
            if (siriusClient == null)
            {
                siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            }
        }

        public async Task<XarcadeModel.Transaction> SignAndAnnounceTransactionAsync(Account account, Transaction transaction)
        {
            if (account == null || transaction == null)
                return null;
            
            XarcadeModel.Transaction xarTransaction = null;
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                string generationHash = await siriusClient.BlockHttp.GetGenerationHash();
                if (string.IsNullOrWhiteSpace(generationHash))
                {
                    //TODO: Log error empty generationHash
                    return null;
                }
                
                var signedTransaction = account.Sign(transaction, generationHash);
                if (signedTransaction == null)
                {
                    //TODO: Log error empty generationHash
                    return null;
                }
                
                await siriusClient.TransactionHttp.Announce(signedTransaction);

                //transaction = await GetTransactionInformation(signedTransaction.Hash);
                //FIXME need to return transaction, but it lacks Height
            }
            catch(Exception)
            {
                //TODO log e
                return null;
            }
            
            return xarTransaction; 
        }

        public async Task<XarcadeModel.Account> CreateAccountAsync(long userID)
        {
            XarcadeModel.Account xarAccount  = null;

            if(userID < 0)
            {
                return null;
                //TODO log exception
            }

            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                Account account = Account.GenerateNewAccount(networkType);
                if(account != null)
                {
                    xarAccount = new XarcadeModel.Account
                    {
                        UserID           = userID,
                        WalletAddress    = account.Address.Pretty,
                        PrivateKey       = account.PrivateKey,
                        PublicKey        = account.PublicKey,
                        Created          = DateTime.Now,
                    };
                }
            }catch(Exception)
            {
                return null;
                //TODO log exception
            }


            return xarAccount;
        }
        public async Task<XarcadeModel.Account> CreateAccountAsync(long userID, string privateKey)
        {
            XarcadeModel.Account xarAccount = null;

            if(userID < 0 || String.IsNullOrEmpty(privateKey))
            {
                return null;
                //TODO log exception
            }

            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                Account account = Account.GenerateNewAccount(networkType);
                if(account != null)
                {
                    xarAccount = new XarcadeModel.Account
                    {
                        UserID           = userID,
                        WalletAddress    = account.Address.Pretty,
                        PrivateKey       = account.PrivateKey,
                        PublicKey        = account.PublicKey,
                        Created          = DateTime.Now,
                    };
                }
            }catch(Exception)
            {
                return null;
                //TODO log e
            }
            

            return xarAccount;

        }


        public async Task<List<XarcadeModel.Transaction>> GetAccountTransactionsAsync(string address, int numberOfResults = 10)
        {

            
            if(String.IsNullOrWhiteSpace(address))
            {
                return null;
                //TODO log e
            }

            List<XarcadeModel.Transaction> transactionList = null;

            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var addressObj = new Address(address, networkType);
                AccountInfo accountInfo = await siriusClient.AccountHttp.GetAccountInfo(addressObj);
                var queryParams = new QueryParams(numberOfResults, "");

                var transactions = await siriusClient.AccountHttp.Transactions(accountInfo.PublicAccount, queryParams);

                if(transactions.Count > 0)
                {
                    transactionList = new List<XarcadeModel.Transaction>();
                    foreach (Transaction transaction in transactions)
                    {
                        var iTransaction = new XarcadeModel.Transaction();
    //FIXME TransactionInfo throwing nonexisting reference
                        //iTransaction.Hash                        = transaction.TransactionInfo.Hash;
                        //iTransaction.Height                      = transaction.TransactionInfo.Height;
                        iTransaction.Created                     = transaction.Deadline.GetLocalDateTime();    

                        XarcadeModel.Asset asset = null;
                        iTransaction.Asset = asset;
            
                        transactionList.Add(iTransaction);
                    }
                }


            }catch(Exception)
            {
                return null;
                //TODO log e
            }

            return transactionList;

        }

        public async Task<XarcadeModel.Mosaic> CreateMosaicAsync(CreateMosaicParams param)
        {
            if(param.Account == null)
            {
                return null;
                //TODO log exception
            }
                
            XarcadeModel.Mosaic mosaic = null;
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);

                if(account != null)
                {
                    var nonce = MosaicNonce.CreateRandom();
                    var mosaicID = MosaicId.CreateFromNonce(nonce, account.PublicKey);
                    var mosaicDefinitionTransaction = MosaicDefinitionTransaction.Create(
                        nonce,
                        mosaicID,
                        Deadline.Create(),
                        MosaicProperties.Create(
                            supplyMutable : param.IsSupplyMutable,
                            transferable  : param.IsTransferrable,
                            levyMutable   : param.IsLevyMutable,
                            divisibility  : param.Divisibility,
                            duration      : param.Duration
                        ),
                        networkType);

                    if(account!= null && mosaicDefinitionTransaction!= null)
                    {
                        await SignAndAnnounceTransactionAsync(account, mosaicDefinitionTransaction);
                    
                        mosaic = new XarcadeModel.Mosaic
                        {
                            MosaicID = mosaicID.Id,
                            AssetID  =  mosaicID.Id + "",
                            Name     = null,
                            Quantity = 0,
                            Created  = DateTime.Now,
                            Owner    = param.Account,
                        };
                    }
                }

            }catch(Exception)
            {
                return null;
                //TODO log e
                //TODO research on possible errors to handle
            }


            return mosaic;
        }

        public async Task<XarcadeModel.Transaction> ModifyMosaicSupplyAsync(ModifyMosaicSupplyParams param)
        {
            if(param.Account == null || param.MosaicID == 0 || param.Amount <= 0) 
            {
                return null;
                //TODO log exception
            } 

            XarcadeModel.Transaction transaction = null;

            try
            {
                var mosaic = new XarcadeModel.Mosaic();
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
                var mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.MosaicID));

                MosaicSupplyType mosaicSupplyType = param.Amount > 0 ? MosaicSupplyType.INCREASE : MosaicSupplyType.DECREASE;
                ulong sendAmount = Convert.ToUInt32(param.Amount);
                var mosaicSupplyChangeTransaction = MosaicSupplyChangeTransaction.Create(
                    Deadline.Create(),
                    mosaicInfo.MosaicId,
                    mosaicSupplyType,
                    sendAmount,
                    networkType);



                if(account!= null && mosaicSupplyChangeTransaction!= null)
                {
                    await SignAndAnnounceTransactionAsync(account, mosaicSupplyChangeTransaction);

                    mosaic = new XarcadeModel.Mosaic
                    {
                        MosaicID = mosaicInfo.MosaicId.Id,
                        AssetID  =  mosaicInfo.MosaicId.Id + "",
                        Name     = null,
                        Quantity = 0,
                        Created  = DateTime.Now,
                        Owner    = param.Account,
                    };

                    transaction = new XarcadeModel.Transaction
                    {
                        Hash    = mosaicSupplyChangeTransaction.GetHashCode().ToString(),
                        Height  = mosaicSupplyChangeTransaction.TransactionInfo.Height,
                        Asset   = mosaic,
                        Created = DateTime.Now,
                    };
                }
            }catch(Exception)
            {
                return null;
                //TODO log e
            }

            return transaction;
        }

//FIXME make this return null if mosaic does isn't found
        public async Task<XarcadeModel.Mosaic> GetMosaicAsync(ulong mosaicID)
        {
            XarcadeModel.Mosaic mosaic = null;
            MosaicInfo mosaicInfo = null;
            try
            {
                mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicID));
                mosaic = new XarcadeModel.Mosaic
                {
                    MosaicID = mosaicInfo.MosaicId.Id,
                };

            }catch(Exception)
            {
                return null;
                //TODO log e
            }

            return mosaic;
        }


        public async Task<XarcadeModel.Transaction> SendMosaicAsync(SendMosaicParams param)
        {
            if(param.MosaicID == 0 || param.Sender == null || param.Amount <= 0)
            {
                return null;
                //TODO log exception
            } 

            XarcadeModel.Mosaic mosaic = null;
            XarcadeModel.Transaction transaction = null;

            try 
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.MosaicID));
                var senderAccount = Account.CreateFromPrivateKey(param.Sender.PrivateKey, networkType);

                Mosaic mosaicToTransfer = new Mosaic(mosaicInfo.MosaicId, param.Amount);
                Address recepient = new Address(param.RecepientAddress, networkType);

                var transferTransaction = TransferTransaction.Create(
                    Deadline.Create(),
                    recepient,
                    new List<Mosaic>()
                    {
                        mosaicToTransfer
                    },
                    PlainMessage.Create(param.Message),
                    networkType
                );

                if(senderAccount!= null && transferTransaction!= null)
                {
                    await SignAndAnnounceTransactionAsync(senderAccount, transferTransaction);
                    mosaic = new XarcadeModel.Mosaic
                    {
                        MosaicID = param.MosaicID,
                        AssetID  =  param.MosaicID + "",
                        Name     = mosaicInfo.MetaId,
                        Quantity = param.Amount,
                        Created  = DateTime.Now,
                        Owner    = param.Sender,
                    };

                    transaction = new XarcadeModel.Transaction
                    {
                        Hash    = transferTransaction.GetHashCode().ToString(),
                        Height  = transferTransaction.TransactionInfo.Height,
                        Asset   = mosaic,
                        Created = DateTime.Now,
                    };
                }
            }catch(Exception)
            {
                return null;
                //TODO log e
            }finally
            {

            }



            return transaction;
        }

        public async Task<XarcadeModel.Transaction> LinkMosaicAsync(LinkMosaicParams param)
        {
            if(param.Account == null || param.MosaicID == 0 || param.Namespace == null)
            {
                return null;
                //TODO log error
            } 
            

            XarcadeModel.Mosaic mosaic = null;
            XarcadeModel.Transaction transaction = null;
            
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.MosaicID));
                var namespaceInfo = await siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(param.Namespace.Domain));
                Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);

                var mosaicLink = AliasTransaction.CreateForMosaic(
                    mosaicInfo.MosaicId,
                    namespaceInfo.Id,
                    AliasActionType.LINK,
                    Deadline.Create(),
                    networkType
                );

                if(account != null && mosaicLink != null)
                {
                    await SignAndAnnounceTransactionAsync(account, mosaicLink);

                    mosaic = new XarcadeModel.Mosaic
                    {
                        MosaicID = mosaicInfo.MosaicId.Id,
                        AssetID  =  mosaicInfo.MosaicId.Id + "",
                        Name     = null,
                        Quantity = 0,
                        Created  = DateTime.Now,
                        Owner    = param.Account,
                    };


                    transaction = new XarcadeModel.Transaction
                    {
                        Hash    = mosaicLink.GetHashCode().ToString(),
                        Height  = mosaicLink.TransactionInfo.Height,
                        Asset   = mosaic,
                        Created = DateTime.Now,
                    };
                }

            }catch(Exception)
            {
                return null;
                //TODO log e
            }
            return transaction;
        }

//TODO @ranz please add check if namespace exists
        public async Task<XarcadeModel.Namespace> CreateNamespaceAsync(CreateNamespaceParams param)
        {
            if(param.Account == null || param.Domain == null)
            {
                return null;
                //TODO log exception
            }

            XarcadeModel.Namespace xarNamespace = null;
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
                RegisterNamespaceTransaction registerNamespaceT = null;
                if(param.Parent == null)
                {
                    registerNamespaceT = RegisterNamespaceTransaction.CreateRootNamespace(
                        Deadline.Create(),
                        param.Domain,
                        param.Duration,
                        networkType);
                }
                else
                {
                    var parentNamespace = new NamespaceId(param.Parent);
                    registerNamespaceT = RegisterNamespaceTransaction.CreateSubNamespace(
                    Deadline.Create(),
                    param.Domain,
                    parentNamespace,
                    networkType
                );
                }

                if(account != null && registerNamespaceT != null)
                {
                    await SignAndAnnounceTransactionAsync(account, registerNamespaceT);
                    xarNamespace = new XarcadeModel.Namespace
                    {
                        Domain  = param.Domain,
                        Created = DateTime.Now,
                        Expiry  = DateTime.Now.AddDays(param.Duration),
                        Owner   = param.Account,
                    };
                }

            }catch(Exception)
            {
                return null;
                //TODO log e
            }

            return xarNamespace;
        }

//FIXME make this return null if nonexistent
        public async Task<XarcadeModel.Namespace> GetNamespaceInformationAsync (string namespaceName)
        {
            AccountInfo ownerAccountInfo = null;
            XarcadeModel.Namespace xarNamespace = null;
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var namespaceInfo = await siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(namespaceName));
                ownerAccountInfo  = await siriusClient.AccountHttp.GetAccountInfo(namespaceInfo.Owner.Address);

                if(namespaceInfo != null && ownerAccountInfo != null)
                {
                    var owner = new XarcadeModel.Account
                    {
                        UserID = 0,
                        WalletAddress = ownerAccountInfo.Address.Pretty,
                        PrivateKey    = null,
                        PublicKey     = ownerAccountInfo.PublicKey,
                        Created       = DateTime.Now, //FIXME @Dane please get actual creation date
                    };
                    
                    xarNamespace = new XarcadeModel.Namespace
                    {
                        Domain   = namespaceName,
                        Owner    = owner,
                        Expiry   = DateTime.Now,   //FIXME @John please get actual expiry date
                        Created  = DateTime.Now    //FIXME @John please get actual creation date
                    };
                }

            }catch(Exception)
            {
                return null;
                //TODO log e
            }

            return xarNamespace;
        }

        public async Task<XarcadeModel.Transaction> SendXPXAsync(SendXpxParams param)
        {
            if(param.Sender == null || param.RecepientAddress == null || param.Amount <= 0)
            {
                return null;
                //TODO log e
            } 

            XarcadeModel.Transaction transaction = null;
            var asset = new XarcadeModel.Asset
            {
                AssetID  = "XPX",
                Name     = param.Message,
                Quantity = param.Amount,
                Owner    = param.Sender,
                Created  = DateTime.Now
            };

            TransferTransaction transferTransaction = null;

            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var Address = new Address(param.RecepientAddress, networkType);
                var xpxToTransfer = NetworkCurrencyMosaic.CreateRelative(param.Amount);
                Account senderAccount = Account.CreateFromPrivateKey(param.Sender.PrivateKey, networkType);

                // Creates transfer transaction, send 10 units using
                // to the new account address
                transferTransaction = TransferTransaction.Create(
                    Deadline.Create(),
                    Address,
                    new List<Mosaic>()
                    {
                    xpxToTransfer
                    },
                    PlainMessage.Create(param.Message),
                    networkType
                );



                if(senderAccount != null && transferTransaction != null)
                {
                    await SignAndAnnounceTransactionAsync(senderAccount, transferTransaction);
                    transaction = new XarcadeModel.Transaction
                    {
                        Hash   = transferTransaction.GetHashCode().ToString(),
                        Asset  = asset,
                    };
                }

            }catch(Exception)
            {
                return null;
                //TODO log e
            }
            
            return transaction;
        }

//FIXME no way to get transaction height
        public async Task<XarcadeModel.Transaction> GetTransactionInformationAsync (string hash)
        {
            XarcadeModel.Transaction transaction = null;
            var asset = new XarcadeModel.Asset();
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var transactionInfo   = await siriusClient.TransactionHttp.GetTransaction(hash);

                transaction = new XarcadeModel.Transaction
                {
                    Hash   = transactionInfo.GetHashCode().ToString(),
                    //transaction.Height = transactionInfo.TransactionInfo.Height;
                    Asset  = asset,
                };

            }
            catch (Exception)
            {
                return null;
                //TODO log e
            }

            return transaction;
        }



//FIXME this still doesn't work properly @John
        public async Task<Transaction> MonitorTransactionAsync(Transaction transaction)
        {
            var networkType = await siriusClient.NetworkHttp.GetNetworkType();
        // Creates instance of SiriusClient

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
        var signerAddress = Address.CreateFromPublicKey(transaction.Signer.PublicKey, networkType);

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
        var unconfirmedTx = await ws.Listener.UnconfirmedTransactionsAdded(signerAddress)
                                            .Take(1)
                                            .Timeout(TimeSpan.FromSeconds(30));

        // Monitors if the transaction get included in the block
        var confirmedTx = await ws.Listener.ConfirmedTransactionsGiven(signerAddress)
                                        .Take(1)
                                        .Timeout(TimeSpan.FromSeconds(30));


        // Gets the results
        var unconfirmedResult =  confirmedTx;

        Console.WriteLine($"Request transaction {unconfirmedResult.TransactionInfo.Hash} reached network");

        var confirmedResult = confirmedTx;

        Console.WriteLine($"Request confirmed with transaction {confirmedResult.TransactionInfo.Hash}");

        return null;
        }


    }
}