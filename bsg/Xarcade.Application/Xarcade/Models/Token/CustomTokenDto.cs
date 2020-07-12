using System;
namespace Xarcade.Application.Xarcade.Models.Token
{
    /// <summary>Xarcade Application Layer Custom Token Dto Composition: GameDto</summary>
    public class CustomTokenDto
    {
        public GameDto Property = new GameDto ();
        
        public override string ToString()
        {
            return
                "===Custom Token DTO==="  +
                "\nGame ID: "     + Property.GameId +
                "\nName: "        + Property.Name +
                "\nOwner: "       + Property.Owner +
                "\nExpiry: "      + Property.Expiry +
                "\n==End of Custom Token DTO==";
        }

    }
    
}