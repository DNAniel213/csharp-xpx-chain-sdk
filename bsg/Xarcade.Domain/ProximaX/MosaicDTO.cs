namespace Xarcade.Domain.Models
{
    public class MosaicDTO : AssetDTO
    {
        public ulong MosaicID {get; set;}
        public NamespaceDTO Namespace {get; set;}

        public override string ToString()
        {
            return
                "===Asset_Mosaic DTO==="  +
                "\nMosaicID: "            + MosaicID +
                "\nAssetID: "            + AssetID +
                "\nName: "         + Name + 
                "\nQuantity: "          +Quantity + 
                "\nOwner: "          +Owner.userID + 
                "\nDate Created: "   + Created +
                "\n==End of Asset_Mosaic DTO==";
        }
    }
}