using System.Threading.Tasks;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Application.Xarcade.Models.Account;

namespace Xarcade.Application.Xarcade
{
    public interface ITransactionService
    {
        Task<TokenTransactionDto> SendTokenAsync(TokenDto token,AccountDto sender, AccountDto receiver);
        Task<TokenTransactionDto> SendXarAsync(TokenDto token,AccountDto sender, AccountDto receiver);
        Task<TokenTransactionDto> SendXpxAsync(TokenDto token,AccountDto sender, AccountDto receiver);
    }
} 