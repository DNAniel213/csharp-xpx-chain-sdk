using System.Reactive.Linq;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
using Xarcade.Infrastructure.Utilities.Logger;
using Xarcade.Infrastructure.Abstract;

namespace Xarcade.Infrastructure.ProximaX
{
    public class ProximaxBlockchainPortal : IBlockchainPortal
    {
        private const string PROXIMAX_NODE_URL = "https://bctestnet1.brimstone.xpxsirius.io";
        private static SiriusClient siriusClient = null;
        private static NetworkType network;
        private static ILogger _logger;
        


        public ProximaxBlockchainPortal(IConfiguration configuration)
        {
            LoggerFactory.Configuration = configuration;
            _logger = LoggerFactory.GetLogger(Logger.Dummy);

            if (siriusClient == null)
            {
                siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            }
            network = this.GetNetworkType();
        }

        /// <summary> 
        /// Retrieves the _network type of the Proximax node
        /// </summary>
        /// 
        /// <returns> 
        /// Network type (testnet or mainnet)
        /// </returns>
        private NetworkType GetNetworkType()
        {
            return siriusClient.NetworkHttp.GetNetworkType().GetAwaiter().GetResult();
        }

        public async Task<XarcadeModel.Transaction> SignAndAnnounceTransactionAsync(Account account, Transaction transaction)
        {
            if (account == null || transaction == null)
                return null;
            
            var xarTransaction = new XarcadeModel.Transaction();
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                string generationHash = await siriusClient.BlockHttp.GetGenerationHash();
                if (string.IsNullOrWhiteSpace(generationHash))
                {
                    _logger.LogError("Generation Hash is empty!!");
                    return null;
                }
                
                var signedTransaction = account.Sign(transaction, generationHash);
                if (signedTransaction == null)
                {
                    _logger.LogError("Generation Hash is empty!!");
                    return null;
                }
                
                await siriusClient.TransactionHttp.Announce(signedTransaction);

                //transaction = await GetTransactionInformation(signedTransaction.Hash);
                xarTransaction.Hash = signedTransaction.Hash;
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            
            return xarTransaction; 
        }

        public async Task<XarcadeModel.Account> CreateAccountAsync(string userID)
        {
            XarcadeModel.Account xarAccount  = null;

            if(String.IsNullOrWhiteSpace(userID))
            {
                _logger.LogError("User ID does not exist!!");
                return null;
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
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }


            return xarAccount;
        }
        public async Task<XarcadeModel.Account> CreateAccountAsync(string userID, string privateKey)
        {
            XarcadeModel.Account xarAccount = null;

            if(String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(privateKey))
            {
                _logger.LogError("User ID does not exist!!");
                return null;
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
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            

            return xarAccount;

        }


        public async Task<List<XarcadeModel.Transaction>> GetAccountTransactionsAsync(string address, int numberOfResults = 10)
        {

            
            if(String.IsNullOrWhiteSpace(address))
            {
                _logger.LogError("Address does not exist!!");
                return null;
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


            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return transactionList;

        }
        public async Task<(XarcadeModel.Mosaic tMosaic, XarcadeModel.Transaction tx)> CreateMosaicAsync(CreateMosaicParams param)
        {
            if(param.Account == null)
            {
                _logger.LogError("Account does not exist!!");

                return (null, null);
            }
                
            XarcadeModel.Mosaic mosaic = null;
            XarcadeModel.Transaction transaction = null;

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
                            MosaicID = mosaicID.Id +"",
                            Name     = null,
                            Quantity = 0,
                            Created  = DateTime.Now,
                            Owner    = param.Account,
                        };

                        transaction = new XarcadeModel.Transaction
                        {
                            Hash    = mosaicDefinitionTransaction.GetHashCode().ToString(),
                            Height  = 1,
                            Asset   = mosaic,
                            Created = DateTime.Now,
                        };
                        
                    }
                }
                else return (null,null);

            }catch(Exception e)
            {

                _logger.LogError(e.ToString());
                return (null, null);
                //TODO research on possible errors to handle
            }

