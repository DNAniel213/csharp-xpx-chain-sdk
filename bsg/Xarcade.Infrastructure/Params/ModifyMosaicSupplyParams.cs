using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class ModifyMosaicSupplyParams
    {
        /// <summary>
        /// Account to create the mosaic from
        /// </summary>
        public Account Account { get; set; } = null;
        public string MosaicID { get; set; } = null;
        public int Amount { get; set; } = -1;
    }

}