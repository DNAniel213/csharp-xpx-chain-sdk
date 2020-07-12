using System;
namespace Xarcade.Application.Xarcade.Models.Token
{
    /// <summary>Xarcade Application Layer Token DTO Model</summary>
    public class TokenDto
    {
        public long TokenId {get; set;}
        public string Name {get; set;}
        public ulong Quantity {get; set;}
        public long Owner {get; set;} //UserId
        public CustomTokenDto customeTokenDto ;
        public XarcadeTokenDto xarcadeTokenDto;

        public override string ToString()
        {
            return
                "===Token DTO==="  +
                "\nToken ID: "    + TokenID +
                "\nName: "        + Name +
                "\nQuanity: "     + Quantity +
                "\nOwner: "       + Owner +
                "\n==End of Token DTO==";
        }

    }
}