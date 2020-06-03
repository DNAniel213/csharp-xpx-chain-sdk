using Microsoft.EntityFrameworkCore;

namespace BSG.Blockchain.Models
{
  public class BlockchainContext: DbContext
  {
    public BlockchainContext(DbContextOptions<BlockchainContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts {get;set;}
  }

}