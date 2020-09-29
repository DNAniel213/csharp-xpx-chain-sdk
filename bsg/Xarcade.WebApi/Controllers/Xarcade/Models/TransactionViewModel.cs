using System;

namespace Xarcade.WebApi.Controllers.Xarcade.Models
{
    public class TransactionViewModel
    {
        public string Hash { get; set; } = null;
        public DateTime Created { get; set; }
        public string Status {get; set;} = null;
    }
}