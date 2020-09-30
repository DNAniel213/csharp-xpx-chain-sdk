using System;

namespace Xarcade.Domain.ProximaX
{
    /// <summary>
    /// Xarcade Namespace Model
    /// Represents the Namespace derived from the ProximaX Blockchain 
    /// </summary>
    public class Namespace
    {
        /// <summary> The API generated unique identifier that represents the Xarcade Namespace in the ProximaX Blockchain </summary>
        public string NamespaceId {get; set;}
        
        /// <summary> The domain namespace</summary>
        public string Domain {get; set;}
      
        /// <summary> The first sub domain namespace</summary>
        public string LayerOne {get; set;} = null;

        /// <summary> The second sub domain namespace</summary>
        public string LayerTwo {get; set;} = null;

        /// <summary> The owner of namespace</summary>
        public Account Owner {get; set;}
        
        /// <summary> The expiry date of the namespace</summary>
        public DateTime Expiry {get; set;}
        
        /// <summary> The creation date of the namespace</summary>
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Xarcade Namespace Model==="  +
                "\nNamespace Id: "      + NamespaceId +
                "\nDomain: "            + Domain +
                "\nLayer One: "         + LayerOne + 
                "\nLayer Two: "         + LayerTwo +
                "\nOwner: "             + Owner.UserID +
                "\nDate Expiry: "       + Expiry + 
                "\nDate Created: "      + Created +
                "\n==End of Namespace Model==";
        }
    }
}
