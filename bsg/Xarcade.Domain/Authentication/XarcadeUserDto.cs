using System;


namespace Xarcade.Domain.Authentication
{
    /// <summary>Xarcade's Account Model</summary>
    public class XarcadeUserDTO
    {
        public long UserID {get; set;}
        public string UserName {get; set;}
        public string Password {get; set;}
        public string Email {get; set;}
        public DateTime Created {get; set;}

        public override string ToString()
        {
            return
                "===XarcadeUser DTO==="  +
                "\nuserID: "            + UserID +
                "\nuserName: "         + UserName + 
                "\npassword: "          + "*******" + 
                "\nemail: "   +         Email +
                "\nDate Created: "   + Created +
                "\n==End of XarcadeUser DTO==";
        }
    }
}