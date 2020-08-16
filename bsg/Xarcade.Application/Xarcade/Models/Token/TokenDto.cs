using System;
namespace Xarcade.Application.Xarcade.Models.Token
{
    /// <summary>Xarcade Application Layer Token DTO Model</summary>
    public class TokenDto
    {
        public ulong TokenId {get; set;}
        public string Name {get; set;}
        public ulong Quantity {get; set;}
        public long Owner {get; set;} //UserId

        public override string ToString()
        {
            return
                "===Token DTO==="  +
                "\nToken ID: "    + TokenId +
                "\nName: "        + Name +
                "\nQuanity: "     + Quantity +
                "\nOwner: "       + Owner +
                "\n==End of Token DTO==";
        }

    }
}