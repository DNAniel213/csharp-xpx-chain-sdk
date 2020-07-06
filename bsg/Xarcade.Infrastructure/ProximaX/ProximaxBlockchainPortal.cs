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
using Xarcade.Api.Blockchain.Abstract;
using Xarcade.Infrastructure.ProximaX.Params;

namespace Xarcade.Api.Blockchain
{
    public class ProximaxBlockchainPortal : IBlockchainPortal
    {
        private const string PROXIMAX_NODE_URL = "https://bctestnet1.brimstone.xpxsirius.io"; 
        private static SiriusClient siriusClient = null;
        private NetworkType networkType = default(NetworkType);
        private const string TRANSACTION_TIME_STAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        private const string PROXIMAX_MOSAIC_NAME = "same.xpx";
        private static string generationHash = null;

        public ProximaxBlockchainPortal()
        {
            if (siriusClient == null)
            {
                siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            }
            networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
            generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
        }

        public async Task<XarcadeModel.TransactionDTO> SignAndAnnounceTransaction(Account account, Transaction transaction)
        {
            SignedTransaction signedTransaction = null;
            XarcadeModel.TransactionDTO transactionDTO = null;
            try
            {
                signedTransaction = account.Sign(transaction, generationHash);
                
            }catch(Exception)
            {
                generationHash    = siriusClient.BlockHttp.GetGenerationHash().Wait();
                signedTransaction = account.Sign(transaction, generationHash);
                //TODO log e
            }finally
            {
                var response = await siriusClient.TransactionHttp.Announce(signedTransaction);
                //transactionDTO = await GetTransactionInformation(signedTransaction.Hash);
//FIXME need to return transactionDTO, but it lacks Height
            }
            return transactionDTO; 
        }

        public async Task<XarcadeModel.AccountDTO> CreateAccount(long userID)
        {
            if(userID < 0)
            {
                throw new System.ArgumentException("Parameter must be greater than 0", "userID");
                //TODO log exception
            }
            else
            {
                var accountDTO  = new XarcadeModel.AccountDTO();
                Account account = Account.GenerateNewAccount(networkType);

                accountDTO.UserID           = userID;
                accountDTO.WalletAddress    = account.Address.Pretty;
                accountDTO.PrivateKey       = account.PrivateKey;
                accountDTO.PublicKey        = account.PublicKey;
                accountDTO.Created          = DateTime.Now;
                return accountDTO;
            }
        }
        public async Task<XarcadeModel.AccountDTO> CreateAccount(long userID, string privateKey)
        {
            var accountDTO = new XarcadeModel.AccountDTO();

            if(userID <= 0)
            {
                throw new System.ArgumentException("Parameter must be greater than 0", "userID");
                //TODO log exception
            }
            else
            {
                try
                {
                    Account account = Account.CreateFromPrivateKey(privateKey, networkType);

                    accountDTO.UserID           = userID;
                    accountDTO.WalletAddress    = account.Address.Pretty;
                    accountDTO.PrivateKey       = account.PrivateKey;
                    accountDTO.PublicKey        = account.PublicKey;
                    accountDTO.Created          = DateTime.Now;

                }catch(ArgumentException)
                {
                    throw new System.ArgumentException("Not a valid private key", "privateKey");
                    //TODO log e
                }
            }

            return accountDTO;

        }


        public async Task<List<XarcadeModel.TransactionDTO>> GetAccountTransactions(string address, int numberOfResults)
        {
            try
            {
                if(numberOfResults <= 0)
                {
                    numberOfResults = 10;
                }

                var transactionDTOList = new List<XarcadeModel.TransactionDTO>();
                var addressObj = new Address(address, networkType);
                AccountInfo accountInfo = await siriusClient.AccountHttp.GetAccountInfo(addressObj);
                var queryParams = new QueryParams(numberOfResults, "");

                var transactions = await siriusClient.AccountHttp.Transactions(accountInfo.PublicAccount, queryParams);
                foreach (Transaction transaction in transactions)
                {
                    XarcadeModel.TransactionDTO iTransaction = new XarcadeModel.TransactionDTO();
//FIXME TransactionInfo throwing nonexisting reference
                    //iTransaction.Hash                        = transaction.TransactionInfo.Hash;
                    //iTransaction.Height                      = transaction.TransactionInfo.Height;
                    iTransaction.Created                     = transaction.Deadline.GetLocalDateTime();    

                    XarcadeModel.AssetDTO assetDTO = null;
                    iTransaction.Asset = assetDTO;
        
                    transactionDTOList.Add(iTransaction);
                }

                return transactionDTOList;
            }catch(Exception e)
            {
                throw e;
                //TODO log e
            }
        }

