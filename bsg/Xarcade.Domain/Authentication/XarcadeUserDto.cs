using System;


namespace Xarcade.Domain.Models
{
    /// <summary>Xarcade's Account Model</summary>
    public class XarcadeUserDTO
    {
        public long userID {get; set;}
        public string userName {get; set;}
        public string password {get; set;}
        public string email {get; set;}
        public DateTime created {get; set;}

        public override string ToString()
        {
            return
                "===XarcadeUser DTO==="  +
                "\nuserID: "            + userID +
                "\nuserName: "         + userName + 
                "\npassword: "          + "*******" + 
                "\nemail: "   +         email +
                "\nDate Created: "   + created +
                "\n==End of XarcadeUser DTO==";
        }
    }
}