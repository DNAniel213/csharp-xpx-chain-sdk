using System;

namespace Xarcade.Domain.ProximaX
{
    public class Transaction
    {
        public string Hash {get; set;}
        public ulong Height {get; set;}
        public Asset Asset {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Transaction DTO==="  +
                "\nHash "            + Hash +
                "\nHeight: "         + Height + 
                "\nAsset: "          + Asset.AssetID + 
                "\nDate Created: "   + Created +
                "\n==End of Transaction DTO==";
        }
    }
}