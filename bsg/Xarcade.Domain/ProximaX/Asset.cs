using System;

namespace Xarcade.Domain.ProximaX
{
    /// <summary>
    /// Xarcade Asset Model
    /// Represents the xarcade asset
    /// </summary>
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
                "===Xarcade Asset Model==="  +
                "\nAssetID: "      + AssetID +
                "\nName: "         + Name + 
                "\nQuantity: "     + Quantity + 
                "\nOwner: "        + Owner.UserID + 
                "\nDate Created: " + Created +
                "\n==End of Xarcade Asset Model==";
        }
    }
}
