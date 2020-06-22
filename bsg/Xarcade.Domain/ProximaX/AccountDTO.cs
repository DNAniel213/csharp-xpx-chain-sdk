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

        

    }
}