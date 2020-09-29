using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class LinkMosaicParams
    {
        /// <summary>
        /// The account linking a mosaic to a namespace.
        /// </summary>
        public Account Account {get; set;} = null;

        /// <summary> The blockchain generated unique identifier that represents the mosaic in the ProximaX Blockchain to be linked. </summary>
        public string MosaicID {get; set;} = null;
        
        /// <summary> The namespace to be linked to the mosaic. </summary>
        public Namespace Namespace {get; set;} = null;
    }

}