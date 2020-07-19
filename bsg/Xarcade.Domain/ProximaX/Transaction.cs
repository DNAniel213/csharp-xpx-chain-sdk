using System;

namespace Xarcade.Domain.ProximaX
{
    public enum State
    {
        Confirmed,
        Unconfirmed
    }
    public class Transaction
    {
        public State Status {get; set;} 
        public string Hash {get; set;}
        public ulong Height {get; set;}
        public Asset Asset {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Transaction DTO==="  +
                "\nStatus "          + Status +
                "\nHash "            + Hash +
                "\nHeight: "         + Height + 
                "\nAsset: "          + Asset.AssetID + 
                "\nDate Created: "   + Created +
                "\n==End of Transaction DTO==";
        }
    }
    
}