        public async Task<XarcadeModel.MosaicDTO> CreateMosaicAsync(CreateMosaicParams param)
        {
            if(param.Account == null)
            {
                throw new System.ArgumentException("Account parameter is required.", "param");
            }
                
            XarcadeModel.MosaicDTO mosaicDTO = new XarcadeModel.MosaicDTO();
            Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
            MosaicId mosaicID =  null;
            MosaicDefinitionTransaction mosaicDefinitionTransaction = null;
            try
            {
                var nonce = MosaicNonce.CreateRandom();
                mosaicID = MosaicId.CreateFromNonce(nonce, account.PublicKey);
                mosaicDefinitionTransaction = MosaicDefinitionTransaction.Create(
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

                await SignAndAnnounceTransaction(account, mosaicDefinitionTransaction);
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                //TODO log e
                //TODO research on possible errors to handle
            }finally
            {
                mosaicDTO.MosaicID = mosaicID.Id;
                mosaicDTO.AssetID  =  mosaicID.Id + "";
                mosaicDTO.Name     = null;
                mosaicDTO.Quantity = 0;
                mosaicDTO.Created  = DateTime.Now;
                mosaicDTO.Owner    = param.Account;
            }


            return mosaicDTO;
        }

        public async Task<XarcadeModel.TransactionDTO> ModifyMosaicSupply(ModifyMosaicSupplyParams param)
        {
            if(param.Account == null || param.MosaicID == 0 || param.Amount <= 0) 
            {
                throw new System.ArgumentException("All parameters are required.", "Account, MosaicID, Amount");
                //TODO log exception
            } 

            XarcadeModel.MosaicDTO mosaicDTO = new XarcadeModel.MosaicDTO();
            XarcadeModel.TransactionDTO transactionDTO = new XarcadeModel.TransactionDTO();
            MosaicInfo mosaicInfo = null;
            Account account = null;
            MosaicSupplyChangeTransaction mosaicSupplyChangeTransaction = null;
            try
            {
                account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
                mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.MosaicID));

                MosaicSupplyType mosaicSupplyType = param.Amount > 0 ? MosaicSupplyType.INCREASE : MosaicSupplyType.DECREASE;
                ulong sendAmount = Convert.ToUInt32(param.Amount);
                mosaicSupplyChangeTransaction = MosaicSupplyChangeTransaction.Create(
                    Deadline.Create(),
                    mosaicInfo.MosaicId,
                    mosaicSupplyType,
                    sendAmount,
                    networkType);

                    
                await SignAndAnnounceTransaction(account, mosaicSupplyChangeTransaction);
            }catch(Exception e)
            {
                throw e;
                //TODO log e
            }finally
            {
                mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;
                mosaicDTO.AssetID  =  mosaicInfo.MosaicId.Id + "";
                mosaicDTO.Name     = null;
                mosaicDTO.Quantity = 0;
                mosaicDTO.Created  = DateTime.Now;
                mosaicDTO.Owner    = param.Account;

                transactionDTO.Hash    = mosaicSupplyChangeTransaction.GetHashCode().ToString();
                transactionDTO.Height  = mosaicSupplyChangeTransaction.TransactionInfo.Height;
                transactionDTO.Asset   = mosaicDTO;
                transactionDTO.Created = DateTime.Now;
            }

