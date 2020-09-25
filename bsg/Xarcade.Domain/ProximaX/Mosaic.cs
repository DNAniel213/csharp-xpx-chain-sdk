namespace Xarcade.Domain.ProximaX
{
    /// <summary>
    /// Xarcade Asset-Mosaic Namespace Linked Asset Model
    /// Represents the xarcade asset linked with the blockchain Mosaic and Namespace
    /// Alias/Linked Asset = Namespace + Mosaic + Linked Asset
    /// </summary>
    public class Mosaic : Asset
    {
        public string MosaicID {get; set;} = null;
        public Namespace Namespace {get; set;} = null;

        public override string ToString()
        {
            return
                "===Asset_Mosaic Linked Asset Xarcade Model==="  +
                "\nMosaicID: "     + MosaicID +
                "\nNamespaceID: "  + Namespace.NamespaceId +
                "\nAssetID: "      + AssetID +
                "\nName: "         + Name + 
                "\nQuantity: "     + Quantity + 
                "\nOwner: "        + Owner.UserID + 
                "\nDate Created: " + Created +
                "\n==End of Asset_Mosaic Linked Asset Xarcade Model==";
        }
    }
}
