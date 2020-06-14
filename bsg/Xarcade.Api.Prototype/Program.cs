using System;
using XarcadeAccount = Xarcade.Api.Prototype.Blockchain.Models;
using System.Reactive.Linq;
using Xarcade.Api.Prototype.Blockchain;
using ProximaX.Sirius.Chain.Sdk.Model.Namespaces;

namespace Xarcade.Api.Prototype
{
    class Program
    {
        public ProximaxBlockchainPortal portal = null;
        public ProximaxAccount pAccount = null;
        public ProximaxMosaic pMosaic = null;
        public ProximaxNamespace pNamespace = null;
        public const string TEST_PRIVATE_KEY_DANE1 = "580B37D8481BDEA4DD1BEA8098EF006AE46DD654FBBC0903418478C1FA363F15"; //Dane 1's account
        public const string TEST_PRIVATE_KEY_DANE2 = "169598F9678F1BE5D959DB8CED4F8FA2297A5124A241E034DC38AD7A0F41B724"; //Dane 2's account
        public const string TEST_PRIVATE_KEY = "90B02ADCFF8D7776EF0016537EBC25709CD1A7F8317CAB3A84DB8FEED52460DC"; //Bruh account
        public const string namespaceName = "scoobydoo";
        static void Main(string[] args)
        {
            Program p = new Program();
            p.portal = new ProximaxBlockchainPortal();
            p.pAccount = new ProximaxAccount(p.portal);
            p.pMosaic = new ProximaxMosaic(p.portal);
            p.pNamespace = new ProximaxNamespace(p.portal);

            //p.CreateMosaicTest();
            //p.SendMosaicTest();
            //p.CreateNamespace();
            //p.CreateSubNamespace();
            p.LinkMosaicToNamespaceTest();
        }
        
        //Creates a mosaic using Dane's private key
        private void CreateMosaicTest()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_KEY_DANE1); 

            var createMosaicT = pMosaic.CreateCoin(account, true, true, false, 0, 1000); 
            var supplyChangeT = pMosaic.ModifyCoinSupply(createMosaicT, 50000);
            
            portal.SignAndAnnounceTransaction(account, createMosaicT).GetAwaiter().GetResult();
            portal.SignAndAnnounceTransaction(account, supplyChangeT).GetAwaiter().GetResult();
        }

        //Sends mosaic from Dane1 to Dane2 accounts. Usually, only need PUBLIC KEY from recepient account 
        private void SendMosaicTest()
        {
            var dane1 = pAccount.CreateAccount(1, TEST_PRIVATE_KEY_DANE1); 
            var dane2 = pAccount.CreateAccount(2, TEST_PRIVATE_KEY_DANE2); 
            var mosaicInfo = pMosaic.GetCoinInfo("10cc5a0ee539b38a").GetAwaiter().GetResult();

            var sendCoinT = pMosaic.SendCoin(mosaicInfo, dane1, dane2.Address, 20, "hope this works.jpg");
            portal.SignAndAnnounceTransaction(dane1, sendCoinT).GetAwaiter().GetResult();
        }

        //Creates a namespace using Bruh's Private Key
        private void CreateNamespace()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_KEY);

            try
            {
                var namespaceInfo = pNamespace.GetNamespaceInformation(namespaceName).GetAwaiter().GetResult();
                throw new ArgumentNullException($"Namespace already exists!");
            }
            catch (Flurl.Http.FlurlHttpException)
            {
                var registerNamespace = pNamespace.CreateNamespace(namespaceName, 100);
                portal.SignAndAnnounceTransaction(account, registerNamespace).GetAwaiter().GetResult();
            }
        }

        //Creates a subnamespace using Bruh's private key and Parent namespace
        private void CreateSubNamespace()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_KEY);

            var subNamespaceName = "wahwah";
            var namespaceId = new NamespaceId(subNamespaceName);
            var parentId = new NamespaceId(namespaceName);

            var registerSubNamespace = pNamespace.CreateSubNamespace(subNamespaceName, parentId);
            portal.SignAndAnnounceTransaction(account, registerSubNamespace).GetAwaiter().GetResult();
        }
        
        //Creates a link between the 'linktest' Namespace and a Mosaic
        private void LinkMosaicToNamespaceTest()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_KEY);
            var namespaceInfo = pNamespace.GetNamespaceInformation("linktest").GetAwaiter().GetResult();
            var mosaicInfo = pMosaic.GetCoinInfo("798b57c1850c523c").GetAwaiter().GetResult();

            var link = pMosaic.LinkNamespaceToMosaic(mosaicInfo, namespaceInfo);
            portal.SignAndAnnounceTransaction(account, link).GetAwaiter().GetResult();
        }
    }
}
