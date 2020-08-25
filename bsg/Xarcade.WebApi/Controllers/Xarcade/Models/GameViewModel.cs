using System.Collections.Generic;
using System;

namespace Xarcade.WebApi.Controllers.Xarcade.Models
{
    public class GameViewModel
    {
        public string Name { get; set; }
        public DateTime Expiry { get; set; }
        public List<TokenViewModel> Tokens {get; set;}
    }
}