using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class ModifyMosaicSupplyParams
    {
        /// <summary> The account modifying the mosaic supply. </summary>
        public Account Account { get; set; } = null;

        /// <summary> The mosaic ID of the mosaic supply being modified. </summary>
        public string MosaicID { get; set; } = null;

        /// <summary> The amount of mosaic supply being modified. </summary>
        public int Amount { get; set; } = 0;
    }

}