            return (mosaic,transaction) ;
        }

        public async Task<XarcadeModel.Transaction> ModifyMosaicSupplyAsync(ModifyMosaicSupplyParams param)
        {

            if(param.Account == null || String.IsNullOrWhiteSpace(param.MosaicID) || param.Amount <= 0) 
            {
                _logger.LogError("Input is invaid!!");

                return null;
            } 

            var transaction = new XarcadeModel.Transaction();

            try
            {
                var mosaic = new XarcadeModel.Mosaic();
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
                var mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(Convert.ToUInt64(param.MosaicID)));

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
                    var tx = await SignAndAnnounceTransactionAsync(account, mosaicSupplyChangeTransaction);


                    mosaic = new XarcadeModel.Mosaic
                    {
                        MosaicID = mosaicInfo.MosaicId.Id + "",
                        Name     = null,
                        Quantity = 0,
                        Created  = DateTime.Now,
                        Owner    = param.Account,
                    };


                    transaction = new XarcadeModel.Transaction
                    {
                        Hash = tx.Hash,
                        Height = tx.Height,
                        Asset = mosaic,
                        Created = DateTime.Now,
                    };

                }
            }catch(Exception e)
            {

                _logger.LogError(e.ToString());
                return null;
            }

            return transaction;
        }

        public async Task<XarcadeModel.Mosaic> GetMosaicAsync(string mosaicID)
        {
            XarcadeModel.Mosaic mosaic = null;
            MosaicInfo mosaicInfo = null;
            try
            {

                mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(Convert.ToUInt64(mosaicID)));
                mosaic = new XarcadeModel.Mosaic
                {
                    MosaicID = mosaicInfo.MosaicId.Id + "",
                    Quantity = mosaicInfo.Supply,
                };  


            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return mosaic;
        }
        
        public async Task<List<XarcadeModel.Mosaic>> GetMosaicListAsync(string walletAddress, string mosaicId)
        {
            var networkType = await siriusClient.NetworkHttp.GetNetworkType();

            var mosaicList = new List<XarcadeModel.Mosaic>();
            try
            {
                Address address = new Address(walletAddress, networkType);
                AccountInfo accountInfo = await siriusClient.AccountHttp.GetAccountInfo(address);
                foreach(Mosaic mosaic in accountInfo.Mosaics)
                {
                    var xarMosaic = new XarcadeModel.Mosaic
                    {
                        Quantity = mosaic.Amount,
                        MosaicID = mosaic.Id.Id +"",
                    };
                    mosaicList.Add(xarMosaic);
                }

                if(mosaicList.Count > 0)
                    return mosaicList;
                else
                    return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        } 


        public async Task<XarcadeModel.Transaction> SendMosaicAsync(SendMosaicParams param)
        {
            if(String.IsNullOrWhiteSpace(param.MosaicID) || param.Sender == null || param.Amount <= 0)
            {
                _logger.LogError("Input is invalid!!");
                return null;
            } 

            XarcadeModel.Mosaic mosaic = null;
            XarcadeModel.Transaction transaction = null;

            try 
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();

                var mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(Convert.ToUInt64(param.MosaicID)));

                var senderAccount = Account.CreateFromPrivateKey(param.Sender.PrivateKey, networkType);

                Mosaic mosaicToTransfer = new Mosaic(mosaicInfo.MosaicId, param.Amount);
                Address recepient = new Address(param.RecipientAddress, networkType);

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
                    var transactionInfo = await SignAndAnnounceTransactionAsync(senderAccount, transferTransaction);
                    mosaic = new XarcadeModel.Mosaic
                    {
                        MosaicID = param.MosaicID + "",
                        Name     = mosaicInfo.MetaId,
                        Quantity = param.Amount,
                        Created  = DateTime.Now,
                        Owner    = param.Sender,
                    };

                    transaction = new XarcadeModel.Transaction
                    {
                        Hash    = transactionInfo.Hash,
                        Height  = transactionInfo.Height,
                        Asset   = mosaic,
                        Created = DateTime.Now,
                    };
                }
            }catch(Exception e)
            {

                _logger.LogError(e.ToString());
                return null;
            }finally
            {

            }



            return transaction;
        }

        public async Task<(XarcadeModel.Mosaic mosaic, XarcadeModel.Transaction tx)> LinkMosaicAsync(LinkMosaicParams param)
        {
            if(param.Account == null || String.IsNullOrWhiteSpace(param.MosaicID) || param.Namespace == null)
            {
                return (null,null);
            } 
            

            XarcadeModel.Mosaic mosaic = null;
            XarcadeModel.Transaction transaction = null;
            
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(Convert.ToUInt64(param.MosaicID)));
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
                    var announcedTransaction = await SignAndAnnounceTransactionAsync(account, mosaicLink);

                    
                    var namespaceI = new XarcadeModel.Namespace
                    {   
                        Domain = param.Namespace.Domain,
                        LayerOne = param.Namespace.LayerOne, 
                        LayerTwo = param.Namespace.LayerTwo, 
                        Owner = param.Account,
                    };

                    mosaic = new XarcadeModel.Mosaic
                    {
                        MosaicID = mosaicInfo.MosaicId.Id + "",
                        Name     = null,
                        Quantity = 0,
                        Created  = DateTime.Now,
                        Owner    = param.Account,
                        Namespace=  namespaceI
                    };


                    transaction = new XarcadeModel.Transaction
                    {
                        Hash    = announcedTransaction.Hash,
                        Height  = announcedTransaction.Height,
                        Asset   = mosaic,
                        Created = DateTime.Now,
                    };
                }

            }catch(Exception e)
            {
                    Console.WriteLine(e);

                _logger.LogError(e.ToString());
                return (null,null);
            }
            return (mosaic, transaction);
        }

