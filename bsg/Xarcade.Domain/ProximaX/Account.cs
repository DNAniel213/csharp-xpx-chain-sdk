using System;
namespace Xarcade.Domain.ProximaX
{
    /// <summary>
    /// Xarcade Account Model
    /// Represents the xarcade user account
    /// </summary>
    public class Account
    {
        /// <summary> The unique identifier that represents the xarcade user </summary>
        public string UserID {get; set;}

        /// <summary> The xarcade user's wallet address </summary>
        public string WalletAddress {get; set;}

        /// <summary> The xarcade user's encrypted private key </summary>
        public string PrivateKey {get; set;}

        /// <summary> The xarcade user's blockchain generated public key </summary>
        public string PublicKey {get; set;}
        
        /// <summary> The xarcade user's account creation date </summary>
        public DateTime Created {get; set;}

        public override string ToString()
        {

            var privateKeyString = PrivateKey != null ? PrivateKey : "hidden";
            return
                "===Xarcade Account Model===" +
                "\nUser ID: "        + UserID +
                "\nWallet Address: " + WalletAddress + 
                "\nPublic Key: "     + PublicKey + 
                "\nPrivate Key: "    + privateKeyString+  
                "\nDate Created: "   + Created +
                "\n==End of Xarcade Account Model==";
        }
    }
}
