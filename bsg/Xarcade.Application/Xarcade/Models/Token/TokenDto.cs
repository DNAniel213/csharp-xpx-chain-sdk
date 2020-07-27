using System;
namespace Xarcade.Application.Xarcade.Models.Token
{
    /// <summary>Xarcade Application Layer Token DTO Model</summary>
    public class TokenDto
    {
        public long TokenId {get; set;}//mosaic Id
        public string Name {get; set;}//namespace connected to mosaic
        public ulong Quantity {get; set;}//supply
         public long Owner {get; set;} //userId

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