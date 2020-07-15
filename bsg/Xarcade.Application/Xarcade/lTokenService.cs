using System.Threading.Tasks;
using Xarcade.Application.Xarcade.Models.TokenTransactionDto;
using Xarcade.Application.Xarcade.Models.TokenDto;
using Xarcade.Application.Xarcade.Models.GameDto;

namespace Xarcade.Application.ProximaX
{
    public interface ITokenService
    {
        /// <summary>
        /// Creates token
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> CreateTokenAsync(TokenDto Token);

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
        Task<TokenTransactionDto> ExtendGameAsync(GameDto Game);

        /// <summary>
        /// Modifies token supply 
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> ModifyTokenSupply(TokenDto Token);

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