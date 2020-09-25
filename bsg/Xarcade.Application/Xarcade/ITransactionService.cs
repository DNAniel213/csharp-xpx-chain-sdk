using System.Threading.Tasks;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Token;
using Xarcade.Application.Xarcade.Models.Account;

namespace Xarcade.Application.Xarcade
{
    public interface ITransactionService
    {
        /// <summary>
        /// Sends a Token from a user to another user
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> SendTokenAsync(TokenDto token,string sender, string receiver, string message);
        
        /// <summary>
        /// Sends an Xar from a user to another user
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> SendXarAsync(TokenDto token,AccountDto sender, AccountDto receiver);
        /// <summary>
        /// Sends a Xpx from a user to another user
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> SendXpxAsync(TokenDto token,AccountDto sender, AccountDto receiver);
    }
} 