            return transactionDTO;
        }

//FIXME make this return null if mosaic does isn't found
        public async Task<XarcadeModel.MosaicDTO> GetMosaicAsync(ulong mosaicID)
        {
            XarcadeModel.MosaicDTO mosaicDTO = new XarcadeModel.MosaicDTO();
            MosaicInfo mosaicInfo = null;
            try
            {
                mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicID));

            }catch(Exception e)
            {
                throw e;
            }
            finally
            {
                mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;
            }

            return mosaicDTO;
        }


        public async Task<XarcadeModel.TransactionDTO> SendMosaicAsync(SendMosaicParams param)
        {
            if(param.MosaicID == 0 || param.Sender == null || param.Amount <= 0)
            {
                throw new System.ArgumentException("All parameters are required.", "Sender, MosaicID, Amount");
            } 

            Account senderAccount = null;
            TransferTransaction transferTransaction = null;
            var transactionDTO = new XarcadeModel.TransactionDTO();
            var mosaicDTO = new XarcadeModel.MosaicDTO();
            MosaicInfo mosaicInfo = null;

            try 
            {
                mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.MosaicID));
                senderAccount = Account.CreateFromPrivateKey(param.Sender.PrivateKey, networkType);

                Mosaic mosaicToTransfer = new Mosaic(mosaicInfo.MosaicId, param.Amount);
                Address recepient = new Address(param.RecepientAddress, networkType);

                transferTransaction = TransferTransaction.Create(
                    Deadline.Create(),
                    recepient,
                    new List<Mosaic>()
                    {
                        mosaicToTransfer
                    },
                    PlainMessage.Create(param.Message),
                    networkType
                );

                await SignAndAnnounceTransaction(senderAccount, transferTransaction);


            }catch(Exception e)
            {
                throw e;
                //TODO log e
            }finally
            {
                mosaicDTO.MosaicID = param.MosaicID;
                mosaicDTO.AssetID  =  param.MosaicID + "";
                mosaicDTO.Name     = mosaicInfo.MetaId;
                mosaicDTO.Quantity = param.Amount;
                mosaicDTO.Created  = DateTime.Now;
                mosaicDTO.Owner    = param.Sender;

                transactionDTO.Hash    = transferTransaction.GetHashCode().ToString();
                transactionDTO.Height  = transferTransaction.TransactionInfo.Height;
                transactionDTO.Asset   = mosaicDTO;
                transactionDTO.Created = DateTime.Now;
            }



            return transactionDTO;
        }

        public async Task<XarcadeModel.TransactionDTO> LinkMosaicAsync(LinkMosaicParams param)
        {
            if(param.Account == null || param.MosaicID == 0 || param.Namespace == null)
            {
                throw new System.ArgumentException("All parameters are required.", "AccountDTO, MosaicID, NamespaceDTO");
            } //Change to exceptions

            var mosaicDTO = new XarcadeModel.MosaicDTO();
            var transactionDTO = new XarcadeModel.TransactionDTO();
            MosaicInfo mosaicInfo = null;
            AliasTransaction mosaicLink = null;
            Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
            
            try
            {
                mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.MosaicID));
                var namespaceInfo = await siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(param.Namespace.Domain));

                mosaicLink = AliasTransaction.CreateForMosaic(
                    mosaicInfo.MosaicId,
                    namespaceInfo.Id,
                    AliasActionType.LINK,
                    Deadline.Create(),
                    networkType
                );
                await SignAndAnnounceTransaction(account, mosaicLink);

            }catch(Exception e)
            {
                throw e;
                //TODO log e
            }finally
            {
                mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;
                mosaicDTO.AssetID  =  mosaicInfo.MosaicId.Id + "";
                mosaicDTO.Name     = null;
                mosaicDTO.Quantity = 0;
                mosaicDTO.Created  = DateTime.Now;
                mosaicDTO.Owner    = param.Account;

                transactionDTO.Hash    = mosaicLink.GetHashCode().ToString();
                transactionDTO.Height  = mosaicLink.TransactionInfo.Height;
                transactionDTO.Asset   = mosaicDTO;
                transactionDTO.Created = DateTime.Now;
            }
            
            return transactionDTO;
        }

