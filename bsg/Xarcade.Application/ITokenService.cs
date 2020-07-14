using System.Collections.Generic;
using System.Threading.Tasks;
using Xarcade.Infrastructure.ProximaX.Params;
using Xarcade.Domain.ProximaX;

namespace Xarcade.Application.ProximaX
{
    public interface ITokenService
    {
        /// <summary>
        /// Create Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> CreateTokenAsync(TokenDto token);

        /// <summary>
        /// Create Game
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> CreateGameAsync(GameDto game);

        /// <summary>
        /// Extend Game rent duration
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> ExtendGameAsync(GameDto game);

        /// <summary>
        /// Modify token supply 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<TokenTransactionDto> ModifyTokenSupply(TokenDto token);

        /// <summary>
        /// Retrieve Token information from token id
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<TokenDto> GetTokenInfoAsync(ulong tokenId);

        /// <summary>
        /// Retrieve Game information from game id
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        Task<GameDto> GetGameInfoAsync(ulong gameId);

    }
}