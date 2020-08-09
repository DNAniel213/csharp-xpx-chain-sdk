using System.Threading.Tasks;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Account;

namespace Xarcade.Application.Xarcade
{
    public interface IAccountService
    {
        /// <summary>
        /// Creates the Owner Account
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        /// <returns></returns>
        Task<OwnerDto> CreateOwnerAccountAsync(long UserID);

        /// <summary>
        /// Creates the User Account
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        /// <param name="OwnerID">Unique identification that represents the owner</param>
        /// <returns></returns>
        Task<UserDto> CreateUserAccountAsync(long UserID, long OwnerID);

        /// <summary>
        /// Retrieves the information of a specific owner account
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        /// <returns></returns>
        Task<OwnerDto> GetOwnerAccountAsync(long UserID);

        /// <summary>
        /// Retrieves the information of a specific user account
        /// </summary>
        /// <param name="UserID">Unique identification that represents the user</param>
        /// <param name="OwnerID">Unique identification that represents the owner</param>
        /// <returns></returns>
        Task<UserDto> GetUserAccountAsync(long UserID);
    }
}