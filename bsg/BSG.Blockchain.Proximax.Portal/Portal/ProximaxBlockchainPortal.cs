using BSG.Blockchain.Portal.Abstract;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ProximaX.Sirius.Chain.Sdk.Client;
using Microsoft.EntityFrameworkCore;

using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Blockchain;
using BlockchainModels = BSG.Blockchain.Models;
using BSG.Blockchain.Controllers;

namespace BSG.Blockchain.Portal.Proximax
{
  public class ProximaxBlockchainPortal : IBlockchainPortal
  {
    private const string PROXIMAX_NODE_URL = "https://bctestnet1.brimstone.xpxsirius.io";
    private static SiriusClient siriusClient = null;
    private static NetworkType network =  NetworkType.MIJIN_TEST;
    public const string TRANSACTION_TIME_STAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
    private const string PROXIMAX_MOSAIC_NAME = "prx.xpx";



    public ProximaxBlockchainPortal()
    {
        if (siriusClient == null)
        {
            siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
        }
        network = this.GetNetworkType().Result;
    }

    private async Task<NetworkType> GetNetworkType()
    {
        NetworkType type = await siriusClient.NetworkHttp.GetNetworkType();
        return type;
    }
    public BlockchainModels.Account GenerateWallet(long userId)
    {
      BlockchainModels.Account output = new BlockchainModels.Account();
      Account account = Account.GenerateNewAccount(network);
      output.Id = userId;
      output.WalletAddress = account.PublicAccount.Address.Plain;
      output.PublicKey = account.PublicKey;
      output.PrivateKey = account.PrivateKey;


      return output;
    }


  }
}