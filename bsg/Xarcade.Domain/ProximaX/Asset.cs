using System;

namespace Xarcade.Domain.ProximaX
{
    public class Asset
    {
        public long AssetID {get; set;}
        public string Name {get; set;}
        public ulong Quantity {get; set;}
        public Account Owner {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Asset DTO==="  +
                "\nAssetID: "            + AssetID +
                "\nName: "         + Name + 
                "\nQuantity: "          +Quantity + 
                "\nOwner: "          +Owner.UserID + 
                "\nDate Created: "   + Created +
                "\n==End of Asset DTO==";
        }
    }
}