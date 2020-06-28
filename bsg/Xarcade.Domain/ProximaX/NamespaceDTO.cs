using System;

namespace Xarcade.Domain.Models
{
    public class NamespaceDTO
    {
        public string Domain {get; set;}
        public string LayerOne {get; set;}
        public string LayerTwo {get; set;}
        public AccountDTO Owner {get; set;}
        public DateTime Expiry {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Namespace DTO==="  +
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