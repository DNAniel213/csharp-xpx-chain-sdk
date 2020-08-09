namespace Xarcade.Domain.ProximaX
{
    public class Mosaic : Asset
    {
        public ulong MosaicID {get; set;}
        public Namespace Namespace {get; set;}

        public override string ToString()
        {
            return
                "===Asset_Mosaic DTO==="  +
                "\nAssetID: "             + AssetID +
                "\nName: "                + Name + 
                "\nQuantity: "            + Quantity + 
                "\nOwner: "               + Owner.UserID + 
                "\nDate Created: "        + Created +
                "\nMosaicID: "            + MosaicID +
                "\nNamespace: "           + Namespace +
                "\n==End of Asset_Mosaic DTO==";
        }
    }
}