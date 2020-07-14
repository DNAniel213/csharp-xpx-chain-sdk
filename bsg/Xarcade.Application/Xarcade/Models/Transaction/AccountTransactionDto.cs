using System;
using Xarcade.Application.Xarcade.Models.Account;

namespace Xarcade.Application.Xarcade.Models.Transaction
{
    /// <summary>Xarcade Application Layer AccountTransactionDto Composition: AccountDto</summary>
    public class AccountTransactionDto
    {
        public string Hash {get; set;}
        public AccountDto Account {get; set;}
        public ulong BlockNumber {get; set;}
        public DateTime Created {get; set;}
        public override string ToString()
        {
            return
                "===Account Transaction DTO==="  +
                "\nHash: "           + Hash +
                "\nBlockNumber: "    + BlockNumber +
                "\nCreated: "        + Created +
                "\n==End of Account Transaction DTO==";
        }

    }
    
}