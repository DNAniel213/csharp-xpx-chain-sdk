namespace Xarcade.Domain.ProximaX
{
    /// <summary>
    /// Xarcade Asset-Mosaic Namespace Linked Asset Model
    /// Represents the xarcade asset linked with the blockchain Mosaic and Namespace
    /// Alias/Linked Asset = Namespace + Mosaic + Linked Asset
    /// </summary>
    public class Mosaic : Asset
    {
        /// <summary> The API generated unique identifier that represents the Xarcade Asset in the ProximaX Blockchain </summary>
        public string MosaicID {get; set;}

        /// <summary> The unique namespace that represents the xarcade asset </summary>
        public Namespace Namespace {get; set;}

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