//TODO @ranz please add check if namespace exists
        public async Task<XarcadeModel.NamespaceDTO> CreateNamespaceAsync(CreateNamespaceParams param)
        {
            if(param.Account == null || param.Domain == null)
            {
                throw new System.ArgumentException("The following parameters are required.", "Account, Domain");
            }
            var namespaceDTO = new XarcadeModel.NamespaceDTO();
            Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);
            RegisterNamespaceTransaction registerNamespaceT = null;
            try
            {
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

                await SignAndAnnounceTransaction(account, registerNamespaceT);
            }catch(Exception e)
            {
                throw e;
                //TODO log e
            }finally
            {
                namespaceDTO.Domain  = param.Domain;
                namespaceDTO.Created = DateTime.Now;
                namespaceDTO.Expiry  = DateTime.Now.AddDays(param.Duration);
                namespaceDTO.Owner   = param.Account;
            }

            return namespaceDTO;
        }

//FIXME make this return null if nonexistent
        public async Task<XarcadeModel.NamespaceDTO> GetNamespaceInformation (string namespaceName)
        {
            AccountInfo ownerAccountInfo = null;
            XarcadeModel.NamespaceDTO namespaceDTO = null;
            try
            {
                var namespaceInfo = await siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(namespaceName));
                ownerAccountInfo  = await siriusClient.AccountHttp.GetAccountInfo(namespaceInfo.Owner.Address);

            }catch(Exception e)
            {
                throw e;
                //TODO log e
            }finally
            {
                XarcadeModel.AccountDTO ownerDTO = new XarcadeModel.AccountDTO
                {
                    UserID = 0,
                    WalletAddress = ownerAccountInfo.Address.Pretty,
                    PrivateKey    = null,
                    PublicKey     = ownerAccountInfo.PublicKey,
                    Created       = DateTime.Now, //FIXME @Dane please get actual creation date
                };
                
                namespaceDTO = new XarcadeModel.NamespaceDTO
                {
                    Domain   = namespaceName,
                    Owner    = ownerDTO,
                    Expiry   = DateTime.Now,   //FIXME @John please get actual expiry date
                    Created  = DateTime.Now    //FIXME @John please get actual creation date
                };
            }

            return namespaceDTO;
        }

        public async Task<XarcadeModel.TransactionDTO> SendXPXAsync(SendXpxParams param)
        {
            if(param.Sender == null || param.RecepientAddress == null || param.Amount <= 0)
            {
                throw new System.ArgumentException("All parameters are required.", "Sender, RecepientAddress, Amount");
            } 
            XarcadeModel.TransactionDTO transactionDTO = new XarcadeModel.TransactionDTO();
            XarcadeModel.AssetDTO assetDTO = new XarcadeModel.AssetDTO
            {
                AssetID  = "XPX",
                Name     = param.Message,
                Quantity = param.Amount,
                Owner    = param.Sender,
                Created  = DateTime.Now
            };

            Account senderAccount = Account.CreateFromPrivateKey(param.Sender.PrivateKey, networkType);
            TransferTransaction transferTransaction = null;

            try
            {
                var Address = new Address(param.RecepientAddress, networkType);
                var xpxToTransfer = NetworkCurrencyMosaic.CreateRelative(param.Amount);

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

                transactionDTO.Hash   = transferTransaction.GetHashCode().ToString();
                transactionDTO.Asset  = assetDTO;


                // Get the generation hash 
                var result =  SignAndAnnounceTransaction(senderAccount, transferTransaction).GetAwaiter().GetResult();

            }catch(Exception e)
            {
                throw e;
                //TODO log e
            }finally
            {

            }
            return transactionDTO;
        }

//FIXME make it return null if non-existent
//FIXME no way to get transaction height
        public async Task<XarcadeModel.TransactionDTO> GetTransactionInformation (string hash)
        {
            var transaction = new XarcadeModel.TransactionDTO();
            var asset = new XarcadeModel.AssetDTO();
            Transaction transactionInfo = null;
            try
            {
                transactionInfo   = await siriusClient.TransactionHttp.GetTransaction(hash);
            }
            catch (Exception e)
            {
                throw e;
                //TODO log e
            }
            finally
            {
                transaction.Hash   = transactionInfo.GetHashCode().ToString();
                //transaction.Height = transactionInfo.TransactionInfo.Height;
                transaction.Asset  = asset;
            }

            return transaction;
        }



//FIXME this still doesn't work properly @John
        public async Task<Transaction> MonitorTransactionAsync(Transaction transaction)
        {
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