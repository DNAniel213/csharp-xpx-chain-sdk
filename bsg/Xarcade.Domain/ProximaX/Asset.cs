using System;

namespace Xarcade.Domain.ProximaX
{
    /// <summary>
    /// Xarcade Asset Model
    /// Represents the xarcade asset
    /// </summary>
    public class Asset
    {
        /// <summary> The unique identifier that represents the xarcade asset </summary>
        public long AssetID {get; set;}
       
        /// <summary> The name represents the xarcade asset </summary>
        public string Name {get; set;}

        /// <summary> The quantity of the xarcade asset</summary>
        public ulong Quantity {get; set;}

        /// <summary> The owner model of the xarcade asset</summary>
        public Account Owner {get; set;}

        /// <summary> The xarcade asset creation date </summary>
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