using System;

namespace Xarcade.Domain.Models
{
    public class AssetDTO
    {
        public string AssetID {get; set;}
        public string Name {get; set;}
        public ulong Quantity {get; set;}
        public AccountDTO Owner {get; set;}
        public DateTime Created {get; set;}
    }
}