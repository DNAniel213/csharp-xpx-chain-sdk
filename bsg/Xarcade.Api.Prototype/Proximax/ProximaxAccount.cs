using XarcadeAccount = Xarcade.Api.Prototype.Blockchain.Models;
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
        public XarcadeAccount.Account CreateAccount(long userId)
        {
            XarcadeAccount.Account output = new XarcadeAccount.Account();
            Account account = Account.GenerateNewAccount(portal.networkType);
            output.UserId = userId;
            output.WalletAddress = account.PublicAccount.Address.Plain;
            output.PublicKey = account.PublicKey;
            output.PrivateKey = account.PrivateKey;

            return output;
        }

        /// <summary>
        /// Generates Wallet Address based on private Key
        /// </summary>
        /// <param name="userId">Client-generated user ID</param>
        /// <param name="privateKey">external token for private key</param>
        /// <returns></returns>
        public XarcadeAccount.Account CreateAccount(long userId, string privateKey)
        {
            XarcadeAccount.Account output = new XarcadeAccount.Account();
            Account account = Account.CreateFromPrivateKey(privateKey, portal.networkType);
            output.UserId = userId;
            output.WalletAddress = account.PublicAccount.Address.Plain;
            output.PublicKey = account.PublicKey;
            output.PrivateKey = account.PrivateKey;

            return output;
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