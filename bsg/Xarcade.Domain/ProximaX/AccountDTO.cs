using System;


namespace Xarcade.Domain.Models
{
    /// <summary>Xarcade's Account Model</summary>
    public class AccountDTO
    {
        public long userID {get; set;}
        public string walletAddress {get; set;}
        public string privateKey {get; set;}
        public string publicKey {get; set;}
        public DateTime created {get; set;}

        public override string ToString()
        {

            var privatekey = privateKey != null ? privateKey : "hidden";
            return
                "===Account DTO==="  +
                "\nUser ID: "        + userID +
                "\nWallet Address: " + walletAddress + 
                "\nPublic Key: "     + publicKey + 
                "\nPrivate Key: "    + privateKey+  
                "\nDate Created: "   + created +
                "\n==End of Account DTO==";
 

        }

    }
}