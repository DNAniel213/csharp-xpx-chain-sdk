using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class LinkMosaicParams
    {
        /// <summary>
        /// Account to link the mosaic from
        /// </summary>
        public Account Account {get; set;} = null;
        public string MosaicID {get; set;} = null;
        public Namespace Namespace {get; set;} = null;
    }

}