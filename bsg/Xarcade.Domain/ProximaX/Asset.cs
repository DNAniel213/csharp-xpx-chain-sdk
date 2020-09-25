using System;

namespace Xarcade.Domain.ProximaX
{
    public class Asset
    {
        public string AssetID {get; set;} = null;
        public string Name {get; set;} = null;
        public ulong Quantity {get; set;} = 0;
        public string OwnerId {get; set;} = null;
        public Account Owner {get; set;} = null; 
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