//TODO @ranz please add check if namespace exists
        public async Task<(XarcadeModel.Namespace gameName,XarcadeModel.Transaction tx)> CreateNamespaceAsync(CreateNamespaceParams param)
        {
            if(param.Account == null || param.Domain == null)
            {
                _logger.LogError("Input is invalid!!");
                return (null,null);
            }

            var xarNamespace = new XarcadeModel.Namespace();
            var xarTransaction = new XarcadeModel.Transaction();
            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
                RegisterNamespaceTransaction  registerNamespaceT = null;

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
                    var trans = await SignAndAnnounceTransactionAsync(account, registerNamespaceT);
                    xarNamespace = new XarcadeModel.Namespace
                    {
                        NamespaceId = registerNamespaceT.NamespaceId.Id +"",
                        Domain  = param.Domain,
                        Created = DateTime.Now,
                        Expiry  = DateTime.Now.AddSeconds(param.Duration * 15),
                        Owner   = param.Account, 
                    };

                    xarTransaction = new XarcadeModel.Transaction
                    {
                        Hash = trans.Hash,
                        Height = trans.Height,
                        Asset = trans.Asset,
                        Created = trans.Created,
                    };
                }

            }catch(Exception e)
            {

                _logger.LogError(e.ToString());
                return (null,null);
            }

            return (xarNamespace,xarTransaction);
        }

        public async Task<(XarcadeModel.Namespace gameName,XarcadeModel.Transaction tx)> ExtendNamespaceDurationAsync(CreateNamespaceParams param)
        {
            if(param.Domain == null)
            {
                _logger.LogError("Input is invalid!!");
                return (null,null);
            }

            XarcadeModel.Namespace renewNamespace = null;
            var networkType = await siriusClient.NetworkHttp.GetNetworkType();
            Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
            RegisterNamespaceTransaction renewT = null;
            XarcadeModel.Transaction transaction = null;

            try
            {
                var namespaceId = new NamespaceId(param.Domain);
                var modelduration = Convert.ToDouble(param.Duration);
                NamespaceType ntype = new NamespaceType();

                ntype = param.Parent != null ? NamespaceType.SUB_NAMESPACE : NamespaceType.ROOT_NAMESPACE;

                if(param.Duration != 0)
                {
                    renewT = new RegisterNamespaceTransaction(
                    networkType,//network type
                    EntityVersion.REGISTER_NAMESPACE.GetValue(),//version
                    Deadline.Create(),//deadline
                    5000000,//max fee based from FeeCalculationStrategy
                    param.Domain,//namespace Name
                    namespaceId,//namespace Id
                    ntype,//namespace Type
                    param.Duration,//duration
                    null,//parent Id 
                    null,//signature 
                    null,//signer 
                    null//transaction Info 
                    );
                }

                renewNamespace = new XarcadeModel.Namespace
                {
                    Domain  = param.Domain,
                    Created = DateTime.Now,
                    Expiry  = DateTime.Now.AddSeconds(modelduration * 15),
                    Owner   = param.Account,
                };

                transaction = await SignAndAnnounceTransactionAsync(account, renewT);


            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return (null,null);
            }
            
            return (renewNamespace, transaction);
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

            }catch(Exception e)
            {

                _logger.LogError(e.ToString());
                return null;
            }

            return xarNamespace;
        }

        public async Task<XarcadeModel.Transaction> SendXPXAsync(SendXpxParams param)
        {
            if(param.Sender == null || param.RecipientAddress == null || param.Amount <= 0)
            {
                _logger.LogError("Input is invalid!!");
                return null;
            } 

            XarcadeModel.Transaction transaction = null;
            var asset = new XarcadeModel.Asset
            {
                Name     = param.Message,
                Quantity = param.Amount,
                Owner    = param.Sender,
                Created  = DateTime.Now
            };

            TransferTransaction transferTransaction = null;

            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                var Address = new Address(param.RecipientAddress, networkType);
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

            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            
            return transaction;
        }

