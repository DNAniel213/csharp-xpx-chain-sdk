using System;
namespace Xarcade.Application.Xarcade.Models.Token
{
    /// <summary>Xarcade Application Layer GameDto Model</summary>
    public class GameDto
    {
        public long GameId {get; set;}
        public string Name {get; set;}
        public long Owner {get; set;} //UserId
        public DateTime Expiry {get; set;}

        public override string ToString()
        {
            return
                "===Game DTO==="  +
                "\nGame ID: "     + GameID +
                "\nName: "        + Name +
                "\nOwner: "       + GameID +
                "\nExpiry: "      + Expiry +
                "\n==End of Game DTO==";
        }

    }
}