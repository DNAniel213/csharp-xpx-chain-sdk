using ProximaX.Sirius.Chain.Sdk.Model.Mosaics;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
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
        /// Generates a new Mosaic 
        /// </summary>
        /// <param name="account">Xarcade's Account Model</param>
        /// <param name="isSupplyMutable">The mosaic supply mutability.</param>
        /// <param name="isTransferable">The mosaic transferability.</param>
        /// <param name="isLevyMutable">The mosaic levy mutability</param>
        /// <param name="divisibility">The mosaic divisibility.</param>
        /// <param name="duration">The number of blocks the mosaic will be active.</param>
        public MosaicDefinitionTransaction CreateCurrency(Account account, bool isSupplyMutable, bool isTransferable, bool isLevyMutable, int divisibility, ulong duration)
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
        /// Modifies currency's supply by specified amount
        /// </summary>
        /// <param name="mosaic">Mosaic to add/subtract supply</param>
        /// <param name="amount">mount to add/subtract</param>
        /// <returns></returns>
        public MosaicSupplyChangeTransaction ModifyCurrencySupply(MosaicDefinitionTransaction mosaic, ulong amount)
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


        public async Task<MosaicInfo> GetMosaicInfo(string mosaicId)
        {
            var mosaicInfo = await portal.siriusClient.MosaicHttp.GetMosaic(new MosaicId(mosaicId));

            return mosaicInfo;
        }







//TODO Get Mosaic Info
//TODO Get Mosaic Balance
//TODO Delete Mosaic

//TODO Convert XPX to Mosaic
//TODO Convert Mosaic to XPX
//TODO Send Mosaic
    }
}