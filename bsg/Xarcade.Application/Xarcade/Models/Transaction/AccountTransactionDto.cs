using System;
using Xarcade.Application.Xarcade.Models.Account;

namespace Xarcade.Application.Xarcade.Models.Transaction
{
    /// <summary>Xarcade Application Layer AccountTransactionDto</summary>
    public class AccountTransactionDto
    {
        public State Status ;
        public string Hash {get; set;}
        public AccountDto Account {get; set;}
        public ulong BlockNumber {get; set;}
        public DateTime Created {get; set;}
        public override string ToString()
        {
            return
                "===Account Transaction DTO==="  +
                "\nStatus: "         + Status +
                "\nHash: "           + Hash +
                "\nBlockNumber: "    + BlockNumber +
                "\nCreated: "        + Created +
                "\n==End of Account Transaction DTO==";
        }

    }
    
}