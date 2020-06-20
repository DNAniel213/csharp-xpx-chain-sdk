using System;

namespace Xarcade.Domain.Models
{
    public class TransactionDTO
    {
        public string Hash {get; set;}
        public ulong Height {get; set;}
        public AssetDTO Asset {get; set;}
        public DateTime Created {get; set;}
    }
}