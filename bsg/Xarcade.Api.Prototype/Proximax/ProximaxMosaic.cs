using ProximaX.Sirius.Chain.Sdk.Model.Mosaics;
using XarcadeAccount = Xarcade.Api.Prototype.Blockchain.Models;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
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
        /// 
        /// </summary>
        /// <param name="account">Xarcade's Account Model</param>
        /// <param name="isSupplyMutable">The mosaic supply mutability.</param>
        /// <param name="isTransferable">The mosaic transferability.</param>
        /// <param name="isLevyMutable">The mosaic levy mutability</param>
        /// <param name="divisibility">The mosaic divisibility.</param>
        /// <param name="duration">The number of blocks the mosaic will be active.</param>
        public MosaicDefinitionTransaction CreateCurrency(XarcadeAccount.Account account, bool isSupplyMutable, bool isTransferable, bool isLevyMutable, int divisibility, ulong duration)
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






//TODO Get Mosaic Info
//TODO Get Mosaic Balance
//TODO Update Mosaic
//TODO Delete Mosaic

//TODO Convert XPX to Mosaic
//TODO Convert Mosaic to XPX
//TODO Send Mosaic
    }
}