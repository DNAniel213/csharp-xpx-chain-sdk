namespace Xarcade.Domain.ProximaX
{
    public class Mosaic : Asset
    {
        public string MosaicID {get; set;} = null;
        public Namespace Namespace {get; set;} = null;

        public override string ToString()
        {
            return
                "===Asset_Mosaic DTO==="  +
                "\nMosaicID: "            + MosaicID +
                "\nAssetID: "            + AssetID +
                "\nName: "         + Name + 
                "\nQuantity: "          +Quantity + 
                "\nOwner: "          +Owner.UserID + 
                "\nDate Created: "   + Created +
                "\n==End of Asset_Mosaic DTO==";
        }
    }
}