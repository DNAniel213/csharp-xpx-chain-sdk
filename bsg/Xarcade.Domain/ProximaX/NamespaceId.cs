using System;

namespace Xarcade.Domain.ProximaX
{
    public class NamespaceId
    {
        public string Name {get; set;}
        public ulong Id {get; set;}

        public override string ToString()
        {
            return
                "===NamespaceId DTO==="  +
                "\nName: "               + Name +
                "\nId: "                 + Id + 
                "\n==End of NamespaceId DTO==";
        }
    }
}