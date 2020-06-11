using System.Reactive.Linq;
using ProximaX.Sirius.Chain.Sdk.Model.Blockchain;
using ProximaX.Sirius.Chain.Sdk.Client;
using System.Threading.Tasks;

namespace Xarcade.Api.Prototype.Blockchain
{
    public class ProximaxBlockchainPortal
    {
        public const string PROXIMAX_NODE_URL = "http://bctestnet1.brimstone.xpxsirius.io:3000"; 
        public SiriusClient siriusClient = null;
        public NetworkType networkType = default(NetworkType);
        public const string TRANSACTION_TIME_STAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        public const string PROXIMAX_MOSAIC_NAME = "same.xpx";

        public ProximaxBlockchainPortal()
        {
            if (siriusClient == null)
            {
                siriusClient = new SiriusClient(ProximaxBlockchainPortal.PROXIMAX_NODE_URL);
            }
            networkType = this.GetNetworkType().Result;
        }

        private async Task<NetworkType> GetNetworkType()
        {
            NetworkType type = await siriusClient.NetworkHttp.GetNetworkType();
            return type;
        }

    }
}