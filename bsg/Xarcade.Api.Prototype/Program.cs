using System.Collections.Generic;
using System;
using XarcadeAccount = Xarcade.Api.Prototype.Blockchain.Models;
using System.Reactive.Linq;
using Xarcade.Api.Prototype.Blockchain;

/*
    BSG ACCOUNT
    wallet uid: blackspeargames
    password: bsg696969
*/

namespace Xarcade.Api.Prototype
{
    class Program
    {
        public ProximaxBlockchainPortal portal = null;
        public ProximaxAccount pAccount = null;
        public ProximaxMosaic pMosaic = null;
        public ProximaxNamespace pNamespace = null;
        public const string TEST_PRIVATE_BSG_1 = "1A90826869ECCBEF591B049745BF3C17EC2A7FA1E9C91787C711194165FE2034"; //BSG 1's account
        public const string TEST_PRIVATE_BSG_2= "8C0C98ED0D0D703B56EB5FCBA55B02BB9661153C44D2C782F54846D902CEC4B5"; //BSG 2's account
        public const string namespaceName = "foobar";
        static void Main(string[] args)
        {
            Program p = new Program();
            p.portal = new ProximaxBlockchainPortal();
            p.pAccount = new ProximaxAccount(p.portal);
            p.pMosaic = new ProximaxMosaic(p.portal);
            p.pNamespace = new ProximaxNamespace(p.portal);

            //p.CreateNewAccountAndSendXpx(); //Creates a new account and sends xpx from BSG_1 to the new account
            p.GetAccountTransactions(); //Gets all transactions from account
            //p.CreateMosaicTest();
            //p.SendMosaicTest();
            //p.CreateNamespace();
            //p.CreateSubNamespace();
            //p.LinkMosaicToNamespaceTest();
        }

        //read the function name
        private void CreateNewAccountAndSendXpx()
        {
            var newAccount = pAccount.CreateAccount(1);
            //Here ibutang ang pag send sa XPXXX

            var newBalance = pAccount.GetAccountInformation(newAccount.PrivateKey).GetAwaiter().GetResult();
            //Console.Writeline new balanceeee
        }

        private void GetAccountTransactions()
        {
            var transactions = pAccount.GetAccountTransactions(TEST_PRIVATE_BSG_1, 10).GetAwaiter().GetResult();
            foreach (var tx in transactions)
            {
                Console.WriteLine("Sender: " + tx.Signer.PublicKey +
                                    " Isconfirmed: " + tx.IsConfirmed());
            }


        }


        //Creates a mosaic using Dane's private key
        private void CreateMosaicTest()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1); 

            var createMosaicT = pMosaic.CreateCoin(account, true, true, false, 0, 1000); 
            var supplyChangeT = pMosaic.ModifyCoinSupply(createMosaicT, 50000);
            
            portal.SignAndAnnounceTransaction(account, createMosaicT).GetAwaiter().GetResult();
            portal.SignAndAnnounceTransaction(account, supplyChangeT).GetAwaiter().GetResult();
        }

        //Sends mosaic from Dane1 to Dane2 accounts. Usually, only need PUBLIC KEY from recepient account 
        private void SendMosaicTest()
        {
            var dane1 = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1); 
            var dane2 = pAccount.CreateAccount(2, TEST_PRIVATE_BSG_2); 
            var mosaicInfo = pMosaic.GetCoinInfo("10cc5a0ee539b38a").GetAwaiter().GetResult();

            var sendCoinT = pMosaic.SendCoin(mosaicInfo, dane1, dane2.Address, 20, "hope this works.jpg");
            portal.SignAndAnnounceTransaction(dane1, sendCoinT).GetAwaiter().GetResult();
        }

        //Creates a namespace using Bruh's Private Key
        private void CreateNamespace()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1);

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
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1);

            var subNamespaceName = "wahwah";

            var registerSubNamespace = pNamespace.CreateSubNamespace(subNamespaceName, namespaceName);
            portal.SignAndAnnounceTransaction(account, registerSubNamespace).GetAwaiter().GetResult();
        }
        
        //Creates a link between the 'linktest' Namespace and a Mosaic
        private void LinkMosaicToNamespaceTest()
        {
            var account = pAccount.CreateAccount(1, TEST_PRIVATE_BSG_1);
            var namespaceInfo = pNamespace.GetNamespaceInformation("linktest").GetAwaiter().GetResult();
            var mosaicInfo = pMosaic.GetCoinInfo("798b57c1850c523c").GetAwaiter().GetResult();

            var link = pMosaic.LinkNamespaceToMosaic(mosaicInfo, namespaceInfo);
            portal.SignAndAnnounceTransaction(account, link).GetAwaiter().GetResult();
        }


    }
}
