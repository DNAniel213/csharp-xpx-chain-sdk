using System;

namespace Xarcade.Domain.ProximaX
{
    public class TransactionDTO
    {
        public string Hash {get; set;}
        public string Height {get; set;}
        public AssetDTO Asset {get; set;}
        public DateTime Created {get; set;}
    }
}