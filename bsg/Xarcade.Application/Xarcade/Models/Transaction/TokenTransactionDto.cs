using System;
using Xarcade.Application.Xarcade.Models.Token;

namespace Xarcade.Application.Xarcade.Models.Transaction
{
    /// <summary>Xarcade Application Layer TokenTransactionDto Composition: TokenDto</summary>
    public class TokenTransactionDto
    {
        public enum State {Confirmed, Unconfirmed}
        public State Status {get; set;}
        public string Hash {get; set;}
        public TokenDto Token {get; set;}
        public ulong BlockNumber {get; set;}
        public DateTime Created {get; set;}
        public override string ToString()
        {
            return
                "===Token Transaction DTO==="  +
                "\nStatus: "         + Status +
                "\nHash: "           + Hash +
                "\nBlockNumber: "    + BlockNumber +
                "\nCreated: "        + Created +
                "\n==End of Token Transaction DTO==";
        }

    }
    
}