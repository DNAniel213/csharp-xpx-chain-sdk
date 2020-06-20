using ProximaX.Sirius.Chain.Sdk.Model.Mosaics;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions.Messages;
using ProximaX.Sirius.Chain.Sdk.Model.Namespaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using XarcadeModel = Xarcade.Domain.Models;
using System.Reactive.Linq;
using System;
namespace Xarcade.Api.Prototype.Blockchain
{
    public class ProximaxMosaic
    {
        public ProximaxBlockchainPortal portal;
        public ProximaxMosaic(ProximaxBlockchainPortal portal)
        {
            this.portal = portal;
        }
        /// <summary>
        /// Creates a transaction that generates a new mosaic
        /// </summary>
        /// <param name="account">Xarcade's Account Model</param>
        /// <param name="isSupplyMutable">The mosaic supply mutability.</param>
        /// <param name="isTransferable">The mosaic transferability.</param>
        /// <param name="isLevyMutable">The mosaic levy mutability</param>
        /// <param name="divisibility">The mosaic divisibility.</param>
        /// <param name="duration">The number of blocks the mosaic will be active.</param>
        public async Task<XarcadeModel.MosaicDTO> CreateMosaicAsync(XarcadeModel.AccountDTO accountDTO, bool isSupplyMutable, bool isTransferable, bool isLevyMutable, int divisibility, ulong duration)
        {
            XarcadeModel.MosaicDTO mosaicDTO = new XarcadeModel.MosaicDTO();
            Account account = Account.CreateFromPrivateKey(accountDTO.PrivateKey, portal.networkType);

            var nonce = MosaicNonce.CreateRandom();
            var mosaicId = MosaicId.CreateFromNonce(nonce, account.PublicKey);
            var mosaicDefinitionTransaction = MosaicDefinitionTransaction.Create(
                nonce,
                mosaicId,
                Deadline.Create(),
                MosaicProperties.Create(
                    supplyMutable: isSupplyMutable,
                    transferable: isTransferable,
                    levyMutable: isLevyMutable,
                    divisibility: divisibility,
                    duration: duration
                ),
                portal.networkType);

            mosaicDTO.MosaicID = mosaicId.Id;

            await portal.SignAndAnnounceTransaction(account, mosaicDefinitionTransaction);

            return mosaicDTO;
        }
        
        /// <summary>
        /// Creates a transaction that modifies currency's supply by specified amount
        /// </summary>
        /// <param name="mosaic">Mosaic to add/subtract supply</param>
        /// <param name="amount">mount to add/subtract</param>
        /// <returns></returns>
        public async Task<XarcadeModel.MosaicDTO> ModifyCoinSupply(XarcadeModel.AccountDTO accountDTO, ulong mosaicId, ulong amount)
        {
            XarcadeModel.MosaicDTO mosaicDTO = new XarcadeModel.MosaicDTO();
            Account account = Account.CreateFromPrivateKey(accountDTO.PrivateKey, portal.networkType);
            MosaicInfo mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicId));

            MosaicSupplyType mosaicSupplyType = amount > 0 ? MosaicSupplyType.INCREASE : MosaicSupplyType.DECREASE;
            MosaicSupplyChangeTransaction mosaicSupplyChangeTransaction = MosaicSupplyChangeTransaction.Create(
                Deadline.Create(),
                mosaicInfo.MosaicId,
                mosaicSupplyType,
                amount,
                portal.networkType);

            mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;

            await portal.SignAndAnnounceTransaction(account, mosaicSupplyChangeTransaction);
            
            
            return mosaicDTO;
        }


        /// <summary>
        /// Gets currency's details
        /// </summary>
        /// <param name="mosaicId"></param>
        /// <returns></returns>
        public async Task<XarcadeModel.MosaicDTO> GetMosaicAsync(string mosaicId)
        {
            XarcadeModel.MosaicDTO mosaicDTO = new XarcadeModel.MosaicDTO();

            var mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicId));
            mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;

            return mosaicDTO;
        }

        /// <summary>
        /// Creates a transaction that sends coins from one account to anotherz
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="sender"></param>
        /// <param name="recepient"></param>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<XarcadeModel.TransactionDTO> SendMosaicAsync(ulong mosaicId, XarcadeModel.AccountDTO sender, string recepientAddress, ulong amount, string message)
        {
            XarcadeModel.TransactionDTO transactionDTO = new XarcadeModel.TransactionDTO();
            MosaicInfo mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicId));
            Account senderAccount = Account.CreateFromPrivateKey(sender.PrivateKey, portal.networkType);

            Mosaic mosaicToTransfer = new Mosaic(mosaicInfo.MosaicId, amount);
            Address recepient = new Address(recepientAddress, portal.networkType);

            TransferTransaction transferTransaction = TransferTransaction.Create(
                Deadline.Create(),
                recepient,
                new List<Mosaic>()
                {
                    mosaicToTransfer
                },
                PlainMessage.Create(message),
                portal.networkType
            );
            await portal.SignAndAnnounceTransaction(senderAccount, transferTransaction);


            return transactionDTO;
        }

        /// <summary>
        /// Creates a transaction that sends coins from one account to anotherz
        /// </summary>
        /// <param name="mosaic">Mosaic for mosaic Id</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<XarcadeModel.MosaicDTO> LinkMosaicAsync(XarcadeModel.AccountDTO accountDTO, ulong mosaicId, XarcadeModel.NamespaceDTO namespaceDTO)
        {
            XarcadeModel.MosaicDTO mosaicDTO = new XarcadeModel.MosaicDTO();
            Account account = Account.CreateFromPrivateKey(accountDTO.PrivateKey, portal.networkType);

            MosaicInfo mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicId));
            var namespaceInfo = await portal.siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(namespaceDTO.Domain));

            AliasTransaction mosaicLink = AliasTransaction.CreateForMosaic(
                mosaicInfo.MosaicId,
                namespaceInfo.Id,
                AliasActionType.LINK,
                Deadline.Create(),
                portal.networkType
            );


            await portal.SignAndAnnounceTransaction(account, mosaicLink);
            
            return mosaicDTO;
        }






//TODO Get Mosaic Info
//TODO Get Mosaic Balance
//TODO Delete Mosaic

//TODO Convert XPX to Mosaic
//TODO Convert Mosaic to XPX
//TODO Send Mosaic
    }
}