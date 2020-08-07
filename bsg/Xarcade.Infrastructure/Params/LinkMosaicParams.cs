using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class LinkMosaicParams
    {
        /// <summary>
        /// Account to link the mosaic from
        /// </summary>
        public Account Account {get; set;} = null;
        public ulong MosaicID {get; set;} = 0;
        public Namespace Namespace {get; set;} = null;
        public long AssetID {get; set;} = 0;
    }

}