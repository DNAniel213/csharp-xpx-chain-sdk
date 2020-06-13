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
        public RegisterNamespaceTransaction CreateSubNamespace (string namespaceName, NamespaceId parentNamespaceId)
        {
            var registerSubNamespace = RegisterNamespaceTransaction.CreateSubNamespace(
                Deadline.Create(),
                namespaceName,
                parentNamespaceId,
                portal.networkType
            );

            return registerSubNamespace;
        }
        public async Task<NamespaceInfo> GetNamespaceInformation (string namespaceName)
        {
            NamespaceInfo namespaceInfo = await portal.siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(namespaceName));

            return namespaceInfo;
        }
    
//TODO Update Namespace
//TODO Delete Namespace
    }
}