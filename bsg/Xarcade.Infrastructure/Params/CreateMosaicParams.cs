using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class CreateMosaicParams
    {
        /// <summary>
        /// Account to create the mosaic from
        /// </summary>
        public Account Account {get; set;} = null;
        /// <summary>
        /// The mosaic supply mutability.
        /// </summary>
        public bool IsSupplyMutable {get; set;} = true;
        /// <summary>
        /// The mosaic transferability. Default is true
        /// </summary>
        public bool IsTransferrable {get; set;} = true;
        /// <summary>
        /// The mosaic levy mutability. Default is true
        /// </summary>
        public bool IsLevyMutable {get; set;} = false;
        /// <summary>
        /// The mosaic divisibility. Default is false
        /// </summary>
        public int Divisibility {get; set;} = 0;
        /// <summary>
        /// The number of blocks the mosaic will be active. Default is 1000
        /// </summary>
        public ulong Duration {get; set;} = 10000;
        /// <summary>
        /// The Id of the asset connected to this mosaic. Default is 0
        /// </summary>
        public long AssetID {get; set;} = 0;
        /// <summary>
        /// The Namespace connected of this mosaic. Default is null
        /// </summary>
        public Namespace Namespace {get; set;} = null;
    }
}