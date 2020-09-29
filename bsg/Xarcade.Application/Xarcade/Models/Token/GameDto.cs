using System;
namespace Xarcade.Application.Xarcade.Models.Token
{
    /// <summary>Xarcade Application Layer GameDto Model</summary>
    public class GameDto
    {
        public string GameId {get; set;}
        public string Name {get; set;}
        public string Owner {get; set;} //UserId
        public DateTime Expiry {get; set;}

        public override string ToString()
        {
            return
                "===Game DTO==="  +
                "\nGame ID: "     + GameId +
                "\nName: "        + Name +
                "\nOwner: "       + Owner +
                "\nExpiry: "      + Expiry +
                "\n==End of Game DTO==";
        }

    }
}