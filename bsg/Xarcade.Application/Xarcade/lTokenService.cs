using System.Threading.Tasks;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Token;

namespace Xarcade.Application.ProximaX
{
    public interface ITokenService
    {
        /// <summary>
        /// Creates token
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> CreateTokenAsync(TokenDto Token, string NamespaceName);

        /// <summary>
        /// Creates game
        /// </summary>
        /// <param name="Game"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> CreateGameAsync(GameDto Game);

        /// <summary>
        /// Extends game duration
        /// </summary>
        /// <param name="Game"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> ExtendGameAsync(GameDto Game, ulong duration);

        /// <summary>
        /// Modifies token supply 
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> ModifyTokenSupplyAsync(TokenDto Token);

        /// <summary>
        /// Retrieves the information of a specific token
        /// </summary>
        /// <param name="TokenId"></param>
        /// <returns></returns>
        Task<TokenDto> GetTokenInfoAsync(long TokenId);

        /// <summary>
        /// Retrieves the information of a specific game
        /// </summary>
        /// <param name="GameId"></param>
        /// <returns></returns>
        Task<GameDto> GetGameInfoAsync(long GameId);

    }
}