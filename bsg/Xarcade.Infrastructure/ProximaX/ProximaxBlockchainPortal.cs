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

using Xarcade.Domain.ProximaX;
using Xarcade.Infrastructure.ProximaX.Params;

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

        public async Task<TransactionDTO> SignAndAnnounceTransactionAsync(Account account, Transaction transaction)
        {
            if (account == null || transaction == null)
                return null;
            

            TransactionDTO transactionDTO = null;
            try
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
                if (string.IsNullOrEmpty(generationHash ))
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
                //transactionDTO = await GetTransactionInformation(signedTransaction.Hash);
                //FIXME need to return transactionDTO, but it lacks Height
            }
            catch(Exception)
            {
                //TODO log e
                return null;
            }
            
            return transactionDTO; 
        }

        public async Task<AccountDTO> CreateAccountAsync(long userID)
        {

            if(userID < 0)
            {
                return null;
                //TODO log exception
            }
            else
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
                var accountDTO  = new AccountDTO();
                Account account = Account.GenerateNewAccount(networkType);

                accountDTO.UserID           = userID;
                accountDTO.WalletAddress    = account.Address.Pretty;
                accountDTO.PrivateKey       = account.PrivateKey;
                accountDTO.PublicKey        = account.PublicKey;
                accountDTO.Created          = DateTime.Now;
                return accountDTO;
            }
        }
        public async Task<AccountDTO> CreateAccountAsync(long userID, string privateKey)
        {
            var accountDTO = new AccountDTO();

            if(userID <= 0)
            {
                return null;
                //TODO log exception
            }

            try
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
                Account account = Account.CreateFromPrivateKey(privateKey, networkType);

                accountDTO.UserID           = userID;
                accountDTO.WalletAddress    = account.Address.Pretty;
                accountDTO.PrivateKey       = account.PrivateKey;
                accountDTO.PublicKey        = account.PublicKey;
                accountDTO.Created          = DateTime.Now;

            }catch(ArgumentException)
            {
                return null;
                //TODO log e
            }
            

            return accountDTO;

        }


        public async Task<List<TransactionDTO>> GetAccountTransactionsAsync(string address, int numberOfResults)
        {
            try
            {
                if(numberOfResults <= 0)
                {
                    numberOfResults = 10;
                }
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
                var transactionDTOList = new List<TransactionDTO>();
                var addressObj = new Address(address, networkType);
                AccountInfo accountInfo = await siriusClient.AccountHttp.GetAccountInfo(addressObj);
                var queryParams = new QueryParams(numberOfResults, "");

                var transactions = await siriusClient.AccountHttp.Transactions(accountInfo.PublicAccount, queryParams);
                foreach (Transaction transaction in transactions)
                {
                    TransactionDTO iTransaction = new TransactionDTO();
//FIXME TransactionInfo throwing nonexisting reference
                    //iTransaction.Hash                        = transaction.TransactionInfo.Hash;
                    //iTransaction.Height                      = transaction.TransactionInfo.Height;
                    iTransaction.Created                     = transaction.Deadline.GetLocalDateTime();    

                    AssetDTO assetDTO = null;
                    iTransaction.Asset = assetDTO;
        
                    transactionDTOList.Add(iTransaction);
                }

                return transactionDTOList;
            }catch(Exception)
            {
                return null;
                //TODO log e
            }
        }

        public async Task<MosaicDTO> CreateMosaicAsync(CreateMosaicParams param)
        {
            if(param.Account == null)
            {
                return null;
                //TODO log exception
            }
                
            MosaicDTO mosaicDTO = new MosaicDTO();
            try
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
                Account account = Account.CreateFromPrivateKey(param.Account.PrivateKey, networkType);

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

                await SignAndAnnounceTransactionAsync(account, mosaicDefinitionTransaction);
                mosaicDTO.MosaicID = mosaicID.Id;
                mosaicDTO.AssetID  =  mosaicID.Id + "";
                mosaicDTO.Name     = null;
                mosaicDTO.Quantity = 0;
                mosaicDTO.Created  = DateTime.Now;
                mosaicDTO.Owner    = param.Account;
            }catch(Exception)
            {
                return null;
                //TODO log e
                //TODO research on possible errors to handle
            }


            return mosaicDTO;
        }

        public async Task<TransactionDTO> ModifyMosaicSupplyAsync(ModifyMosaicSupplyParams param)
        {
            if(param.Account == null || param.MosaicID == 0 || param.Amount <= 0) 
            {
                return null;
                //TODO log exception
            } 

            var transactionDTO = new TransactionDTO();

            try
            {
                var mosaicDTO = new MosaicDTO();
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
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
                    
                await SignAndAnnounceTransactionAsync(account, mosaicSupplyChangeTransaction);
            }catch(Exception)
            {
                return null;
                //TODO log e
            }

            return transactionDTO;
        }

