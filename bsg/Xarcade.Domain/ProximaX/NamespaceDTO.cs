using System;

namespace Xarcade.Domain.ProximaX
{
    public class NamespaceDTO
    {
        public string Domain {get; set;}
        public string LayerOne {get; set;}
        public string LayerTwo {get; set;}
        public AccountDTO Owner {get; set;}
        public DateTime Expiry {get; set;}
        public DateTime Created {get; set;}
    }
}