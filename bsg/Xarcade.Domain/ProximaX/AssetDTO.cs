using System;

namespace Xarcade.Domain.ProximaX
{
    public class AssetDTO
    {
        public string id {get; set;}
        public string Name {get; set;}
        public ulong Quantity {get; set;}
        public AccountDTO Owner {get; set;}
        public DateTime Created {get; set;}
    }
}