//FIXME make this return null if mosaic does isn't found
        public async Task<MosaicDTO> GetMosaicAsync(ulong mosaicID)
        {
            MosaicDTO mosaicDTO = new MosaicDTO();
            MosaicInfo mosaicInfo = null;
            try
            {
                mosaicInfo = await siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicID));

            }catch(Exception)
            {
                return null;
                //TODO log e
            }
            finally
            {
                mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;
            }

            return mosaicDTO;
        }


        public async Task<TransactionDTO> SendMosaicAsync(SendMosaicParams param)
        {
            if(param.MosaicID == 0 || param.Sender == null || param.Amount <= 0)
            {
                return null;
                //TODO log exception
            } 

            var mosaicDTO = new MosaicDTO();
            var transactionDTO = new TransactionDTO();

            try 
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
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

                await SignAndAnnounceTransactionAsync(senderAccount, transferTransaction);

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

            }catch(Exception)
            {
                return null;
                //TODO log e
            }finally
            {

            }



            return transactionDTO;
        }

        public async Task<TransactionDTO> LinkMosaicAsync(LinkMosaicParams param)
        {
            if(param.Account == null || param.MosaicID == 0 || param.Namespace == null)
            {
                return null;
                //TODO log error
            } 
            

            var mosaicDTO = new MosaicDTO();
            var transactionDTO = new TransactionDTO();
            
            try
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
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

                await SignAndAnnounceTransactionAsync(account, mosaicLink);

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
            }catch(Exception)
            {
                return null;
                //TODO log e
            }
            return transactionDTO;
        }

//TODO @ranz please add check if namespace exists
        public async Task<NamespaceDTO> CreateNamespaceAsync(CreateNamespaceParams param)
        {
            if(param.Account == null || param.Domain == null)
            {
                return null;
                //TODO log exception
            }
            var namespaceDTO = new NamespaceDTO();

            try
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
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

                await SignAndAnnounceTransactionAsync(account, registerNamespaceT);

                namespaceDTO.Domain  = param.Domain;
                namespaceDTO.Created = DateTime.Now;
                namespaceDTO.Expiry  = DateTime.Now.AddDays(param.Duration);
                namespaceDTO.Owner   = param.Account;
            }catch(Exception)
            {
                return null;
                //TODO log e
            }

            return namespaceDTO;
        }

//FIXME make this return null if nonexistent
        public async Task<NamespaceDTO> GetNamespaceInformationAsync (string namespaceName)
        {
            AccountInfo ownerAccountInfo = null;
            NamespaceDTO namespaceDTO = null;
            try
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
                var namespaceInfo = await siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(namespaceName));
                ownerAccountInfo  = await siriusClient.AccountHttp.GetAccountInfo(namespaceInfo.Owner.Address);

                AccountDTO ownerDTO = new AccountDTO
                {
                    UserID = 0,
                    WalletAddress = ownerAccountInfo.Address.Pretty,
                    PrivateKey    = null,
                    PublicKey     = ownerAccountInfo.PublicKey,
                    Created       = DateTime.Now, //FIXME @Dane please get actual creation date
                };
                
                namespaceDTO = new NamespaceDTO
                {
                    Domain   = namespaceName,
                    Owner    = ownerDTO,
                    Expiry   = DateTime.Now,   //FIXME @John please get actual expiry date
                    Created  = DateTime.Now    //FIXME @John please get actual creation date
                };
            }catch(Exception)
            {
                return null;
                //TODO log e
            }

            return namespaceDTO;
        }

        public async Task<TransactionDTO> SendXPXAsync(SendXpxParams param)
        {
            if(param.Sender == null || param.RecepientAddress == null || param.Amount <= 0)
            {
                return null;
                //TODO log e
            } 
            TransactionDTO transactionDTO = new TransactionDTO();
            AssetDTO assetDTO = new AssetDTO
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
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
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

                transactionDTO.Hash   = transferTransaction.GetHashCode().ToString();
                transactionDTO.Asset  = assetDTO;


                // Get the generation hash 
                var result =  SignAndAnnounceTransactionAsync(senderAccount, transferTransaction).GetAwaiter().GetResult();

            }catch(Exception)
            {
                return null;
                //TODO log e
            }finally
            {

            }
            return transactionDTO;
        }

//FIXME no way to get transaction height
        public async Task<TransactionDTO> GetTransactionInformationAsync (string hash)
        {
            var transaction = new TransactionDTO();
            var asset = new AssetDTO();
            try
            {
                var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
                string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
                var transactionInfo   = await siriusClient.TransactionHttp.GetTransaction(hash);
                transaction.Hash   = transactionInfo.GetHashCode().ToString();
                //transaction.Height = transactionInfo.TransactionInfo.Height;
                transaction.Asset  = asset;
            }
            catch (Exception)
            {
                return null;
                //TODO log e
            }
            finally
            {

            }

            return transaction;
        }



//FIXME this still doesn't work properly @John
        public async Task<Transaction> MonitorTransactionAsync(Transaction transaction)
        {
            var networkType = siriusClient.NetworkHttp.GetNetworkType().Wait();
            string generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
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