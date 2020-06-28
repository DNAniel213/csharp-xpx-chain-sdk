using System;


namespace Xarcade.Domain.Models
{
    /// <summary>Xarcade's Account Model</summary>
    public class AccountDTO
    {
        public long UserID {get; set;}
        public string WalletAddress {get; set;}
        public string PrivateKey {get; set;}
        public string PublicKey {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {

            var privatekey = PrivateKey != null ? PrivateKey : "hidden";
            return
                "===Account DTO==="  +
                "\nUser ID: "        + UserID +
                "\nWallet Address: " + WalletAddress + 
                "\nPublic Key: "     + PublicKey + 
                "\nPrivate Key: "    + PrivateKey+  
                "\nDate Created: "   + Created +
                "\n==End of Account DTO==";
 

        }

    }
}