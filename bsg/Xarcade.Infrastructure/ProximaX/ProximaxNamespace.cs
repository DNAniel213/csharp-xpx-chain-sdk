using ProximaX.Sirius.Chain.Sdk.Model.Namespaces;
using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using ProximaX.Sirius.Chain.Sdk.Model.Transactions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using XarcadeModels = Xarcade.Domain.Models;
using XarcadeParams = Xarcade.Domain.Params;
using System;

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
        public async Task<XarcadeModels.NamespaceDTO> GetNamespaceInformation (string namespaceName)
        {
            NamespaceInfo namespaceInfo = await portal.siriusClient.NamespaceHttp.GetNamespace(new NamespaceId(namespaceName));
            AccountInfo ownerAccountInfo = await portal.siriusClient.AccountHttp.GetAccountInfo(namespaceInfo.Owner.Address);
            XarcadeModels.AccountDTO ownerDTO = new XarcadeModels.AccountDTO
            {
                UserID = 0,
                WalletAddress = ownerAccountInfo.Address.Pretty,
                PrivateKey    = null,
                PublicKey     = ownerAccountInfo.PublicKey,
                Created       = DateTime.Now, //TODO @Dane please get actual creation date
            };
            
            XarcadeModels.NamespaceDTO namespaceDTO = new XarcadeModels.NamespaceDTO
            {
                Domain   = namespaceInfo.Id.Name,
                LayerOne = "",
                LayerTwo = "",
                Owner    = ownerDTO,
                Expiry   = DateTime.Now,   //TODO @John please get actual expiry date
                Created  = DateTime.Now    //TODO @John please get actual creation date
            };

            return namespaceDTO;
        }
    
//TODO Update Namespace
//TODO Delete Namespace
    }
}