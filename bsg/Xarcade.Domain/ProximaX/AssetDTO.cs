using System;

namespace Xarcade.Domain.Models
{
    public class AssetDTO
    {
        public string AssetID {get; set;}
        public string Name {get; set;}
        public ulong Quantity {get; set;}
        public AccountDTO Owner {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Asset DTO==="  +
                "\nAssetID: "            + AssetID +
                "\nName: "         + Name + 
                "\nQuantity: "          +Quantity + 
                "\nOwner: "          +Owner.userID + 
                "\nDate Created: "   + Created +
                "\n==End of Asset DTO==";
        }
    }
}