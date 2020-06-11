using ProximaX.Sirius.Chain.Sdk.Model.Accounts;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Xarcade.Api.Prototype.Blockchain
{
    /// <summary>
    /// Contains all methods involving an Account
    /// </summary>
    public class ProximaxAccount
    {
        public ProximaxBlockchainPortal portal;
        public ProximaxAccount(ProximaxBlockchainPortal portal)
        {
            this.portal = portal;
        }

        /// <summary>
        /// Generates Wallet Address
        /// </summary>
        /// <param name="userId">Client-generated user ID</param>
        /// <returns></returns>
        public Account CreateAccount(long userId)
        {
            Account account = Account.GenerateNewAccount(portal.networkType);
            return account;
        }

        /// <summary>
        /// Generates Wallet Address based on private Key
        /// </summary>
        /// <param name="userId">Client-generated user ID</param>
        /// <param name="privateKey">external token for private key</param>
        /// <returns></returns>
        public Account CreateAccount(long userId, string privateKey)
        {
            Account account = Account.CreateFromPrivateKey(privateKey, portal.networkType);


            return account;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<AccountInfo> GetAccountInformation(Address address)
        {
            AccountInfo accountInfo = await portal.siriusClient.AccountHttp.GetAccountInfo(address);

            return accountInfo;
        }
        /*
        public async Task<Address> GetAddress()
        {

        }*/




    }
}