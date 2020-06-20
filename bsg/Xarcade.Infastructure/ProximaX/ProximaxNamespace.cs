using ProximaX.Sirius.Chain.Sdk.Model.Namespaces;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Xarcade.Api.Prototype.Blockchain
{
    /// <summary>
    /// Contains all methods involving a Namespace
    /// </summary>
    public class ProximaxNamespace
    {
        public ProximaxBlockchainPortal portal;
        public ProximaxNamespace(ProximaxBlockchainPortal portal)
        {
            this.portal = portal;
        }

        public readonly ulong MAX_FEE = 5000000;
        
        /// <summary>
        /// Registers a new Namespace
        /// </summary>
        /// <param name="parentNamespaceName">The name of the namespace </param>
        /// <param name="duration">The maximum rent duration of the namespace </param>
        /// <returns></returns>
        public RegisterNamespaceTransaction CreateNamespace(string parentNamespaceName, ulong duration)
        {
            var registerNamespace = RegisterNamespaceTransaction.CreateRootNamespace(
                Deadline.Create(),
                parentNamespaceName,
                duration,
                portal.networkType);

            return registerNamespace;
        }

        /// <summary>
        /// Registers a new sub Namespace
        /// </summary>
        /// <param name="namespaceName">The name of the subnamespace </param>
        /// <param name="parentNamespaceId">The name of the parent namespace </param>
        /// <returns></returns>
        public RegisterNamespaceTransaction CreateSubNamespace (string namespaceName, string parentNamespaceId)
        {
            var parentNamespace = new NamespaceId(parentNamespaceId);
            var registerSubNamespace = RegisterNamespaceTransaction.CreateSubNamespace(
                Deadline.Create(),
                namespaceName,
                parentNamespace,
                portal.networkType
            );

            return registerSubNamespace;
        }
        public async Task<NamespaceInfo> GetNamespaceInformation (string namespaceName)
        {
            var namespaceInfo = await portal.siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(namespaceName));

            return namespaceInfo;
        }

        /// <summary>
        /// Extends Namespace Rent Duration
        /// </summary>
        /// <param name="namespaceName">The name of the namespace </param>
        /// <param name="privatekey">The private key of the user </param>
        /// <param name="namespaceInfo">The details of the namespace </param>
        /// <param name="duration">The desired extension duration </param>
        /// <returns></returns>
        public RegisterNamespaceTransaction ExtendNamespaceDuration(string namespaceName,string privateKey,NamespaceInfo namespaceInfo, ulong? duration)
        {
            var namespaceId = new NamespaceId(namespaceName);

            RegisterNamespaceTransaction renew = new RegisterNamespaceTransaction(
                portal.networkType,//network type
                EntityVersion.REGISTER_NAMESPACE.GetValue(),//version
                Deadline.Create(),//deadline
                MAX_FEE,//max fee based from FeeCalculationStrategy
                namespaceName,//namespace Name
                namespaceId,//namespace Id
                namespaceInfo.Type,//namespace Type
                duration,//duration
                null,//parent Id 
                null,//signature 
                null,//signer 
                null//transaction Info 
                );
            return renew;
        }
    
//TODO Update Namespace
//TODO Delete Namespace
    }
}