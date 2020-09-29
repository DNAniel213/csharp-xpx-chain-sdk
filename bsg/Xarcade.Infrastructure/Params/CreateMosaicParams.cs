using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class CreateMosaicParams
    {
        /// <summary>
        /// The account creating the mosaic.
        /// Costs 500 XPX plus transactions fees by default to create a mosaic.
        /// </summary>
        public Account Account {get; set;} = null;

        /// <summary>
        /// The mosaic supply mutability. Default is true.
        /// The mosaic supply can change at a later point.
        /// </summary>
        public bool IsSupplyMutable {get; set;} = true;

        /// <summary>
        /// The mosaic transferability. Default is true.
        /// The mosaic can be transferred between accounts
        /// </summary>
        public bool IsTransferrable {get; set;} = true;

        /// <summary>
        /// The mosaic levy mutability. Default is false.
        /// The levy fee can be adjusted.
        /// Levy is a fee you receive whenever other users transact with your Mosaic
        /// </summary>
        public bool IsLevyMutable {get; set;} = false;

        /// <summary>
        /// The mosaic divisibility. Default is 0.
        /// Determines up to what decimal place the mosaic can be divided.
        /// Max Range: 0 - 6
        /// </summary>
        public int Divisibility {get; set;} = 0;

        /// <summary>
        /// The number of blocks the mosaic will be active. Default is 1000
        /// Maximum of 3650 days (10 years).
        /// For non-expiring mosaics, leave this property undefined.
        /// </summary>
        public ulong Duration {get; set;} = 1000;
    }
}