//FIXME no way to get transaction height @Fixed by Janyl
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
                    Height = transactionInfo.TransactionInfo.Height,
                    Asset  = asset,
                };

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return transaction;
        }

        //FIXME: This does not work properly yet -Ranz
        public async Task<XarcadeModel.Account> GetAccountInformationAsync (string address)
        {
            XarcadeModel.Account account = null;

            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                //var accountInfo = await siriusClient.AccountHttp.GetAccountInfo();

                account = new XarcadeModel.Account
                {
                    //WalletAddress = accountInfo.Address.Plain
                };
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return account;
        }



//FIXME this still doesn't work properly @John
        public async Task<Transaction> MonitorTransactionAsync(Transaction transaction)
        {
            if(transaction == null)
            {
                _logger.LogError("Input is invalid!!");
                return null;
            }

            try
            {
                var networkType = await siriusClient.NetworkHttp.GetNetworkType();
                // Creates instance of SiriusClient
                var ws = new SiriusWebSocketClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL, 443, true);
                // Opens the listener
                await ws.Listener.Open();

                // Monitors if the websocker listener is alive by subscribing to NewBlock channel.
                // Blocks are generated every 15 seconds in average, so a timeout can be raised if
                // there is no response after 30 seconds.
                ws.Listener.NewBlock()
                .Timeout(TimeSpan.FromSeconds(30))  
                .Subscribe(
                    block => {
                    },
                    err => {
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
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }

            return null;
        }

        /// <summary> 
        /// Retrieve the current native coin balance of the user in the blockchain
        /// </summary>
        /// 
        /// <param name="account"> 
        /// User's account information
        /// </param>
        /// 
        /// <returns> 
        /// User Account Balance
        /// </returns>
        public async Task<XarcadeModel.Asset> GetTokenBalanceAsync(XarcadeModel.Account account, string tokenName)
        {
            XarcadeModel.Asset accountAssetBalance = null;
            XarcadeModel.Account tokenOwner = null;

            if (account == null)
            {
                _logger.LogError("Account parameter is null!");
                return null;
            }

            if (tokenName == null)
            {
                _logger.LogError("Token parameter is null!");
                return null;
            }

            AccountInfo accountInfo;
            try
            {
                accountInfo = await siriusClient.AccountHttp.GetAccountInfo(new Address (account.WalletAddress, network));
                if (accountInfo == null)
                {
                    _logger.LogError("ProximaX GetAccountInfo returned null!");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"A ProximaX exception occured when trying to call GetAccountInfo! Exception ({ex.Message})");
                return null;
            }

            var mosaicId = accountInfo.Mosaics.Select(m => m.HexId).ToList();
            var mosaicNames = await siriusClient.MosaicHttp.GetMosaicNames(mosaicId);
            
            if (mosaicNames == null)
            {
                _logger.LogError("ProximaX GetAccountInfo GetMosaicNames null!");
                return null;
            }

            var hexId = mosaicNames.Where(n => n.Names.Any(name => name.Equals(tokenName))).FirstOrDefault();
            var mosaic = accountInfo.Mosaics.Where(m => m.HexId == hexId.MosaicId.HexId).FirstOrDefault();

            var balance = mosaic.Amount;

            tokenOwner = new XarcadeModel.Account()
            {
                WalletAddress = account.WalletAddress,
                PrivateKey = account.PrivateKey,
                PublicKey = account.PublicKey,
                Created = DateTime.Now
            };

            accountAssetBalance = new XarcadeModel.Asset()
            {
                Name = tokenName,
                Quantity = mosaic.Amount,
                Owner = tokenOwner,
                Created = DateTime.Now
            };

            return accountAssetBalance;
        }


    }
}