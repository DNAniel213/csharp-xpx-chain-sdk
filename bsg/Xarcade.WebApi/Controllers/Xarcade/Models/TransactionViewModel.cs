namespace Xarcade.WebApi.Controllers.Xarcade.Models
{
    public class TransactionViewModel
    {
        public string Hash { get; set; }
        public DateTime Created { get; set; }
        public string Status {get; set;}
    }
}