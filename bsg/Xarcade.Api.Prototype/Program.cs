using System;
using XarcadeAccount = Xarcade.Api.Prototype.Blockchain.Models;
using System.Reactive.Linq;
using Xarcade.Api.Prototype.Blockchain;
namespace Xarcade.Api.Prototype
{
    class Program
    {
        public ProximaxBlockchainPortal portal = null;
        public ProximaxAccount pAccount = null;
        public ProximaxMosaic pMosaic = null;
        public const string TEST_PRIVATE_KEY = "580B37D8481BDEA4DD1BEA8098EF006AE46DD654FBBC0903418478C1FA363F15"; //Dane's account
        static void Main(string[] args)
        {
            Program p = new Program();
            p.portal = new ProximaxBlockchainPortal();
            p.pAccount = new ProximaxAccount(p.portal);
            p.pMosaic = new ProximaxMosaic(p.portal);

            p.CreateMosaic();

        }

        private void CreateMosaic()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_KEY); 

            var mosaicInfo = pMosaic.CreateCurrency(account, true, true, false, 0, 1000); 
            var supplyChange = pMosaic.ModifyCurrencySupply(mosaicInfo, 50000);
            
            portal.SignAndAnnounceTransaction(account, mosaicInfo).GetAwaiter().GetResult();
            portal.SignAndAnnounceTransaction(account, supplyChange).GetAwaiter().GetResult();
        }
    }
}
