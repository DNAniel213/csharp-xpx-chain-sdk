using System;
namespace Xarcade.Application.Xarcade.Models.Account
{
    /// <summary>Xarcade Application Layer Account DTO Model</summary>
    public class AccountDto
    {
        public string UserID {get; set;}
        public string WalletAddress {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===Account DTO==="  +
                "\nUser ID: "        + UserID +
                "\nWallet Address: " + WalletAddress +  
                "\nDate Created: "   + Created +
                "\n==End of Account DTO==";
        }

    }
}