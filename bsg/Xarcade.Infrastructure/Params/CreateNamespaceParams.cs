using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class CreateNamespaceParams
    {
        /// <summary>
        /// The account creating the namespace.
        /// Costs 1 xpx per block plus transactions fees.
        /// Sub-namespaces has a fixed cost of 100 xpx plus transaction fees.
        /// </summary>
        public Account Account {get; set;} = null;

        /// <summary> The domain/sub-domain namespace. </summary>
        public string Domain {get; set;} = null;

        /// <summary> 
        /// The duration of the namespace in blocks.
        /// Default is 1000 blocks. 1 Block = 15 Seconds.
        /// Maximum namespace duration is 365 days
        /// </summary>
        public ulong Duration {get; set;} = 1000;
        
        /// <summary> 
        /// The parent domain namespace when creating a sub-domain space. 
        /// Default is null.
        ///</summary>
        public string Parent {get; set;} = null;
    }
}
