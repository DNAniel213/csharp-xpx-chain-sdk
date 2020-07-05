using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class LinkMosaicParams
    {
        /// <summary>
        /// Account to link the mosaic from
        /// </summary>
        public AccountDTO Account {get; set;} = null;
        public ulong MosaicID {get; set;} = 0;
        public NamespaceDTO Namespace {get; set;} = null;
    }

}