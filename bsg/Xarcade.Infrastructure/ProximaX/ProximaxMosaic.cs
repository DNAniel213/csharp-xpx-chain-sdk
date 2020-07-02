using ProximaX.Sirius.Chain.Sdk.Model.Mosaics;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions.Messages;
using ProximaX.Sirius.Chain.Sdk.Model.Namespaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using XarcadeModels  = Xarcade.Domain.Models;
using XarcadeParams  = Xarcade.Domain.Params;
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
        public async Task<XarcadeModels.MosaicDTO> CreateMosaicAsync(XarcadeParams.CreateMosaicParams param)
        {
            if(param.accountDTO == null) return null; //replace with throw exception
                
            XarcadeModels.MosaicDTO mosaicDTO = new XarcadeModels.MosaicDTO();
            Account account = Account.CreateFromPrivateKey(param.accountDTO.PrivateKey, portal.networkType);

            var nonce = MosaicNonce.CreateRandom();
            var mosaicId = MosaicId.CreateFromNonce(nonce, account.PublicKey);
            var mosaicDefinitionTransaction = MosaicDefinitionTransaction.Create(
                nonce,
                mosaicId,
                Deadline.Create(),
                MosaicProperties.Create(
                    supplyMutable: param.isSupplyMutable,
                    transferable: param.isTransferrable,
                    levyMutable: param.isLevyMutable,
                    divisibility: param.divisibility,
                    duration: param.duration
                ),
                portal.networkType);

            mosaicDTO.MosaicID = mosaicId.Id;
            mosaicDTO.AssetID  =  mosaicId.Id + "";
            mosaicDTO.Name     = null;
            mosaicDTO.Quantity = 0;
            mosaicDTO.Created = DateTime.Now;
            mosaicDTO.Owner    = param.accountDTO;

            await portal.SignAndAnnounceTransaction(account, mosaicDefinitionTransaction);

            return mosaicDTO;
        }
        
        /// <summary>
        /// Creates a transaction that modifies currency's supply by specified amount
        /// </summary>
        /// <param name="mosaic">Mosaic to add/subtract supply</param>
        /// <param name="amount">mount to add/subtract</param>
        /// <returns></returns>
        public async Task<XarcadeModels.MosaicDTO> ModifyMosaicSupply(XarcadeParams.ModifyMosaicSupplyParams param)
        {
            if(param.accountDTO == null || param.mosaicID == 0 || param.amount == 0) return null;  //change to exception throwing

            XarcadeModels.MosaicDTO mosaicDTO = new XarcadeModels.MosaicDTO();
            Account account = Account.CreateFromPrivateKey(param.accountDTO.PrivateKey, portal.networkType);
            MosaicInfo mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.mosaicID));

            MosaicSupplyType mosaicSupplyType = param.amount > 0 ? MosaicSupplyType.INCREASE : MosaicSupplyType.DECREASE;
            ulong sendAmount = Convert.ToUInt32(param.amount);
            MosaicSupplyChangeTransaction mosaicSupplyChangeTransaction = MosaicSupplyChangeTransaction.Create(
                Deadline.Create(),
                mosaicInfo.MosaicId,
                mosaicSupplyType,
                sendAmount,
                portal.networkType);
    
            mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;
            mosaicDTO.AssetID  =  mosaicInfo.MosaicId.Id + "";
            mosaicDTO.Name     = null;
            mosaicDTO.Quantity = 0;
            mosaicDTO.Created = DateTime.Now;
            mosaicDTO.Owner    = param.accountDTO;

            await portal.SignAndAnnounceTransaction(account, mosaicSupplyChangeTransaction);
            
            
            return mosaicDTO;
        }


        /// <summary>
        /// Gets currency's details via hex string id
        /// </summary>
        /// <param name="mosaicId"></param>
        /// <returns></returns>
        public async Task<XarcadeModels.MosaicDTO> GetMosaicAsync(string mosaicId)
        {
            XarcadeModels.MosaicDTO mosaicDTO = new XarcadeModels.MosaicDTO();

            var mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicId));
            mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;

            return mosaicDTO;
        }

        /// <summary>
        /// Gets currency's details via ulong id
        /// </summary>
        /// <param name="mosaicId"></param>
        /// <returns></returns>
        public async Task<XarcadeModels.MosaicDTO> GetMosaicAsync(ulong mosaicId)
        {
            XarcadeModels.MosaicDTO mosaicDTO = new XarcadeModels.MosaicDTO();

            var mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicId));
            mosaicDTO.MosaicID = mosaicInfo.MosaicId.Id;

            return mosaicDTO;
        }

        /// <summary>
        /// Creates a transaction that sends coins from one account to another
        /// </summary>
        /// <returns></returns>
        public async Task<XarcadeModels.TransactionDTO> SendMosaicAsync(XarcadeParams.SendMosaicParams param)
        {
            if(param.mosaicID == 0 || param.sender == null || param.amount == 0) return null; //change into try-catch

            XarcadeModels.TransactionDTO transactionDTO = new XarcadeModels .TransactionDTO();
            MosaicInfo mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.mosaicID));
            Account senderAccount = Account.CreateFromPrivateKey(param.sender.PrivateKey, portal.networkType);

            Mosaic mosaicToTransfer = new Mosaic(mosaicInfo.MosaicId, param.amount);
            Address recepient = new Address(param.recepientAddress, portal.networkType);

            TransferTransaction transferTransaction = TransferTransaction.Create(
                Deadline.Create(),
                recepient,
                new List<Mosaic>()
                {
                    mosaicToTransfer
                },
                PlainMessage.Create(param.message),
                portal.networkType
            );

            await portal.SignAndAnnounceTransaction(senderAccount, transferTransaction);


            return transactionDTO;
        }

        /// <summary>
        /// Creates a transaction that sends coins from one account to anotherz
        /// </summary>
        /// <returns></returns>
        public async Task<XarcadeModels.MosaicDTO> LinkMosaicAsync(XarcadeParams.LinkMosaicParams param)
        {
            if(param.accountDTO == null || param.mosaicID == 0 || param.namespaceDTO == null) return null; //Change to exceptions

            XarcadeModels.MosaicDTO mosaicDTO = new XarcadeModels.MosaicDTO();
            Account account = Account.CreateFromPrivateKey(param.accountDTO.PrivateKey, portal.networkType);

            MosaicInfo mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(param.mosaicID));
            var namespaceInfo = await portal.siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(param.namespaceDTO.Domain));

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