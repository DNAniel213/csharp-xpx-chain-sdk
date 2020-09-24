using System;

namespace Xarcade.Domain.ProximaX
{
    /// <summary>
    /// Xarcade Transaction Model
    /// Represents a single Xarcade Transaction in the blockchain through the API.
    /// </summary>
    public class Transaction
    {
        /// <summary> The hash code of the transaction. </summary>
        public string Hash {get; set;}

        /// <summary> The height of the transaction. </summary>
        public ulong Height {get; set;}

        /// <summary> The asset of the transaction. </summary>
        public Asset Asset {get; set;}

        /// <summary> The date of the transaction. </summary>
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Xarcade Transaction Model==="  +
                "\nHash "            + Hash +
                "\nHeight: "         + Height + 
                "\nAsset: "          + Asset.AssetID + 
                "\nDate Created: "   + Created +
                "\n==End of Xaracade Transaction Model==";
        }
    }
}
