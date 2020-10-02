using System.Collections.Generic;
using System;

namespace Xarcade.WebApi.Controllers.Xarcade.Models
{
    public class GameViewModel
    {
        public string Name { get; set; } = null;
        public DateTime Expiry { get; set; }
        public TokenViewModel Token {get; set;} = null;
    }
}