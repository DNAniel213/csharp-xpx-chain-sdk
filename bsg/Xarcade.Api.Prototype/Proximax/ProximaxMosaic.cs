using ProximaX.Sirius.Chain.Sdk.Model.Mosaics;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions.Messages;
using ProximaX.Sirius.Chain.Sdk.Model.Namespaces;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        public MosaicDefinitionTransaction CreateCoin(Account account, bool isSupplyMutable, bool isTransferable, bool isLevyMutable, int divisibility, ulong duration)
        {
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

            return mosaicDefinitionTransaction;
        }
        
        /// <summary>
        /// Creates a transaction that modifies currency's supply by specified amount
        /// </summary>
        /// <param name="mosaic">Mosaic to add/subtract supply</param>
        /// <param name="amount">mount to add/subtract</param>
        /// <returns></returns>
        public MosaicSupplyChangeTransaction ModifyCoinSupply(MosaicDefinitionTransaction mosaic, ulong amount)
        {
            MosaicSupplyType mosaicSupplyType = amount > 0 ? MosaicSupplyType.INCREASE : MosaicSupplyType.DECREASE;
            var mosaicSupplyChangeTransaction = MosaicSupplyChangeTransaction.Create(
                Deadline.Create(),
                mosaic.MosaicId,
                mosaicSupplyType,
                amount,
                portal.networkType);
            
            return mosaicSupplyChangeTransaction;
        }


        /// <summary>
        /// Gets currency's details
        /// </summary>
        /// <param name="mosaicId"></param>
        /// <returns></returns>
        public async Task<MosaicInfo> GetCoinInfo(string mosaicId)
        {
            var mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicId));

            return mosaicInfo;
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
        public TransferTransaction SendCoin(MosaicInfo currency, Account sender, Address recepient, ulong amount, string message)
        {
            Mosaic mosaicToTransfer = new Mosaic(currency.MosaicId, amount);

            var transferTransaction = TransferTransaction.Create(
                Deadline.Create(),
                recepient,
                new List<Mosaic>()
                {
                    mosaicToTransfer
                },
                PlainMessage.Create(message),
                portal.networkType
            );
            return transferTransaction;
        }

        /// <summary>
        /// Creates a transaction that sends coins from one account to anotherz
        /// </summary>
        /// <param name="mosaic">Mosaic for mosaic Id</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public AliasTransaction LinkNamespaceToMosaic(MosaicInfo mosaic, NamespaceInfo name)
        {
            var mosaicLink = AliasTransaction.CreateForMosaic(
                mosaic.MosaicId,
                name.Id,
                AliasActionType.LINK,
                Deadline.Create(),
                portal.networkType
            );
            return mosaicLink;
        }






//TODO Get Mosaic Info
//TODO Get Mosaic Balance
//TODO Delete Mosaic

//TODO Convert XPX to Mosaic
//TODO Convert Mosaic to XPX
//TODO Send Mosaic
    }
}