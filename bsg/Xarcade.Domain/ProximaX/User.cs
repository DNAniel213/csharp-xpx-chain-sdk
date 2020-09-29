namespace Xarcade.Domain.ProximaX
{
    /// <summary>
    /// Xarcade User Model
    /// Represents the Xarcade User Account 
    /// </summary>
    public class User : Account
    {
         /// <summary> The API generated unique identifier that represents the Xarcade User. </summary>
        public string OwnerID {get; set;}
    }
}
