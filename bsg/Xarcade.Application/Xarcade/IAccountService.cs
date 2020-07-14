using System.Threading.Tasks;
using Xarcade.Application.Xarcade.Models.Transaction;
using Xarcade.Application.Xarcade.Models.Account;

namespace Xarcade.Application.Xarcade
{
    public interface IAccountService
    {
        Task<AccountTransactionDto> CreateOwnerAccountAsync(long UserID);
        Task<AccountTransactionDto> CreateUserAccountAsync(long UserID, long OwnerID);
        Task<OwnerDto> GetOwnerAccountAsync(long UserID);
        Task<UserDto> GetUserAccountAsync(long UserID, long OwnerID);
    }
}