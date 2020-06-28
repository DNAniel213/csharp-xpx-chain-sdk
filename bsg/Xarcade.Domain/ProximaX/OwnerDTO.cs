namespace Xarcade.Domain.Models
{
    public class OwnerDTO : AccountDTO 
    {
        public string email {get; set;}
        public string userName {get; set;}
        public string password {get; set;}
        public string fName {get; set;}
        public string lName {get; set;}
    }
}