using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class ModifyMosaicSupplyParams
    {
        /// <summary>
        /// Account to create the mosaic from
        /// </summary>
        public AccountDTO Account { get; set; } = null;
        public ulong MosaicID { get; set; } = 0;
        public int Amount { get; set; } = -1;
    }

}