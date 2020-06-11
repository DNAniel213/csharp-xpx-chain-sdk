using System.Reactive.Linq;
using ProximaX.Sirius.Chain.Sdk.Model.Blockchain;
using ProximaX.Sirius.Chain.Sdk.Client;
using System.Threading.Tasks;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;

namespace Xarcade.Api.Prototype.Blockchain
{
    public class ProximaxBlockchainPortal
    {
        public const string PROXIMAX_NODE_URL = "https://bctestnet1.brimstone.xpxsirius.io"; 
        public SiriusClient siriusClient = null;
        public NetworkType networkType = default(NetworkType);
        public const string TRANSACTION_TIME_STAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        public const string PROXIMAX_MOSAIC_NAME = "same.xpx";

        public string generationHash = null;

        public ProximaxBlockchainPortal()
        {
            if (siriusClient == null)
            {
                siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            }
            networkType = this.GetNetworkType().Result;
            generationHash = siriusClient.BlockHttp.GetGenerationHash().Wait();
        }
        

        private async Task<NetworkType> GetNetworkType()
        {
            NetworkType type = await siriusClient.NetworkHttp.GetNetworkType();
            return type;
        }

        public async Task<bool> SignAndAnnounceTransaction(Account account,Transaction transaction)
        {
            if(generationHash != null)
            {
                var signedTransaction = account.Sign(transaction, generationHash);
                await siriusClient.TransactionHttp.Announce(signedTransaction);
            }
            else 
            {
                //Something bad happens
            }

            
            return true; //TODO change to try-catch result
        }




    }
}