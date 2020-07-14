using System;
namespace Xarcade.Application.Xarcade.Models.Account
{
    /// <summary>Xarcade Application Layer User Model</summary>
    public class UserDto : AccountDto
    {
        public long OwnerID {get; set;}
        public override string ToString()
        {
            return
                "===User DTO==="  +
                "\nOwner ID: "    + OwnerID +
                "\n==End of User DTO==";
        }

    }
}