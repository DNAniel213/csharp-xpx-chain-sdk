using System;

namespace Xarcade.Domain.ProximaX
{
    public class Namespace
    {
        public long NamespaceId {get; set;}
        public string Domain {get; set;}
        public string LayerOne {get; set;}
        public string LayerTwo {get; set;}
        public Account Owner {get; set;}
        public DateTime Expiry {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Namespace DTO==="  +
                "\nNamespace Id: "      + NamespaceId +
                "\nDomain: "            + Domain +
                "\nLayer One: "         + LayerOne + 
                "\nLayer Two: "         + LayerTwo +
                "\nOwner: "             + Owner +
                "\nDate Expiry: "       + Expiry + 
                "\nDate Created: "      + Created +
                "\n==End of Transaction DTO==";
        }
    }
}