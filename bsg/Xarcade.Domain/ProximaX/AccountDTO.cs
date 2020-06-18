using System;

namespace Xarcade.Domain.ProximaX
{
    public class AccountDTO
    {
        public long UserId {get; set;}
        public string WalletAddress {get; set;}
        public string PrivateKey {get; set;}
        public string PublicKey {get; set;}
        public DateTime Created {get; set;}

    }
}